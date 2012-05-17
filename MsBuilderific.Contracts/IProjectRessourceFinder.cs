using System;
using System.Collections.Generic;

namespace MsBuilderific.Contracts
{
    public interface IProjectRessourceFinder
    {
        /// <summary>
        /// Parses the visual studio project and return the resulting
        /// </summary>
        /// <returns>
        /// An instance of the class <see cref="VisualStudioProject"/> representing the project
        /// </returns>
        IEnumerable<String> ExtractRessourcesFromProject(string visualStudioProject);
    }
}