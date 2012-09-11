using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using MsBuilderific.Contracts;

namespace MsBuilderific.Core
{
    /// <summary>
    /// Command line switches which can be used by the program
    /// </summary>
    internal class MsBuilderificCoreOptions : CommandLineOptionsBase, IMsBuilderificCoreOptions
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the list of file/folder exclusions that will be excluded from the dependency detection and build file generation process
        /// </summary>
        [OptionList("e", "exclusions", HelpText = "The list of excluded patterns, separated by ;", Required = false, Separator = ';')]
        public List<string> ProjectDetectionExclusionPatterns { get; set; }

        /// <summary>
        /// Gets or sets the root folder from which the tool will scan for projects
        /// </summary>
        [Option("r", "root", HelpText = "The path to your source root, where the tool will start scanning", Required = true)]
        public string RootFolder { get; set; }

        /// <summary>
        /// Gets or sets a filename where the dependecy graph of your projetcs will be saved or nothing if you do not which to generate the .graphml file
        /// </summary>
        [Option("g", "graph", DefaultValue = "projectDependency.graphml", HelpText = "The graph filename where the dependency graph will be saved", Required = false)]
        public string GraphFilename { get; set; }

        /// <summary>
        /// Gets or sets the path used by MsBuilderific to create relative build/copy paths
        /// </summary>
        [Option("p", "relativetopath", HelpText = "The build script shall be made relative to this path", Required = false)]
        public string RelativeToPath { get; set; }

        /// <summary>
        /// Gets or sets the build output file
        /// </summary>
        [Option("o", "outputfile", HelpText = "The build script output file", Required = true)]
        public string MsBuildOutputFilename { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if we generate specific build/rebuild/clean targets for web projects
        /// </summary>
        [Option("s", "servicespecifictarget", DefaultValue = false, HelpText = "True to generate specific build target for web projects", Required = false)]
        public bool GenerateSpecificTargetForWebProject { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if MsBuilderific should look for .csproj
        /// </summary>
        [Option("c", "csharp", DefaultValue = true, HelpText = "True to support CSharp projects, false otherwise ", Required = false)]
        public bool CSharpSupport { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if MsBuilderific should look for .vbproj
        /// </summary>
        [Option("v", "vbnet", DefaultValue = true, HelpText = "True to support Vb.NET projects, false otherwise ", Required = false)]
        public bool VbNetSupport { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Display helps to the end-user
        /// </summary>
        /// <returns>
        /// The program help message
        /// </returns>
        [HelpOption("?", "help", HelpText = "Display MsBuilderific help")]
        public String GetUsage()
        {
            var help = new HelpText(new HeadingInfo("MsBuilderific", "1.0.1").ToString()){
                MaximumDisplayWidth = Console.WindowWidth, 
                Copyright = new CopyrightInfo("Vooban Inc.", 2011)
            };

            help.AddPreOptionsLine("Usage : MsBuilderific.Console -r C:\\Sources -e C:\\Sources\\test.csproj;C:\\Sources\\Obsolete;");
            help.AddPreOptionsLine("");
            help.AddOptions(this);
            help.AddPostOptionsLine("\r\n"); 

            return help;
        }        

        #endregion
    }
}
