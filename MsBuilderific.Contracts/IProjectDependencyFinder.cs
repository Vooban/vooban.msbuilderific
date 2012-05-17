using System.Collections.Generic;
using QuickGraph;

namespace MsBuilderific.Contracts
{
    public interface IProjectDependencyFinder
    {
        /// <summary>
        /// Add an exclusion pattern, which will be found projects.
        /// </summary>
        /// <param name="pattern">
        /// The pattern to include
        /// </param>
        void AddExclusionPattern(string pattern);

        /// <summary>
        /// Removes an exclusion pattern, which will be found projects.
        /// </summary>
        /// <param name="pattern">
        /// The pattern to include
        /// </param>
        void RemoveExclusionPattern(string pattern);

        /// <summary>
        /// Build the dependency graph between the projects in the root folder and optionnaly save the graph in a GraphML file
        /// </summary>
        /// <param name="options">
        /// The options used to get the dependency order of the projects
        /// </param>
        /// <returns>
        /// The list of <see cref="VisualStudioProject"/> in the correct build order
        /// </returns>
        List<VisualStudioProject> GetDependencyOrder(IMsBuilderificOptions options);

        /// <summary>
        /// Process the graph to generate the correct project build order
        /// </summary>
        /// <param name="graph">
        /// The graph that will be navigated to find the right project build order
        /// </param>
        /// <returns>
        /// A list of projects in the correct build order
        /// </returns>
        List<VisualStudioProject> GetDependencyOrderFromGraph(AdjacencyGraph<VisualStudioProject, Edge<VisualStudioProject>> graph);

        /// <summary>
        /// Build the dependency graph from the projects found in the root folder - the exlusion patterns
        /// </summary>
        /// <param name="options">
        /// The options used to get the dependency order of the projects
        /// </param>
        /// <returns>
        /// A graph representing the dependencies between the projects in the root folder
        /// </returns>
        AdjacencyGraph<VisualStudioProject, Edge<VisualStudioProject>> GetDependencyGraph(IMsBuilderificOptions options);

        /// <summary>
        /// Saves the graph in the GraphML format in the specified filename
        /// </summary>
        /// <param name="graph">
        /// The graph to persist
        /// </param>
        /// <param name="filename">
        /// The filename in which the graph willb e persisted
        /// </param>
        void PersistGraph(AdjacencyGraph<VisualStudioProject, Edge<VisualStudioProject>> graph, string filename);
    }
}
