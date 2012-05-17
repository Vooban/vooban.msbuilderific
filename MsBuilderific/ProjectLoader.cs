using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MsBuilderific.Contracts;

namespace MsBuilderific
{
    /// <summary>
    /// This class provides .csproj and .vbproj parsing capabilities
    /// </summary>
    public class ProjectLoader
    {
        #region Private Members

        private readonly String _projectPath;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of the class <see cref="ProjectLoader"/>.
        /// </summary>
        /// <param name="projectPath">Path to the .csproj or .vbproj</param>
        public ProjectLoader(string projectPath)
        {
            if (!string.IsNullOrEmpty(projectPath) && !projectPath.Contains(".csproj") && !projectPath.Contains(".vbproj"))
                throw new ArgumentException("The project path must be either a CSharp project or VBProject", "projectPath");

            _projectPath = projectPath;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Parses the visual studio project and return the resulting
        /// </summary>
        /// <returns>
        /// An instance of the class <see cref="VisualStudioProject"/> representing the project
        /// </returns>
        public VisualStudioProject Parse()
        {
            var xproject = GetProjectAsXDocument();

            if (xproject != null && xproject.Root != null)
            {
                var root = xproject.Root;
                
                var projectInfo = (from item in root.Elements("PropertyGroup")
                                   where !item.HasAttributes && item.Element("ProjectGuid") != null && 
                                   root.Elements("ItemGroup").Elements("Compile") != null && 
                                   root.Elements("ItemGroup").Elements("Compile").Count() > 0 &&
                                   item.Element("RootNamespace") != null && item.Element("AssemblyName") != null && item.Element("OutputType") != null
                                   select new{
                                                 RootNamespace = item.Element("RootNamespace").Value,
                                                 AssemblyName = item.Element("AssemblyName").Value,
                                                 ProjectGuid = Guid.Parse(item.Element("ProjectGuid").Value),
                                                 OutputType = item.Element("OutputType").Value,
                                                 Path = _projectPath
                                             }).FirstOrDefault();

                if (projectInfo != null)
                {
                    var result = new VisualStudioProject(projectInfo.ProjectGuid, projectInfo.AssemblyName, projectInfo.RootNamespace, projectInfo.Path){IsWebProject = IsWebBuild(xproject)};

                    result.Dependencies.AddRange(FindFileReferences(root));
                    result.Dependencies.AddRange(FindProjectReferences(root));
                    result.IsTestProject = result.Dependencies.Contains("Microsoft.VisualStudio.QualityTools.UnitTestFramework");
                    result.OutputType = projectInfo.OutputType;
                    return result;
                }
            }

            return null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get the visual studio project as a <see cref="XDocument"/> instance, without xml namespaces
        /// </summary>
        /// <returns>
        /// The <see cref="XDocument"/> object
        /// </returns>
        private XDocument GetProjectAsXDocument()
        {
            var xproject = XDocument.Load(_projectPath);

            if (xproject.Root != null)
            {
                foreach (XElement e in xproject.Root.DescendantsAndSelf())
                {
                    if (e.Name.Namespace != XNamespace.None)
                        e.Name = XNamespace.None.GetName(e.Name.LocalName);

                    if (e.Attributes().Where(a => a.IsNamespaceDeclaration || a.Name.Namespace != XNamespace.None).Any())
                        e.ReplaceAttributes(e.Attributes().Select(a => a.IsNamespaceDeclaration ? null : a.Name.Namespace != XNamespace.None ? new XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value) : a));
                }
            }

            return xproject;
        }

        /// <summary>
        /// Finds project references in the current project
        /// </summary>
        /// <param name="xproject">
        /// The <see cref="XDocument"/> from which to extract information
        /// </param>
        /// <returns>
        /// The list of assembly names that are file references
        /// </returns>
        private static IEnumerable<String> FindProjectReferences(XContainer xproject)
        {
            var projectReferences = from item in xproject.Elements("ItemGroup").Elements("ProjectReference")
                                   select item.Element("Name").Value.Split(',')[0];

            return projectReferences.ToList();
        }

        /// <summary>
        /// Finds file references in the current project
        /// </summary>
        /// <param name="xproject">
        /// The <see cref="XDocument"/> from which to extract information
        /// </param>
        /// <returns>
        /// The list of assembly names that are file references
        /// </returns>
        private static IEnumerable<string> FindFileReferences(XContainer xproject)
        {
            // where item.Element("HintPath") != null
            var references = from item in xproject.Elements("ItemGroup").Elements("Reference")                            
                            select item.Attribute("Include").Value.Split(',')[0];

            return references.ToList();
        }

        /// <summary>
        /// Check if the project contained in the <see cref="XDocument"/> is a web project
        /// </summary>
        /// <param name="xproject">The <see cref="XDocument"/> loaded with the project file</param>
        /// <returns>
        /// <c>True</c> if the project is a web project, <c>false</c> otherwise
        /// </returns>
        private static bool IsWebBuild(XDocument xproject)
        {
            var isweb = false;
            if (xproject.Root != null)
            {
                var projectExtensions = xproject.Root.Element("ProjectExtensions");
                if (projectExtensions != null)
                {
                    var visualStudio = projectExtensions.Element("VisualStudio");
                    if (visualStudio != null)
                    {
                        var flavor = visualStudio.Element("FlavorProperties");
                        if (flavor != null)
                            isweb = flavor.Element("WebProjectProperties") != null;
                    }
                }
            }

            return isweb;
        }

        #endregion
    }
}
