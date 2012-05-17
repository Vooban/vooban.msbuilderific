using System;
using System.IO;
using System.Text;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Visitors;
using MsBuilderific.Extensions;

namespace MsBuilderific.Visitors.Clean
{
    public class CleanBuildArtefactsVisitor : BuildOrderVisitor
    {
        public override string VisitCleanTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            var cleanBuilder = new StringBuilder();

            cleanBuilder.AppendLine(String.Format("		<CleanedFiles Include=\"\\\\?\\$(MSBuildProjectDirectory)\\{0}\\bin\\**;\\\\?\\{0}\\obj\\**;\" />", project.GetRelativeFolderPath(options)));

            if (!string.IsNullOrEmpty(options.CopyOutputTo))
                cleanBuilder.AppendLine(String.Format("		<CleanedFiles Include=\"\\\\?\\$(DestinationFolder)\\**\\{0}*\" />", Path.GetFileNameWithoutExtension(project.GetRelativeFilePath(options))));

            if(project.IsWebProject)
                cleanBuilder.AppendLine(String.Format("		<CleanedServicesFiles Include=\"\\\\?\\$(MSBuildProjectDirectory)\\{0}\\bin\\**;\\\\?\\{0}\\obj\\**;\" />", project.GetRelativeFolderPath(options)));

            return cleanBuilder.ToString();
        }
    }
}
