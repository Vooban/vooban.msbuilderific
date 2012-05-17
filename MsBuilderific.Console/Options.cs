using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using MsBuilderific.Contracts;

namespace MsBuilderific.Console
{
    /// <summary>
    /// Command line switches which can be used by the program
    /// </summary>
    internal class Options : IMsBuilderificOptions
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the build output path
        /// </summary>
        [Option("op", "outputPath", HelpText = "The project's build output path", Required = false)]
        public String OutputPath { get; set; }

        /// <summary>
        /// Gets or sets the list of file/folder exclusions that will be excluded from the dependency detection and build file generation process
        /// </summary>
        [OptionList("e", "exclusions", HelpText = "The list of excluded patterns, separated by ;", Required = false, Separator = ';')]
        public List<string> ExclusionPatterns { get; set; }

        /// <summary>
        /// Gets or sets the root folder from which the tool will scan for projects
        /// </summary>
        [Option("r", "root", HelpText = "The path to your source root, where the tool will start scanning", Required = true)]
        public string RootFolder { get; set; }

        /// <summary>
        /// Gets or sets a filename where the dependecy graph of your projetcs will be saved or nothing if you do not which to generate the .graphml file
        /// </summary>
        [Option("g", "graph", HelpText = "The graph filename where the dependency graph will be saved", Required = false)]
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
        public string OutputFile { get; set; }

        /// <summary>
        /// Gets or sets the directory where projects output (.dll, .exe, .config, .pdb, .xml) will be copied, or nothing to ingnore this
        /// </summary>
        [Option("x", "copyoutputto", HelpText = "The path where the build result will be copied", Required = false)]
        public string CopyOutputTo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if we generate msbuild packages 
        /// </summary>
        [Option("z", "generatePackages", HelpText = "True to generate msbuild packages unpon build", Required = false)]
        public bool GeneratePackagesOnBuild { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if we generate specific build/rebuild/clean targets for web projects
        /// </summary>
        [Option("s", "servicespecifictarget", HelpText = "True to generate specific build target for web projects", Required = false)]
        public bool ServiceSpecificTarget { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if all the projects are builds in the same folder 
        /// </summary>
        [Option("u", "supportsuniqueoutputpath", HelpText = "True to indicate that all the projects builds in the same folder", Required = false)]
        public bool SupportsUniqueOutputPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if MsBuilderific should look for .csproj
        /// </summary>
        [Option("c", "csharp", HelpText = "True to support CSharp projects, false otherwise ", Required = false)]
        public bool CSharpSupport { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if MsBuilderific should look for .vbproj
        /// </summary>
        [Option("v", "vbnet", HelpText = "True to support Vb.NET projects, false otherwise ", Required = false)]
        public bool VbNetSupport { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if copy pdb to the output location
        /// </summary>
        [Option("d", "copyPdbs", HelpText = "True to copy pdb to the output location, false otherwise", Required = false)]
        public bool CopyPdbs { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if we add MsTest task in the resulting msbuild file
        /// </summary>
        [Option("t", "generateMsTests", HelpText = "True to generate mstest invokation, false otherwise", Required = false)]
        public bool GenerateMsTestTask { get; set; }

        /// <summary>
        /// Gets or sets the regex used to discover mstests assemblies
        /// </summary>
        [Option("k", "testDiscoveryPattern", HelpText = "The regex used to discover mstests assemblies", Required = false)]
        public String TestDiscoveryPattern { get; set; }

        /// <summary>
        /// Gets or sets the list of configuration that will be used to perform web.config transformations when building in release mode
        /// </summary>
        [OptionList("w", "transforms", HelpText = "The list of transforms that must be applied to web.config files", Required = false, Separator = ';')]
        public List<String> Transforms { get; set; }

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
                MaximumDisplayWidth = System.Console.WindowWidth, 
                Copyright = new CopyrightInfo("Vooban Inc.", 2011)
            };

            help.AddPreOptionsLine("Usage : MsBuilderific.Console -r C:\\Sources -e C:\\Sources\\test.csproj;C:\\Sources\\Obsolete;");
            help.AddOptions(this);

            return help;
        }

        #endregion
    }
}
