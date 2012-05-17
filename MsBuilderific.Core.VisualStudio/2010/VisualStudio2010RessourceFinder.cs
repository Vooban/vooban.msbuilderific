using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MsBuilderific.Contracts;

namespace MsBuilderific.Core.VisualStudio
{
    /// <summary>
    /// This class provides .csproj and .vbproj parsing capabilities
    /// </summary>
    public class VisualStudio2010RessourceFinder : IProjectRessourceFinder
    {
        #region Public methods

        /// <summary>
        /// Parses the visual studio project and return the resulting
        /// </summary>
        /// <returns>
        /// An instance of the class <see cref="VisualStudioProject"/> representing the project
        /// </returns>
        public IEnumerable<String> ExtractRessourcesFromProject(string visualStudioProjectPath)
        {
            if (String.IsNullOrEmpty(visualStudioProjectPath))
                throw new ArgumentNullException("The project path must not be null or empty");

            if (!visualStudioProjectPath.Contains(".csproj") && !visualStudioProjectPath.Contains(".vbproj"))
                throw new ArgumentException("The project path must be either a CSharp project or VBProject", "projectPath");

            var xproject = GetProjectAsXDocument(visualStudioProjectPath);

            if (xproject != null && xproject.Root != null)
            {
                var root = xproject.Root;

                var projectInfo = (from item in root.Elements("PropertyGroup")
                                   where !item.HasAttributes && item.Element("ProjectGuid") != null &&
                                   root.Elements("ItemGroup").Elements("Compile") != null &&
                                   root.Elements("ItemGroup").Elements("Compile").Any() &&
                                   item.Element("RootNamespace") != null && item.Element("AssemblyName") != null
                                   select new
                                   {
                                       RootNamespace = item.Element("RootNamespace").Value,
                                       AssemblyName = item.Element("AssemblyName").Value,
                                       ProjectGuid = Guid.Parse(item.Element("ProjectGuid").Value),
                                       Path = visualStudioProjectPath
                                   }).FirstOrDefault();

                if (projectInfo != null)
                {
                    var ressourceInfo = (from item in root.Elements("ItemGroup").Elements("EmbeddedResource")
                                         where item.Element("LastGenOutput") != null
                                         select new{
                                                       LastGenOutput = item.Element("LastGenOutput").Value,
                                                       projectInfo.AssemblyName,
                                                       ProjectPath = visualStudioProjectPath
                                                   });

                        var ressources = ressourceInfo.Select(p =>
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
        private XDocument GetProjectAsXDocument(string visualStudioProjectPath)
        {
            var xproject = XDocument.Load(visualStudioProjectPath);

            if (xproject.Root != null)
            {
                foreach (XElement e in xproject.Root.DescendantsAndSelf())
                {
                    if (e.Name.Namespace != XNamespace.None)
                        e.Name = XNamespace.None.GetName(e.Name.LocalName);

                    if (e.Attributes().Any(a => a.IsNamespaceDeclaration || a.Name.Namespace != XNamespace.None))
                        e.ReplaceAttributes(e.Attributes().Select(a => a.IsNamespaceDeclaration ? null : a.Name.Namespace != XNamespace.None ? new XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value) : a));
                }
            }

            return xproject;
        }

        #endregion
    }
}
