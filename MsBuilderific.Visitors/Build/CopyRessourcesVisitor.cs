using System.Text;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Extensions.Extensions;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class CopyRessourcesVisitor : BuildOrderVisitor
    {
        private readonly IProjectRessourceFinder _ressourceFinder;

        public CopyRessourcesVisitor(IProjectRessourceFinder ressourceFinder)
        {
            _ressourceFinder = ressourceFinder;
        }

        public override bool ShallExecute(IMsBuilderificOptions options)
        {
            return options == null || !string.IsNullOrEmpty(options.CopyOutputTo);
        }

        public override string VisitServiceTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            return VisitBuildAllTypeTarget(project, options);
        }

        public override string VisitBuildAllTypeTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            var buildBuilder = new StringBuilder();

            buildBuilder.AppendLine(AddCopyRessourcesInformation(project, options, false));

            return buildBuilder.ToString();
        }

        private string AddCopyRessourcesInformation(VisualStudioProject project, IMsBuilderificOptions options, bool uniqueOutputPath)
        {
            var buildBuilder = new StringBuilder();
            var folder = project.GetRelativeFolderPath(options);

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
