using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MsBuilderific
{
    /// <summary>
    /// This class provides .csproj and .vbproj parsing capabilities
    /// </summary>
    public class RessourceFinder

    {
        #region Private Members

        private readonly String _projectPath;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of the class <see cref="ProjectLoader"/>.
        /// </summary>
        /// <param name="projectPath">Path to the .csproj or .vbproj</param>
        public RessourceFinder(string projectPath)
        {
            if (!projectPath.Contains(".csproj") && !projectPath.Contains(".vbproj"))
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
        public IEnumerable<String> Parse()
        {
            var xproject = GetProjectAsXDocument();

            if (xproject != null && xproject.Root != null)
            {
                var root = xproject.Root;

                var projectInfo = (from item in root.Elements("PropertyGroup")
                                   where !item.HasAttributes && item.Element("ProjectGuid") != null &&
                                   root.Elements("ItemGroup").Elements("Compile") != null &&
                                   root.Elements("ItemGroup").Elements("Compile").Count() > 0 &&
                                   item.Element("RootNamespace") != null && item.Element("AssemblyName") != null
                                   select new
                                   {
                                       RootNamespace = item.Element("RootNamespace").Value,
                                       AssemblyName = item.Element("AssemblyName").Value,
                                       ProjectGuid = Guid.Parse(item.Element("ProjectGuid").Value),
                                       Path = _projectPath
                                   }).FirstOrDefault();

                if (projectInfo != null)
                {
                    var ressourceInfo = (from item in root.Elements("ItemGroup").Elements("EmbeddedResource")
                                         where item.Element("LastGenOutput") != null
                                         select new{
                                                       LastGenOutput = item.Element("LastGenOutput").Value,
                                                       AssemblyName = projectInfo.AssemblyName,
                                                       ProjectPath = _projectPath
                                                   });

                        var ressources = ressourceInfo.Select((p) =>
                                                                  {
                                                                      var split = p.LastGenOutput.Split('.');

                                                                      if (split.Length > 3 && split[1].Length == 2)
                                                                          return split[1];

                                                                      return null;
                                                                  }).Distinct();

                    return ressources.AsEnumerable();
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

        #endregion
    }
}
