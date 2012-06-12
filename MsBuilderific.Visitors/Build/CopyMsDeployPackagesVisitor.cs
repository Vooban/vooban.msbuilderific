using System.Text;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    /// <summary>
    /// Visitors that allows one to copy MsDeploy result to the output folder
    /// </summary>
    public class CopyMsDeployPackagesVisitor : BuildOrderVisitor
    {
        #region Private members

        private readonly MsDeployProjectVisitorOptions _options;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyMsDeployPackagesVisitor"/> class.
        /// </summary>
        /// <param name="options">The application core options.</param>
        public CopyMsDeployPackagesVisitor(MsDeployProjectVisitorOptions options)
        {
            _options=options;
        }

        #endregion

        #region IBuildOrderVisitor

        /// <summary>
        /// Get a value indicating whether or not the visitor should execute, based on the core options specified
        /// </summary>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>
        ///   <c>true</c> if the visitor shall be called, <c>false</c> otherwise
        /// </returns>
        public override bool ShallExecute(IMsBuilderificCoreOptions coreOptions)
        {
            return coreOptions == null || _options.GeneratePackagesOnBuild;
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
            return VisitBuildWebProjectTarget(project, coreOptions);
        }

        /// <summary>
        /// Call for web project when it is time to add build information to the build file for the <paramref name="project"/>, for each possible targets
        /// </summary>
        /// <param name="project">The project that is being visited for build</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>
        /// The msbuild syntax to add to the build file
        /// </returns>
        public override string VisitBuildWebProjectTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            var buildBuilder = new StringBuilder();

            _options.Transforms.ForEach(t => buildBuilder.AppendLine(AddCopyPackagesInformation(project, coreOptions, false, t)));
            
            buildBuilder.AppendLine(AddCopyPackagesInformation(project, coreOptions, false));

            return buildBuilder.ToString();
        }

        /// <summary>
        /// Call for program projects (exe) when it is time to add build information to the build file for the <paramref name="project"/>, for each possible targets
        /// </summary>
        /// <param name="project">The project that is being visited for build</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>
        /// The msbuild syntax to add to the build file
        /// </returns>
        public override string VisitBuildExeProjectTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            return VisitBuildWebProjectTarget(project, coreOptions);
        }

        /// <summary>
        /// Adds the copy packages information.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="coreOptions">The core options.</param>
        /// <param name="uniqueOutputPath">if set to <c>true</c> [unique output path].</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        private static string AddCopyPackagesInformation(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions, bool uniqueOutputPath, string configuration = "$(Configuration)")
        {
            var buildBuilder = new StringBuilder();
            var folder = project.GetRelativeFolderPath(coreOptions);
            string temp;
            string command;

            if (!uniqueOutputPath)
            {
                temp = string.Format("{0}\\bin\\Packages\\{1}\\{2}.zip", folder, configuration, project.AssemblyName);
                command = string.Format("		<Move SourceFiles=\"{0}\" DestinationFolder=\"$(DestinationFolder)\\Packages\\{1}\" Condition=\"Exists('{0}') AND $(GenerateMsDeployPackages)\" ContinueOnError=\"$(ContinueOnError)\" />", temp, configuration);
            }
            else
            {
                temp = string.Format("(UniqueOutputPath)\\Packages\\{0}\\{1}.zip", configuration, project.AssemblyName);
                command = string.Format("		<Move SourceFiles=\"{0}\" DestinationFolder=\"$(DestinationFolder)\\Packages\\{1}\" Condition=\"Exists('{0}') AND $(GenerateMsDeployPackages)\" ContinueOnError=\"$(ContinueOnError)\" />", temp, configuration);
            }

            buildBuilder.AppendLine(command);

            return buildBuilder.ToString();
        }

        #endregion
    }
}
