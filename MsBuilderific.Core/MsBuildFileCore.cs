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

        /// <summary>
        /// Initializes a new instance of the <see cref="MsBuildFileCore"/> class.
        /// </summary>
        /// <param name="visitors">The visitors.</param>
        public MsBuildFileCore(IEnumerable<IBuildOrderVisitor> visitors)
        {
            _visitors = visitors.ToList();
        }

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
        /// <param name="coreOptions">The coreOptions that will be passed to the visitors</param>
        public void WriteBuildScript(List<VisualStudioProject> dependencyOrder, IMsBuilderificCoreOptions coreOptions)
        {
            if (dependencyOrder == null)
                return;

            var cleanStringBuilder = new StringBuilder();
            var buildStringBuilder = new StringBuilder();
            var preBuildStringBuilder = new StringBuilder();
            var postBuildStringBuilder = new StringBuilder();            
            var serviceStringBuilder = new StringBuilder();
            var preServiceStringBuilder = new StringBuilder();
            var postServiceStringBuilder = new StringBuilder();

            foreach (var vsnetProject in dependencyOrder)
            {
                if (vsnetProject == null)
                    continue;

                var currentVisualStudioProject = vsnetProject;

                var sortedVisitors = _visitors.OrderBy(s => s.Order);
                sortedVisitors.ToList().ForEach(v =>
                                      {
                                          if (v == null || !v.ShallExecute(coreOptions))
                                              return;

                                          cleanStringBuilder.AppendLine(v.PreVisitCleanTarget(currentVisualStudioProject, coreOptions));
                                          cleanStringBuilder.AppendLine(v.VisitCleanTarget(currentVisualStudioProject, coreOptions));
                                          cleanStringBuilder.AppendLine(v.PostVisitCleanTarget(currentVisualStudioProject, coreOptions));

                                          preBuildStringBuilder.AppendLine(v.PreVisitBuildTarget(currentVisualStudioProject, coreOptions));
                                          if (currentVisualStudioProject.IsWebProject)
                                              buildStringBuilder.AppendLine(v.VisitBuildWebProjectTarget(currentVisualStudioProject, coreOptions));
                                          else if (currentVisualStudioProject.OutputType != ProjectOutputType.Library.ToString())
                                              buildStringBuilder.AppendLine(v.VisitBuildExeProjectTarget(currentVisualStudioProject, coreOptions));
                                          else
                                              buildStringBuilder.AppendLine(v.VisitBuildLibraryProjectTarget(currentVisualStudioProject, coreOptions));
                                          
                                          buildStringBuilder.AppendLine(v.VisitBuildAllTypeTarget(currentVisualStudioProject, coreOptions));
                                          
                                          postBuildStringBuilder.AppendLine(v.PostVisitBuildTarget(currentVisualStudioProject, coreOptions));

                                          if (coreOptions.GenerateSpecificTargetForWebProject && currentVisualStudioProject.IsWebProject)
                                          {
                                              preServiceStringBuilder.AppendLine(v.PreVisitServiceTarget(currentVisualStudioProject, coreOptions));
                                              serviceStringBuilder.AppendLine(v.VisitServiceTarget(currentVisualStudioProject, coreOptions));
                                              postServiceStringBuilder.AppendLine(v.PostVisitServiceTarget(currentVisualStudioProject, coreOptions));
                                          }
                                      });                              
            }

            buildStringBuilder.Insert(0, preBuildStringBuilder.ToString());
            buildStringBuilder.Append(postBuildStringBuilder.ToString());

            serviceStringBuilder.Insert(0, preServiceStringBuilder.ToString());
            serviceStringBuilder.Append(postServiceStringBuilder.ToString());
            
            var clean = Regex.Replace(cleanStringBuilder.ToString(), @"(?m)^[ \t]*\r?\n", string.Empty, RegexOptions.Multiline);
            var build = Regex.Replace(buildStringBuilder.ToString(), @"(?m)^[ \t]*\r?\n", string.Empty, RegexOptions.Multiline);
            var service = Regex.Replace(serviceStringBuilder.ToString(), @"(?m)^[ \t]*\r?\n", string.Empty, RegexOptions.Multiline);
            var output = string.Format(Properties.Resources.msbuildtemplate, coreOptions.CopyOutputTo, clean, build, service);

            output = output.Replace("$OutputPath$", coreOptions.MsBuildFileOuputPath);
            File.WriteAllText(coreOptions.MsBuildOutputFilename, output);
        }

        #endregion
    }
}
