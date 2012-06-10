using System;
using CommandLine;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class MsTestsProjectVisitorOptions : IVisitorOptions
    {
        /// <summary>
        /// Gets or sets a value indicating if we add MsTest task in the resulting msbuild file
        /// </summary>
        [Option("t", "generateMsTests", HelpText = "True to generate mstest invokation, false otherwise", Required = false, DefaultValue = false)]
        public bool GenerateMsTestTask { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if we add MsTest task in the resulting msbuild file
        /// </summary>
        [Option("b", "generateTeamCityTestMessage", HelpText = "True to generate team city transaction message to hook up test, false otherwise", Required = false, DefaultValue = false)]
        public bool GenerateTeamCityTransactionMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if we add MsTest task in the resulting msbuild file
        /// </summary>
        [Option("w", "globalTestSettings", HelpText = "The global MsTest tests settings to use, otherwise null", Required = false)]
        public string GlobalTestSettings { get; set; }

        /// <summary>
        /// Gets or sets the regex used to discover mstests assemblies
        /// </summary>
        [Option("k", "testDiscoveryPattern", HelpText = "The regex used to discover mstests assemblies", Required = false, DefaultValue = ".+Tests")]
        public String TestDiscoveryPattern { get; set; }
    }
}
