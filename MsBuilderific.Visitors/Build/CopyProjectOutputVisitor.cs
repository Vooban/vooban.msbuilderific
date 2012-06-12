using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class CopyProjectOutputVisitor : BuildOrderVisitor
    {
        private readonly CopyProjectOutputVisitorOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyProjectOutputVisitor"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public CopyProjectOutputVisitor(CopyProjectOutputVisitorOptions options)
        {
            _options=options;
        }

        /// <summary>
        /// Get a value indicating whether or not the visitor should execute, based on the core options specified
        /// </summary>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>
        ///   <c>true</c> if the visitor shall be called, <c>false</c> otherwise
        /// </returns>
        public override bool ShallExecute(IMsBuilderificCoreOptions coreOptions)
        {
            return coreOptions == null || !string.IsNullOrEmpty(_options.CopyOutputTo);
        }

        /// <summary>
        /// Called when it is time to add build specific instructions for a <paramref name="project"/> that contains WCF services
        /// </summary>
        /// <param name="project">The project that will be visited for build for the specific Service target</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>
        /// The msbuild syntax to add to the build file
        /// </returns>
        public override string VisitServiceTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            return VisitBuildAllTypeTarget(project, coreOptions);
        }

        /// <summary>
        /// Call when it is time to add build information to the build file for the <paramref name="project"/>, for each possible targets
        /// </summary>
        /// <param name="project">The project that is being visited for build</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>
        /// The msbuild syntax to add to the build file
        /// </returns>
        public override string VisitBuildAllTypeTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            var buildBuilder = new StringBuilder();

            var extensions = new List<String>();
            
            if(_options.CopyConfig)
                extensions.Add("config");

            if(_options.CopyXml)
                extensions.Add("xml");

            if (_options.CopyRessources)
            {
                extensions.Add("ressources");
                extensions.Add("resx");
            }
            
            if (project.OutputType == ProjectOutputType.Exe.ToString() || project.OutputType == ProjectOutputType.WinExe.ToString())
            {
                if (_options.CopyExe)
                    extensions.Add("exe");
            }
            else
            {
                if(_options.CopyDll)
                    extensions.Add("dll");
            }

            if (_options.CopyPdbs)
                extensions.Add("pdb");

            foreach (var ext in extensions)
            {
                var capitalizedExt = char.ToUpper(ext[0]) + ext.Substring(1);
                buildBuilder.AppendLine(AddCopyInformation(project, coreOptions, capitalizedExt, false));
            }

            return buildBuilder.ToString();
        }

        /// <summary>
        /// Adds the copy information.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="coreOptions">The core options.</param>
        /// <param name="capitalizedExt">The capitalized ext.</param>
        /// <param name="uniqueOutputPath">if set to <c>true</c> [unique output path].</param>
        /// <returns></returns>
        private string AddCopyInformation(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions, string capitalizedExt, bool uniqueOutputPath)
        {
            var buildBuilder = new StringBuilder();
            var folder = project.GetRelativeFolderPath(coreOptions);

            if (!uniqueOutputPath)
            {
                // In a non web project, we look in the configuration folder, otherwise in the bin folder
                string temp;
                if (!project.IsWebProject)
                    temp = string.Format("{0}\\bin\\$(Configuration)\\{1}.{2}", folder, project.AssemblyName, capitalizedExt.ToLower());
                else
                    temp = string.Format("{0}\\bin\\{1}.{2}", folder, project.AssemblyName, capitalizedExt.ToLower());

                buildBuilder.AppendFormat("		<Copy SourceFiles=\"{0}\" DestinationFolder=\"$(DestinationFolder)\" Condition=\"Exists('{0}') AND $(CopyEnabled) AND $(Copy{1})\" ContinueOnError=\"$(ContinueOnError)\" OverwriteReadOnlyFiles=\"True\" SkipUnchangedFiles=\"True\" />", temp, capitalizedExt);
                buildBuilder.AppendLine();

                if (_options.CopyCodeContracts)
                {
                    if (!project.IsWebProject)
                        temp = string.Format("{0}\\bin\\$(Configuration)\\CodeContracts\\{1}.Contracts.{2}", folder, project.AssemblyName, capitalizedExt.ToLower());
                    else
                        temp = string.Format("{0}\\bin\\CodeContracts\\{1}.Contracts.{2}", folder, project.AssemblyName, capitalizedExt.ToLower());

                    buildBuilder.AppendFormat("		<Copy SourceFiles=\"{0}\" DestinationFolder=\"$(DestinationFolder)\\CodeContracts\" Condition=\"Exists('{0}') AND $(CopyEnabled) AND $(Copy{1}) AND $(CopyCodeContracts)\" ContinueOnError=\"$(ContinueOnError)\" OverwriteReadOnlyFiles=\"True\" SkipUnchangedFiles=\"True\" />", temp, capitalizedExt);
                }

                buildBuilder.AppendLine();
            }

            return buildBuilder.ToString();
        }

        /// <summary>
        /// Allow the visitors to add properties in the main property group of the MsBuild file
        /// </summary>
        /// <returns>
        /// A dictionary of property name/value to add the MsBuild property group section
        /// </returns>
        public override IDictionary<string, string> GetVisitorProperties()
        {
            var properties = new Dictionary<string, string>
                                 {
                                     {"CopyEnabled", _options.CopyEnabled.ToString(CultureInfo.InvariantCulture)},
                                     {"CopyXml", _options.CopyXml.ToString(CultureInfo.InvariantCulture)},
                                     {"CopyPdb", _options.CopyPdbs.ToString(CultureInfo.InvariantCulture)},
                                     {"CopyExe", _options.CopyExe.ToString(CultureInfo.InvariantCulture)},
                                     {"CopyConfig", _options.CopyConfig.ToString(CultureInfo.InvariantCulture)},
                                     {"CopyDll", _options.CopyDll.ToString(CultureInfo.InvariantCulture)},
                                     {"CopyCodeContracts", _options.CopyCodeContracts.ToString(CultureInfo.InvariantCulture)},
                                     {"CopyRessources", _options.CopyRessources.ToString(CultureInfo.InvariantCulture)},
                                     {"DestinationFolder",_options.CopyOutputTo}
                                 };

            return properties;
        }
    }
}
