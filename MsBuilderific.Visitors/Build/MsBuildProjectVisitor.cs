using System;
using System.Collections.Generic;
using System.Text;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class MsBuildProjectVisitor : BuildOrderVisitor
    {
        private readonly MsBuildProjectVisitorOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsBuildProjectVisitor"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public MsBuildProjectVisitor(MsBuildProjectVisitorOptions options)
        {
            _options = options;
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
            // Same logic for exe and library projects, defaults to same implementation
            return VisitBuildLibraryProjectTarget(project, coreOptions);
        }

        /// <summary>
        /// Call for project library when it is time to add build information to the build file for the <paramref name="project"/>, for each possible targets
        /// </summary>
        /// <param name="project">The project that is being visited for build</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>
        /// The msbuild syntax to add to the build file
        /// </returns>
        public override string VisitBuildLibraryProjectTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            var buildCommand = new StringBuilder();

            //ExecuteTests
            string condition = null;
            if(project.IsTestProject)
                condition = "Condition=\"$(BuildTestProjects)\"";
            
            string customOutputPath = null;
            if (!string.IsNullOrEmpty(_options.MsBuildFileOuputPath))
                customOutputPath = "OutputPath=$(OutputPath)";

            buildCommand.AppendLine(String.Format("		<MSBuild Properties=\"Configuration=$(Configuration);Platform=$(PlatformConfig);{0}\" Projects=\"{1}\" Targets=\"$(Action)\" StopOnFirstFailure=\"True\" ContinueOnError=\"$(ContinueOnError)\" UnloadProjectsOnCompletion=\"$(UnloadProjectsOnCompletion)\" UseResultsCache=\"$(UseResultsCache)\" {2} >", customOutputPath, project.GetRelativeFilePath(coreOptions), condition));
            buildCommand.AppendLine(string.Format("			<Output TaskParameter=\"TargetOutputs\" ItemName=\"ProjectArtefacts\"/>"));
            buildCommand.AppendLine(string.Format("		</MSBuild>"));

            return buildCommand.ToString();
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
            var buildCommand = new StringBuilder();

            buildCommand.AppendLine(String.Format("		<MSBuild Properties=\"Configuration=$(Configuration);Platform=$(PlatformConfig);DefineConstants=THREAD_STATIC_CONTAINER\" Projects=\"{0}\" Targets=\"Build\" StopOnFirstFailure=\"True\" ContinueOnError=\"$(ContinueOnError)\" UnloadProjectsOnCompletion=\"$(UnloadProjectsOnCompletion)\" UseResultsCache=\"$(UseResultsCache)\" >", project.GetRelativeFilePath(coreOptions)));
            buildCommand.AppendLine(string.Format("			<Output TaskParameter=\"TargetOutputs\" ItemName=\"ProjectArtefacts\"/>"));
            buildCommand.AppendLine(string.Format("		</MSBuild>"));

            return buildCommand.ToString();
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
            // Building a web service and a web project is the same
            return VisitBuildWebProjectTarget(project, coreOptions);
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
                                    {"OutputPath", _options.MsBuildFileOuputPath}, 
                                    {"UseResultsCache", "True"}, 
                                    {"Action", "Build"}, 
                                    {"UnloadProjectsOnCompletion", "True"}, 
                                    {"BuildTestProjects", "True"}
                                 };

            return properties;
        }
    }
}