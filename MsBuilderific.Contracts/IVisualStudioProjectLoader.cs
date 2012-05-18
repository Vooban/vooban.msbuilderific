namespace MsBuilderific.Contracts
{
    public interface IVisualStudioProjectLoader
    {
        /// <summary>
        /// Parses the visual studio project and return the resulting
        /// </summary>
        /// <returns>
        /// An instance of the class <see cref="VisualStudioProject"/> representing the project
        /// </returns>
        VisualStudioProject Parse(string visualStudioProjectPath);

        /// <summary>
        /// Gets the Visual Studio Version support provider by this project loader
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Parses the visual studio project and return the resulting
        /// </summary>
        /// <param name="visualStudioProjectPath">Path to the Visual Studio Project to parse</param>
        /// <returns>
        /// <c>true</c> if the parser was able to load this Visual Studio Project, <c>false</c> otherwise
        /// </returns>
        bool CanParse(string visualStudioProjectPath);

        /// <summary>
        /// Parses the visual studio project and return the resulting
        /// </summary>
        /// <param name="visualStudioProjectPath">Path to the Visual Studio Project to parse</param>
        /// <param name="result">The parsed project if supported, null otherwise</param>
        /// <returns>
        /// <c>true</c> if the parser was able to load this Visual Studio Project, <c>false</c> otherwise
        /// </returns>
        bool TryParse(string visualStudioProjectPath, out VisualStudioProject result);

    }
}