using System.Collections.Generic;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Contracts
{
    public interface IMsBuildFileCore
    {
        /// <summary>
        /// Accepts a new visitor into the msbuild file's generation core 
        /// </summary>
        /// <param name="newVisitor">The new visitor which will be called in the file generation process</param>
        void AcceptVisitor(IBuildOrderVisitor newVisitor);

        /// <summary>
        /// Removes a visitor from the generation process
        /// </summary>
        /// <param name="kickedVisitor">The visitor to remove from the generation process</param>
        void KickOutVisitor(IBuildOrderVisitor kickedVisitor);

        /// <summary>
        /// Generates a build script from the projects' dependency order
        /// </summary>
        /// <param name="dependencyOrder">
        /// The order in which the MsBuildScript shall be generated
        /// </param>
        /// <param name="options">The options that will be passed to the visitors</param>
        void WriteBuildScript(List<VisualStudioProject> dependencyOrder, IMsBuilderificOptions options);
    }
}