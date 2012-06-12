using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class MsDeployProjectVisitorOptions : CommandLineOptionsBase, IVisitorOptions
    {
        /// <summary>
        /// Gets or sets a value indicating if we generate msbuild packages 
        /// </summary>
        [Option(null, "msdeployGeneratePackage", HelpText = "True to generate msbuild packages unpon build", Required = false, DefaultValue = false)]
        public bool GeneratePackagesOnBuild { get; set; }

        /// <summary>
        /// Gets or sets the list of configuration that will be used to perform web.config transformations when building in release mode
        /// </summary>
        [OptionList(null, "msdeployTransforms", HelpText = "The list of transforms that must be applied to web.config files", Required = false, Separator = ';')]
        public List<String> Transforms { get; set; }

        #region Public Methods

        [HelpOption("?", "help", HelpText = "Display MsBuilderific help")]
        public String GetUsage()
        {
            var help = new HelpText(new HeadingInfo("MsDeploy Visitor", null).ToString())
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
