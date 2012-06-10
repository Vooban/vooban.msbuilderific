using System;
using CommandLine;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class MsBuildProjectVisitorOptions :IVisitorOptions
    {
        /// <summary>
        /// Gets or sets the build output path
        /// </summary>
        [Option("m", "outputPath", HelpText = "The project's build output path", Required = false)]
        public String MsBuildFileOuputPath { get; set; }
    }
}
