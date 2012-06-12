using CommandLine;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class CopyToOptions : CommandLineOptionsBase, IVisitorOptions
    {
        /// <summary>
        /// Gets or sets the directory where projects output (.dll, .exe, .config, .pdb, .xml) will be copied, or nothing to ingnore this
        /// </summary>
        [Option("x", "copyoutputto", HelpText = "The path where the build result will be copied", Required = false)]
        public string CopyOutputTo { get; set; }
    }
}
