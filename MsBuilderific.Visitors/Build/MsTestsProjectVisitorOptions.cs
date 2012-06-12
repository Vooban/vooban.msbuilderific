using System;
using CommandLine;
using CommandLine.Text;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class MsTestsProjectVisitorOptions : CommandLineOptionsBase, IVisitorOptions
    {
        /// <summary>
        /// Gets or sets a value indicating if we add MsTest task in the resulting msbuild file
        /// </summary>
        [Option(null, "mstestGenerate", HelpText = "True to generate mstest invokation, false otherwise", Required = false, DefaultValue = false)]
        public bool GenerateMsTestTask { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if we add MsTest task in the resulting msbuild file
        /// </summary>
        [Option(null, "mstestTeamCityMessage", HelpText = "True to generate team city transaction message to hook up test, false otherwise", Required = false, DefaultValue = false)]
        public bool GenerateTeamCityTransactionMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if we add MsTest task in the resulting msbuild file
        /// </summary>
        [Option(null, "mstestGlobalTestSettings", HelpText = "The global MsTest tests settings to use, otherwise null", Required = false)]
        public string GlobalTestSettings { get; set; }

        /// <summary>
        /// Gets or sets the regex used to discover mstests assemblies
        /// </summary>
        [Option(null, "mstestDiscoveryPattern", HelpText = "The regex used to discover mstests assemblies", Required = false, DefaultValue = ".+Tests")]
        public String TestDiscoveryPattern { get; set; }

        #region Public Methods

        [HelpOption("?", "help", HelpText = "Display MsBuilderific help")]
        public String GetUsage()
        {
            var help = new HelpText(new HeadingInfo("MsTest Visitor", null).ToString())
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
