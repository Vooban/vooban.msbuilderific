using System.Collections.Generic;
using MsBuilderific.Contracts;

namespace MsBuilderific
{
    public class MsBuilderificOptions : IMsBuilderificOptions
    {
        public string OutputPath { get; set; }

        /// <summary>
        /// Gets or sets the list of configuration that will be used to perform web.config transformations when building in release mode
        /// </summary>
        public List<string> Transforms
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of file/folder exclusions that will be excluded from the dependency detection and build file generation process
        /// </summary>
        public List<string> ExclusionPatterns
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the root folder from which the tool will scan for projects
        /// </summary>
        public string RootFolder
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets a filename where the dependecy graph of your projetcs will be saved or nothing if you do not which to generate the .graphml file
        /// </summary>
        public string GraphFilename
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the path used by MsBuilderific to create relative build/copy paths
        /// </summary>
        public string RelativeToPath
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the build output file
        /// </summary>
        public string OutputFile
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the directory where projects output (.dll, .exe, .config, .pdb, .xml) will be copied, or nothing to ingnore this
        /// </summary>
        public string CopyOutputTo
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets a value indicating if we generate msbuild packages 
        /// </summary>
        public bool GeneratePackagesOnBuild
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets a value indicating if we generate specific build/rebuild/clean targets for web projects
        /// </summary>
        public bool ServiceSpecificTarget
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets a value indicating if all the projects are builds in the same folder 
        /// </summary>
        public bool SupportsUniqueOutputPath
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets a value indicating if MsBuilderific should look for .csproj
        /// </summary>
        public bool CSharpSupport
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets a value indicating if MsBuilderific should look for .vbproj
        /// </summary>
        public bool VbNetSupport
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets a value indicating if copy pdb to the output location
        /// </summary>
        public bool CopyPdbs
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets a value indicating if we add MsTest task in the resulting msbuild file
        /// </summary>
        public bool GenerateMsTestTask
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the regex used to discover mstests assemblies
        /// </summary>
        public string TestDiscoveryPattern
        {
            get;
            set;
        }
    }
}
