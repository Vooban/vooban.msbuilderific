using System.Text;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Extensions;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class CopyRessourcesVisitor : BuildOrderVisitor
    {
        private readonly IVisualStudioProjectRessourceFinder _ressourceFinder;

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyRessourcesVisitor"/> class.
        /// </summary>
        /// <param name="ressourceFinder">The ressource finder.</param>
        public CopyRessourcesVisitor(IVisualStudioProjectRessourceFinder ressourceFinder)
        {
            _ressourceFinder = ressourceFinder;
        }

        /// <summary>
        /// Get a value indicating whether or not the visitor should execute, based on the core options specified
        /// </summary>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>
        ///   <c>true</c> if the visitor shall be called, <c>false</c> otherwise
        /// </returns>
        public override bool ShallExecute(IMsBuilderificCoreOptions coreOptions)
        {
            return coreOptions == null || !string.IsNullOrEmpty(coreOptions.CopyOutputTo);
        }

        /// <summary>
        /// Called when it is time to add build specific instructions for a <paramref name="project"/> that contains WCF services
        /// </summary>
        /// <param name="project">The project that will be visited for build for the specific Service target</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>
        /// The msbuild syntax to add to the build file
        /// </returns>
        public override string VisitServiceTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            return VisitBuildAllTypeTarget(project, coreOptions);
        }

        /// <summary>
        /// Call when it is time to add build information to the build file for the <paramref name="project"/>, for each possible targets
        /// </summary>
        /// <param name="project">The project that is being visited for build</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>
        /// The msbuild syntax to add to the build file
        /// </returns>
        public override string VisitBuildAllTypeTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            var buildBuilder = new StringBuilder();

            buildBuilder.AppendLine(AddCopyRessourcesInformation(project, coreOptions, false));

            return buildBuilder.ToString();
        }

        /// <summary>
        /// Adds the copy ressources information.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="coreOptions">The core options.</param>
        /// <param name="uniqueOutputPath">if set to <c>true</c> [unique output path].</param>
        /// <returns></returns>
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
