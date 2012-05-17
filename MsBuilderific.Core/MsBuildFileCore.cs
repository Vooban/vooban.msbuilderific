using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Core
{
    /// <summary>
    /// Class reponsible for generating the MsBuild file
    /// </summary>
    public class MsBuildFileCore : IMsBuildFileCore
    {
        #region Private Members

        private readonly List<IBuildOrderVisitor> _visitors = new List<IBuildOrderVisitor>();

        #endregion

        #region Public Properties

        #endregion

        #region Constructors

        #endregion

        #region "Visitor pattern"

        /// <summary>
        /// Accepts a new visitor into the msbuild file's generation core 
        /// </summary>
        /// <param name="newVisitor">The new visitor which will be called in the file generation process</param>
        public void AcceptVisitor(IBuildOrderVisitor newVisitor)
        {
            if (newVisitor == null)
                throw new ArgumentNullException("newVisitor", "Vous ne pouvez pas ajouter un visiteur nul.");

            newVisitor.Order = _visitors.Count + 1;
            _visitors.Add(newVisitor);
        }

        /// <summary>
        /// Removes a visitor from the generation process
        /// </summary>
        /// <param name="kickedVisitor">The visitor to remove from the generation process</param>
        public void KickOutVisitor(IBuildOrderVisitor kickedVisitor)
        {
            _visitors.Add(kickedVisitor);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Generates a build script from the projects' dependency order
        /// </summary>
        /// <param name="dependencyOrder">
        /// The order in which the MsBuildScript shall be generated
        /// </param>
        /// <param name="options">The options that will be passed to the visitors</param>
        public void WriteBuildScript(List<VisualStudioProject> dependencyOrder, IMsBuilderificOptions options)
        {
            if (dependencyOrder == null)
                return;

            var cleanBuilder = new StringBuilder();
            var buildBuilder = new StringBuilder();
            var preBuildBuilder = new StringBuilder();
            var postBuildBuilder = new StringBuilder();            
            var serviceBuilder = new StringBuilder();
            var preServiceBuilder = new StringBuilder();
            var postServiceBuilder = new StringBuilder();

            foreach (var vsnetProject in dependencyOrder)
            {
                if (vsnetProject == null)
                    continue;

                var currentVisualStudioProject = vsnetProject;

                var sortedVisitors = _visitors.OrderBy(s => s.Order);
                sortedVisitors.ToList().ForEach(v =>
                                      {
                                          if (v == null || !v.ShallExecute(options))
                                              return;

                                          cleanBuilder.AppendLine(v.PreVisitCleanTarget(currentVisualStudioProject, options));
                                          cleanBuilder.AppendLine(v.VisitCleanTarget(currentVisualStudioProject, options));
                                          cleanBuilder.AppendLine(v.PostVisitCleanTarget(currentVisualStudioProject, options));

                                          preBuildBuilder.AppendLine(v.PreVisitBuildTarget(currentVisualStudioProject, options));
                                          if (currentVisualStudioProject.IsWebProject)
                                              buildBuilder.AppendLine(v.VisitBuildWebProjectTarget(currentVisualStudioProject, options));
                                          else if (currentVisualStudioProject.OutputType != ProjectOutputType.Library.ToString())
                                              buildBuilder.AppendLine(v.VisitBuildExeProjectTarget(currentVisualStudioProject, options));
                                          else
                                              buildBuilder.AppendLine(v.VisitBuildLibraryProjectTarget(currentVisualStudioProject, options));
                                          buildBuilder.AppendLine(v.VisitBuildAllTypeTarget(currentVisualStudioProject, options));
                                          postBuildBuilder.AppendLine(v.PostVisitBuildTarget(currentVisualStudioProject, options));

                                          if (options.ServiceSpecificTarget && currentVisualStudioProject.IsWebProject)
                                          {
                                              preServiceBuilder.AppendLine(v.PreVisitServiceTarget(currentVisualStudioProject, options));
                                              serviceBuilder.AppendLine(v.VisitServiceTarget(currentVisualStudioProject, options));
                                              postServiceBuilder.AppendLine(v.PostVisitServiceTarget(currentVisualStudioProject, options));
                                          }
                                      });                              
            }

            buildBuilder.Insert(0, preBuildBuilder.ToString());
            buildBuilder.Append(postBuildBuilder.ToString());

            serviceBuilder.Insert(0, preServiceBuilder.ToString());
            serviceBuilder.Append(postServiceBuilder.ToString());
            
            var clean = Regex.Replace(cleanBuilder.ToString(), @"(?m)^[ \t]*\r?\n", string.Empty, RegexOptions.Multiline);
            var build = Regex.Replace(buildBuilder.ToString(), @"(?m)^[ \t]*\r?\n", string.Empty, RegexOptions.Multiline);
            var service = Regex.Replace(serviceBuilder.ToString(), @"(?m)^[ \t]*\r?\n", string.Empty, RegexOptions.Multiline);
            var output = string.Format(Properties.Resources.msbuildtemplate, options.CopyOutputTo, clean, build, service);

            output = output.Replace("$OutputPath$", options.OutputPath);
            File.WriteAllText(options.OutputFile, output);
        }

        #endregion
    }
}
