using System.Text;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Extensions;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class CopyMsDeployPackagesVisitor : BuildOrderVisitor
    {
        public override bool ShallExecute(IMsBuilderificCoreOptions coreOptions)
        {
            return coreOptions == null || coreOptions.GeneratePackagesOnBuild;
        }

        public override string VisitServiceTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            return VisitBuildWebProjectTarget(project, coreOptions);
        }

        public override string VisitBuildWebProjectTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            var buildBuilder = new StringBuilder();

            coreOptions.Transforms.ForEach(t => buildBuilder.AppendLine(AddCopyPackagesInformation(project, coreOptions, false, t)));
            
            buildBuilder.AppendLine(AddCopyPackagesInformation(project, coreOptions, false));

            return buildBuilder.ToString();
        }

        public override string VisitBuildExeProjectTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            return VisitBuildWebProjectTarget(project, coreOptions);
        }

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
    }
}
