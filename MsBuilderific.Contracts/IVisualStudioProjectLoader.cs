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
    }
}