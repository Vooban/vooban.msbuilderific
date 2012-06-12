using System;
using CommandLine;
using CommandLine.Text;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class MsBuildProjectVisitorOptions : CommandLineOptionsBase, IVisitorOptions
    {
        /// <summary>
        /// Gets or sets the build output path
        /// </summary>
        [Option("m", "msbuildOutputPath", HelpText = "The project's build output path", Required = false)]
        public String MsBuildFileOuputPath { get; set; }

        #region Public Methods

        [HelpOption("?", "help", HelpText = "Display MsBuilderific help")]
        public String GetUsage()
        {
            var help = new HelpText(new HeadingInfo("MsBuild Visitor", null).ToString())
            {
                MaximumDisplayWidth = Console.WindowWidth,
                Copyright = new CopyrightInfo("Vooban Inc.", 2011)
            };

            help.AddPreOptionsLine("");
            help.AddOptions(this);
            help.AddPostOptionsLine("\r\n"); 

            return help;
        }

        #endregion
    }
}
