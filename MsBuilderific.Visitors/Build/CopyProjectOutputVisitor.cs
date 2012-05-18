using System;
using System.Collections.Generic;
using System.Text;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Extensions;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class CopyProjectOutputVisitor : BuildOrderVisitor
    {
        public override bool ShallExecute(IMsBuilderificCoreOptions coreOptions)
        {
            return coreOptions == null || !string.IsNullOrEmpty(coreOptions.CopyOutputTo);
        }

        public override string VisitServiceTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            return VisitBuildAllTypeTarget(project, coreOptions);
        }

        public override string VisitBuildAllTypeTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            var buildBuilder = new StringBuilder();

            var extensions = new List<String> { "xml", "config" };
            if (project.OutputType == ProjectOutputType.Exe.ToString() || project.OutputType == ProjectOutputType.WinExe.ToString())
                extensions.Add("exe");
            else
                extensions.Add("dll");

            if (coreOptions.CopyPdbs)
                extensions.Add("pdb");

            foreach (var ext in extensions)
            {
                var capitalizedExt = char.ToUpper(ext[0]) + ext.Substring(1);
                buildBuilder.AppendLine(AddCopyInformation(project, coreOptions, capitalizedExt, false));
            }

            return buildBuilder.ToString();
        }

        private string AddCopyInformation(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions, string capitalizedExt, bool uniqueOutputPath)
        {
            var buildBuilder = new StringBuilder();
            var folder = project.GetRelativeFolderPath(coreOptions);

            if (!uniqueOutputPath)
            {
                // In a non web project, we look in the configuration folder, otherwise in the bin folder
                string temp;
                if (!project.IsWebProject)
                    temp = string.Format("{0}\\bin\\$(Configuration)\\{1}.{2}", folder, project.AssemblyName, capitalizedExt.ToLower());
                else
                    temp = string.Format("{0}\\bin\\{1}.{2}", folder, project.AssemblyName, capitalizedExt.ToLower());

                buildBuilder.AppendFormat("		<Copy SourceFiles=\"{0}\" DestinationFolder=\"$(DestinationFolder)\" Condition=\"Exists('{0}') AND $(CopyEnabled) AND $(Copy{1})\" ContinueOnError=\"$(ContinueOnError)\" OverwriteReadOnlyFiles=\"True\" SkipUnchangedFiles=\"True\" />", temp, capitalizedExt);
                buildBuilder.AppendLine();

                if (!project.IsWebProject)
                    temp = string.Format("{0}\\bin\\$(Configuration)\\CodeContracts\\{1}.Contracts.{2}", folder, project.AssemblyName, capitalizedExt.ToLower());
                else
                    temp = string.Format("{0}\\bin\\CodeContracts\\{1}.Contracts.{2}", folder, project.AssemblyName, capitalizedExt.ToLower());

                buildBuilder.AppendFormat("		<Copy SourceFiles=\"{0}\" DestinationFolder=\"$(DestinationFolder)\\CodeContracts\" Condition=\"Exists('{0}') AND $(CopyEnabled) AND $(Copy{1}) AND $(CopyCodeContracts)\" ContinueOnError=\"$(ContinueOnError)\" OverwriteReadOnlyFiles=\"True\" SkipUnchangedFiles=\"True\" />", temp, capitalizedExt);
                buildBuilder.AppendLine();
            }

            return buildBuilder.ToString();
        }
    }
}
