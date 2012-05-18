using System;
using System.Collections.Generic;
using CommandLine;

namespace MsBuilderific.Visitors
{
    public class VisitorsOptions
    {
        /// <summary>
        /// Gets or sets a value indicating if we generate msbuild packages 
        /// </summary>
        [Option("z", "generatePackages", HelpText = "True to generate msbuild packages unpon build", Required = false)]
        public bool GeneratePackagesOnBuild { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if all the projects are builds in the same folder 
        /// </summary>
        [Option("u", "supportsuniqueoutputpath", HelpText = "True to indicate that all the projects builds in the same folder", Required = false)]
        public bool SupportsUniqueOutputPath { get; set; }

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
    }
}
