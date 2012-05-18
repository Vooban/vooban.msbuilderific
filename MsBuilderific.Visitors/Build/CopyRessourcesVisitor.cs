using System.Text;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Extensions;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class CopyRessourcesVisitor : BuildOrderVisitor
    {
        private readonly IVisualStudioProjectRessourceFinder _ressourceFinder;

        public CopyRessourcesVisitor(IVisualStudioProjectRessourceFinder ressourceFinder)
        {
            _ressourceFinder = ressourceFinder;
        }

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

            buildBuilder.AppendLine(AddCopyRessourcesInformation(project, coreOptions, false));

            return buildBuilder.ToString();
        }

        private string AddCopyRessourcesInformation(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions, bool uniqueOutputPath)
        {
            var buildBuilder = new StringBuilder();
            var folder = project.GetRelativeFolderPath(coreOptions);

            foreach (var currentRessource in _ressourceFinder.ExtractRessourcesFromProject(project.Path))
            {
                if (!string.IsNullOrEmpty(currentRessource))
                {
                    string temp;

                    if (!uniqueOutputPath)
                    {
                        // In a non web project, we look in the configuration folder, otherwise in the bin folder
                        if (!project.IsWebProject)
                            temp = string.Format("{0}\\bin\\$(Configuration)\\{1}\\{2}.resources.dll", folder, currentRessource, project.AssemblyName);
                        else
                            temp = string.Format("{0}\\bin\\{1}\\{2}.resources.dll", folder, currentRessource, project.AssemblyName);
                    }
                    else
                        temp = string.Format("(UniqueOutputPath)\\{0}\\{1}.resources.dll", currentRessource, project.AssemblyName);

                    buildBuilder.AppendFormat("		<Copy SourceFiles=\"{0}\" DestinationFolder=\"$(DestinationFolder)\\{1}\" Condition=\"Exists('{0}') AND $(CopyEnabled) AND $(CopyRessources)\" ContinueOnError=\"$(ContinueOnError)\" OverwriteReadOnlyFiles=\"True\" SkipUnchangedFiles=\"True\" />", temp, currentRessource);
                    buildBuilder.AppendLine();
                }
            }

            return buildBuilder.ToString();
        }
    }
}
