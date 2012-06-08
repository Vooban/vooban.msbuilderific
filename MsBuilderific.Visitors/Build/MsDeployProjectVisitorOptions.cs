using System;
using System.Collections.Generic;
using CommandLine;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class MsDeployProjectVisitorOptions : IVisitorOptions
    {
        /// <summary>
        /// Gets or sets a value indicating if we generate msbuild packages 
        /// </summary>
        [Option("z", "generatePackages", HelpText = "True to generate msbuild packages unpon build", Required = false)]
        public bool GeneratePackagesOnBuild { get; set; }

        /// <summary>
        /// Gets or sets the list of configuration that will be used to perform web.config transformations when building in release mode
        /// </summary>
        [OptionList("w", "transforms", HelpText = "The list of transforms that must be applied to web.config files", Required = false, Separator = ';')]
        public List<String> Transforms { get; set; }
    }
}
