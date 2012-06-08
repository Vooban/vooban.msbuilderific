using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class CopyProjectOutputVisitorOptions :IVisitorOptions
    {
        /// <summary>
        /// Gets or sets a value indicating if copy pdb to the output location
        /// </summary>
        [Option("d", "copyPdbs", HelpText = "True to copy pdb to the output location, false otherwise", Required = false)]
        public bool CopyPdbs { get; set; }
    }

}
