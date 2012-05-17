using System.Text;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Extensions;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class DeleteBinAfterPackagingVisitor : BuildOrderVisitor
    {
        public override string PostVisitServiceTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            return PostVisitBuildTarget(project, options);
        }

        public override string PostVisitBuildTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            var deleteBinBuilder = new StringBuilder();

            var folder = project.GetRelativeFolderPath(options);

            if (!project.IsWebProject)
                deleteBinBuilder.AppendLine(string.Format("		<RemoveDir Directories=\"{0}\\bin\\$(Configuration)\" Condition=\"$(GenerateMsDeployPackages)\" />", folder));
            else
                deleteBinBuilder.AppendLine(string.Format("		<RemoveDir Directories=\"{0}\\bin\" Condition=\"$(GenerateMsDeployPackages)\" />", folder));

            return deleteBinBuilder.ToString();
        }
    }
}
