using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using QuickGraph;
using QuickGraph.Serialization;

namespace MsBuilderific
{
    /// <summary>
    /// Finds the dependencies between the projects provided and generate the right build order
    /// </summary>
    public class ProjectDependencyFinder
    {
        #region Private Members

        private readonly List<String> _excludedPatterns = new List<String>();
        private readonly bool _supportVbproj;
        private readonly bool _supportCsproj;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDependencyFinder"/> class.
        /// </summary>
        public ProjectDependencyFinder()
        {
            _supportVbproj = false;
            _supportCsproj = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDependencyFinder"/> class.
        /// </summary>
        /// <param name="supportVbproj">if set to <c>true</c> the class will look for .vbproj while scanning the main folder.</param>
        /// <param name="supportCsproj">if set to <c>true</c> the class will look for .csproj while scanning the main folder.</param>
        public ProjectDependencyFinder(bool supportVbproj, bool supportCsproj)
        {
            _supportVbproj = supportVbproj;
            _supportCsproj = supportCsproj;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add an exclusion pattern, which will be found projects.
        /// </summary>
        /// <param name="pattern">
        /// The pattern to include
        /// </param>
        public void AddExclusionPattern(string pattern)
        {
            if (!_excludedPatterns.Contains(pattern))
                _excludedPatterns.Add(pattern);
        }

        /// <summary>
        /// Removes an exclusion pattern, which will be found projects.
        /// </summary>
        /// <param name="pattern">
        /// The pattern to include
        /// </param>
        public void RemoveExclusionPattern(string pattern)
        {
            if (_excludedPatterns.Contains(pattern))
                _excludedPatterns.Remove(pattern);
        }

        /// <summary>
        /// Build the dependency graph between the projects in the root folder and optionnaly save the graph in a GraphML file
        /// </summary>
        /// <param name="rootFolder">
        /// The root folder to search for visual studio projects
        /// </param>
        /// <returns>
        /// The list of <see cref="VisualStudioProject"/> in the correct build order
        /// </returns>
        public List<VisualStudioProject> GetDependencyOrder(string rootFolder)
        {
            return GetDependencyOrder(rootFolder , null);
        }

        /// <summary>
        /// Build the dependency graph between the projects in the root folder and optionnaly save the graph in a GraphML file
        /// </summary>
        /// <param name="rootFolder">
        /// The root folder to search for visual studio projects
        /// </param>
        /// <param name="graphFilename">
        /// The filename where the graph will be save, or nothing to disable graph persistence
        /// </param>
        /// <returns>
        /// The list of <see cref="VisualStudioProject"/> in the correct build order
        /// </returns>
        public List<VisualStudioProject> GetDependencyOrder(string rootFolder, string graphFilename)
        {
            var graph = GetDependencyGraph(rootFolder);

            if (!string.IsNullOrEmpty(graphFilename))
                PersistGraph(graph, graphFilename);

            return GetDependencyOrder(graph);
        }

        /// <summary>
        /// Build the dependency graph from the projects found in the root folder - the exlusion patterns
        /// </summary>
        /// <param name="rootFolder">
        /// The folder to search
        /// </param>
        /// <returns>
        /// A graph representing the dependencies between the projects in the root folder
        /// </returns>
        public AdjacencyGraph<VisualStudioProject, Edge<VisualStudioProject>> GetDependencyGraph(string rootFolder)
        {
            if (!Directory.Exists(rootFolder))
                throw new ArgumentException(string.Format("Le répertoire source est inexistant : {0}", rootFolder), "rootFolder");

            var graph = new AdjacencyGraph<VisualStudioProject, Edge<VisualStudioProject>>();

            var projects = Directory.GetFiles(rootFolder, "*.*proj", SearchOption.AllDirectories);
            if (_supportCsproj && _supportVbproj)
                projects = projects.Where(s => s.EndsWith(".vbproj") || s.EndsWith(".csproj")).ToArray();
            else if (_supportCsproj)
                projects = projects.Where(s => s.EndsWith(".csproj")).ToArray();
            else if (_supportVbproj)
                projects = projects.Where(s => s.EndsWith(".vbproj")).ToArray();
            else 
                projects = new string[]{};

            var projs = projects.Where(f => !_excludedPatterns.Any(f.Contains));

            foreach (var resultat in projs.Select(csproj => new ProjectLoader(csproj)).Select(loader => loader.Parse()))
            {
                if (resultat != null)
                    graph.AddVertex(resultat);
            }

            foreach (var v in graph.Vertices)
            {
                foreach (var dep in v.Dependencies)
                {
                    var dependence = dep;
                    var target = graph.Vertices.Where(x => x.AssemblyName == dependence).SingleOrDefault();

                    if (target != null)
                        graph.AddVerticesAndEdge(new Edge<VisualStudioProject>(v, target));
                }
            }

            return graph;
        }

        /// <summary>
        /// Process the graph to generate the correct project build order
        /// </summary>
        /// <param name="graph">
        /// The graph that will be navigated to find the right project build order
        /// </param>
        /// <returns>
        /// A list of projects in the correct build order
        /// </returns>
        public List<VisualStudioProject> GetDependencyOrder(AdjacencyGraph<VisualStudioProject, Edge<VisualStudioProject>> graph)
        {
            var queue = new Queue<VisualStudioProject>();

            ProcessGraph(graph, ref queue);

            return queue.ToList();
        }
       
        /// <summary>
        /// Saves the graph in the GraphML format in the specified filename
        /// </summary>
        /// <param name="graph">
        /// The graph to persist
        /// </param>
        /// <param name="filename">
        /// The filename in which the graph willb e persisted
        /// </param>
        public void PersistGraph(AdjacencyGraph<VisualStudioProject, Edge<VisualStudioProject>> graph, string filename)
        {
            using (var xwriter = XmlWriter.Create(filename))
                graph.SerializeToGraphML<VisualStudioProject, Edge<VisualStudioProject>, AdjacencyGraph<VisualStudioProject, Edge<VisualStudioProject>>>(xwriter, s => s.AssemblyName, e => String.Format("{0} depends on {1}", e.Source.AssemblyName, e.Target.AssemblyName));

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Processes the graph by looking for project without dependency, removing them from the graph, and iterating until no more
        /// vertices exists in the graph
        /// </summary>
        /// <param name="graph">
        /// The graph to navigate
        /// </param>
        /// <param name="queue">
        /// The queue used to enqueue project in the right order
        /// </param>
        /// <returns>
        /// The number of projects that were removed from the graph
        /// </returns>
        private static int ProcessGraph(AdjacencyGraph<VisualStudioProject, Edge<VisualStudioProject>> graph, ref Queue<VisualStudioProject> queue)
        {
            var removed = new List<VisualStudioProject>();

            foreach (var v in graph.Vertices.Where(v => graph.OutEdges(v).Count() == 0))
            {
                queue.Enqueue(v);
                removed.Add(v);
            }

            if (removed.Count > 0)
                removed.ForEach(v => graph.RemoveVertex(v));

            var itemProcessed = removed.Count;

            if (itemProcessed > 0)
                itemProcessed += ProcessGraph(graph, ref queue);

            return itemProcessed;
        }

        #endregion
    }
}
