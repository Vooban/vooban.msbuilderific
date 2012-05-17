using System.Text;
using MsBuilderific.Extensions;

namespace MsBuilderific.Visitors.Build
{
    public class CopyMsDeployPackagesVisitor : BuildOrderVisitor
    {
        public override bool ShallExecute(IMsBuilderificOptions options)
        {
            return options == null || options.GeneratePackagesOnBuild;
        }

        public override string VisitServiceTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            return VisitBuildWebProjectTarget(project, options);
        }

        public override string VisitBuildWebProjectTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            var buildBuilder = new StringBuilder();

            options.Transforms.ForEach(t => buildBuilder.AppendLine(AddCopyPackagesInformation(project, options, false, t)));
            
            buildBuilder.AppendLine(AddCopyPackagesInformation(project, options, false));

            return buildBuilder.ToString();
        }

        public override string VisitBuildExeProjectTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            return VisitBuildWebProjectTarget(project, options);
        }

        private string AddCopyPackagesInformation(VisualStudioProject project, IMsBuilderificOptions options, bool uniqueOutputPath, string configuration = "$(Configuration)")
        {
            var buildBuilder = new StringBuilder();
            var folder = project.GetRelativeFolderPath(options);
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
