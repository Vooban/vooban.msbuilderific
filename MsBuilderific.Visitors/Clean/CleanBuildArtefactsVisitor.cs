using System;
using System.IO;
using System.Text;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Extensions;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Clean
{
    public class CleanBuildArtefactsVisitor : BuildOrderVisitor
    {
        public override string VisitCleanTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            var cleanBuilder = new StringBuilder();

            cleanBuilder.AppendLine(String.Format("		<CleanedFiles Include=\"\\\\?\\$(MSBuildProjectDirectory)\\{0}\\bin\\**;\\\\?\\{0}\\obj\\**;\" />", project.GetRelativeFolderPath(coreOptions)));

            if (!string.IsNullOrEmpty(coreOptions.CopyOutputTo))
                cleanBuilder.AppendLine(String.Format("		<CleanedFiles Include=\"\\\\?\\$(DestinationFolder)\\**\\{0}*\" />", Path.GetFileNameWithoutExtension(project.GetRelativeFilePath(coreOptions))));

            if(project.IsWebProject)
                cleanBuilder.AppendLine(String.Format("		<CleanedServicesFiles Include=\"\\\\?\\$(MSBuildProjectDirectory)\\{0}\\bin\\**;\\\\?\\{0}\\obj\\**;\" />", project.GetRelativeFolderPath(coreOptions)));

            return cleanBuilder.ToString();
        }
    }
}
