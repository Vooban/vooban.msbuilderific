using System.Text;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Extensions;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class DeleteBinAfterPackagingVisitor : BuildOrderVisitor
    {
        public override string PostVisitServiceTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            return PostVisitBuildTarget(project, coreOptions);
        }

        public override string PostVisitBuildTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            var deleteBinBuilder = new StringBuilder();

            var folder = project.GetRelativeFolderPath(coreOptions);

            if (!project.IsWebProject)
                deleteBinBuilder.AppendLine(string.Format("		<RemoveDir Directories=\"{0}\\bin\\$(Configuration)\" Condition=\"$(GenerateMsDeployPackages)\" />", folder));
            else
                deleteBinBuilder.AppendLine(string.Format("		<RemoveDir Directories=\"{0}\\bin\" Condition=\"$(GenerateMsDeployPackages)\" />", folder));

            return deleteBinBuilder.ToString();
        }
    }
}
