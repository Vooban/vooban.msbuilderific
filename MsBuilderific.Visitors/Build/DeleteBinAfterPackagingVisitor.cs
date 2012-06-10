using System.Text;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Extensions;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class DeleteBinAfterPackagingVisitor : BuildOrderVisitor
    {
        /// <summary>
        /// Called just after adding specific instructions for a <paramref name="project"/> that contains WCF services
        /// </summary>
        /// <param name="project">The project that will be visited for build for the specific Service target</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>
        /// The msbuild syntax to add to the build file
        /// </returns>
        public override string PostVisitServiceTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            return PostVisitBuildTarget(project, coreOptions);
        }

        /// <summary>
        /// Called just after the build operation on the <paramref name="project"/>
        /// </summary>
        /// <param name="project">The project that was visited for build</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>
        /// The msbuild syntax to add to the build file
        /// </returns>
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
