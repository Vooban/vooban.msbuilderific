using System;
using System.Text;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Extensions;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class MsBuildProjectVisitor : BuildOrderVisitor
    {
        public override string VisitBuildExeProjectTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            // Same logic for exe and library projects, defaults to same implementation
            return VisitBuildLibraryProjectTarget(project, options);
        }

        public override string VisitBuildLibraryProjectTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            var buildCommand = new StringBuilder();

            //ExecuteTests
            string condition = null;
            if(project.IsTestProject)
                condition = "Condition=\"$(BuildTestProjects)\"";
            
            string customOutputPath = null;
            if (!string.IsNullOrEmpty(options.OutputPath))
                customOutputPath = "OutputPath=$(OutputPath)";

            buildCommand.AppendLine(String.Format("		<MSBuild Properties=\"Configuration=$(Configuration);Platform=$(PlatformConfig);{0}\" Projects=\"{1}\" Targets=\"$(Action)\" StopOnFirstFailure=\"True\" ContinueOnError=\"$(ContinueOnError)\" UnloadProjectsOnCompletion=\"$(UnloadProjectsOnCompletion)\" UseResultsCache=\"$(UseResultsCache)\" {2} >", customOutputPath, project.GetRelativeFilePath(options), condition));
            buildCommand.AppendLine(string.Format("			<Output TaskParameter=\"TargetOutputs\" ItemName=\"ProjectArtefacts\"/>"));
            buildCommand.AppendLine(string.Format("		</MSBuild>"));

            return buildCommand.ToString();
        }

        public override string VisitBuildWebProjectTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            var buildCommand = new StringBuilder();

            buildCommand.AppendLine(String.Format("		<MSBuild Properties=\"Configuration=$(Configuration);Platform=$(PlatformConfig);DefineConstants=THREAD_STATIC_CONTAINER\" Projects=\"{0}\" Targets=\"Build\" StopOnFirstFailure=\"True\" ContinueOnError=\"$(ContinueOnError)\" UnloadProjectsOnCompletion=\"$(UnloadProjectsOnCompletion)\" UseResultsCache=\"$(UseResultsCache)\" >", project.GetRelativeFilePath(options)));
            buildCommand.AppendLine(string.Format("			<Output TaskParameter=\"TargetOutputs\" ItemName=\"ProjectArtefacts\"/>"));
            buildCommand.AppendLine(string.Format("		</MSBuild>"));

            return buildCommand.ToString();
        }

        public override string VisitServiceTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            // Building a web service and a web project is the same
            return VisitBuildWebProjectTarget(project, options);
        }
    }
}