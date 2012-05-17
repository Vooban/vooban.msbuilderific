using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using MsBuilderific.Contracts;
using QuickGraph;
using QuickGraph.Serialization;

namespace MsBuilderific.Core
{
    /// <summary>
    /// Finds the dependencies between the projects provided and generate the right build order
    /// </summary>
    public class ProjectDependencyFinder
    {
        #region Private Members

        private readonly List<String> _excludedPatterns = new List<String>();
   
        private readonly ProjectLoader _projectLoader;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectDependencyFinder"/> class.
        /// </summary>
        public ProjectDependencyFinder(ProjectLoader projectLoader)
        {
            _projectLoader = projectLoader;
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
        /// <param name="options">
        /// The options used to get the dependency order of the projects
        /// </param>
        /// <returns>
        /// The list of <see cref="VisualStudioProject"/> in the correct build order
        /// </returns>
        public List<VisualStudioProject> GetDependencyOrder(IMsBuilderificOptions options)
        {
            var graph = GetDependencyGraph(options);

            if (!string.IsNullOrEmpty(options.GraphFilename))
                PersistGraph(graph, options.GraphFilename);

            return GetDependencyOrder(graph);
        }

        /// <summary>
        /// Build the dependency graph from the projects found in the root folder - the exlusion patterns
        /// </summary>
        /// <param name="options">
        /// The options used to get the dependency order of the projects
        /// </param>
        /// <returns>
        /// A graph representing the dependencies between the projects in the root folder
        /// </returns>
        public AdjacencyGraph<VisualStudioProject, Edge<VisualStudioProject>> GetDependencyGraph(IMsBuilderificOptions options)
        {
            if (!Directory.Exists(options.RootFolder))
                throw new ArgumentException(string.Format("Le répertoire source est inexistant : {0}", options.RootFolder), "rootFolder");

            var graph = new AdjacencyGraph<VisualStudioProject, Edge<VisualStudioProject>>();

            var projects = Directory.GetFiles(options.RootFolder, "*.*proj", SearchOption.AllDirectories);
            if (options.CSharpSupport && options.VbNetSupport)
                projects = projects.Where(s => s.EndsWith(".vbproj") || s.EndsWith(".csproj")).ToArray();
            else if (options.CSharpSupport)
                projects = projects.Where(s => s.EndsWith(".csproj")).ToArray();
            else if (options.VbNetSupport)
                projects = projects.Where(s => s.EndsWith(".vbproj")).ToArray();
            else 
                projects = new string[]{};

            var projs = projects.Where(f => !_excludedPatterns.Any(f.Contains));

            foreach (var resultat in projs.Select(csproj => _projectLoader.Parse(csproj)))
            {
                if (resultat != null)
                    graph.AddVertex(resultat);
            }

            foreach (var v in graph.Vertices)
            {
                foreach (var dep in v.Dependencies)
                {
                    var dependence = dep;
                    var target = graph.Vertices.SingleOrDefault(x => x.AssemblyName == dependence);

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

            foreach (var v in graph.Vertices.Where(v => !graph.OutEdges(v).Any()))
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
