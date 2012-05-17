using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Visitors;
using MsBuilderific.Extensions;
using MsBuilderific.Visitors;

namespace MsBuilderific
{
    /// <summary>
    /// Class reponsible for generating the MsBuild file
    /// </summary>
    public class MsBuildFileCore
    {
        #region Private Members

        private readonly IMsBuilderificOptions _options;
        private readonly List<BuildOrderVisitor> _visitors = new List<BuildOrderVisitor>();

        #endregion

        #region Public Properties

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of the <see cref="MsBuildFileGenerator"/> class.
        /// </summary>
        /// <param name="options">The option used by the core to create the msbuild file</param>        
        public MsBuildFileCore(IMsBuilderificOptions options)
        {
            _options = options;
        }

        #endregion

        #region "Visitor pattern"

        /// <summary>
        /// Accepts a new visitor into the msbuild file's generation core 
        /// </summary>
        /// <param name="newVisitor">The new visitor which will be called in the file generation process</param>
        public void AcceptVisitor(BuildOrderVisitor newVisitor)
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
        public void KickOutVisitor(BuildOrderVisitor kickedVisitor)
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
        public void WriteBuildScript(List<VisualStudioProject> dependencyOrder)
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

            foreach (var vbproject in dependencyOrder)
            {
                if (vbproject == null)
                    continue;

                _visitors.ForEach(v =>
                                      {
                                          if (v == null || !v.ShallExecute(_options))
                                              return;

                                          cleanBuilder.AppendLine(v.PreVisitCleanTarget(vbproject, _options));
                                          cleanBuilder.AppendLine(v.VisitCleanTarget(vbproject, _options));
                                          cleanBuilder.AppendLine(v.PostVisitCleanTarget(vbproject, _options));

                                          preBuildBuilder.AppendLine(v.PreVisitBuildTarget(vbproject, _options));
                                          if (vbproject.IsWebProject)
                                              buildBuilder.AppendLine(v.VisitBuildWebProjectTarget(vbproject, _options));
                                          else if (vbproject.OutputType != ProjectOutputType.Library.ToString())
                                              buildBuilder.AppendLine(v.VisitBuildExeProjectTarget(vbproject, _options));
                                          else
                                              buildBuilder.AppendLine(v.VisitBuildLibraryProjectTarget(vbproject, _options));
                                          buildBuilder.AppendLine(v.VisitBuildAllTypeTarget(vbproject, _options));
                                          postBuildBuilder.AppendLine(v.PostVisitBuildTarget(vbproject, _options));

                                          if (_options.ServiceSpecificTarget && vbproject.IsWebProject)
                                          {
                                              preServiceBuilder.AppendLine(v.PreVisitServiceTarget(vbproject, _options));
                                              serviceBuilder.AppendLine(v.VisitServiceTarget(vbproject, _options));
                                              postServiceBuilder.AppendLine(v.PostVisitServiceTarget(vbproject, _options));
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
            var output = string.Format(Properties.Resources.msbuildtemplate, _options.CopyOutputTo, clean, build, service);

            output = output.Replace("$OutputPath$", _options.OutputPath);
            File.WriteAllText(_options.OutputFile, output);
        }

        #endregion
    }
}
