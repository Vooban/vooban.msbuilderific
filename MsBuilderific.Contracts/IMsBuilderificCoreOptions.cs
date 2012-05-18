using System;
using System.Collections.Generic;

namespace MsBuilderific.Contracts
{
    public interface IMsBuilderificCoreOptions : IMsBuilderificOptions
    {
        /// <summary>
        /// Gets or sets the build output path
        /// </summary>
        String OutputPath { get;  }

        /// <summary>
        /// Gets or sets the list of configuration that will be used to perform web.config transformations when building in release mode
        /// </summary>
        List<String> Transforms { get; }

        /// <summary>
        /// Gets or sets the list of file/folder exclusions that will be excluded from the dependency detection and build file generation process
        /// </summary>
        List<String> ExclusionPatterns { get;  }

        /// <summary>
        /// Gets or sets the root folder from which the tool will scan for projects
        /// </summary>
        String RootFolder { get;  }

        /// <summary>
        /// Gets or sets a filename where the dependecy graph of your projetcs will be saved or nothing if you do not which to generate the .graphml file
        /// </summary>
        String GraphFilename { get;  }

        /// <summary>
        /// Gets or sets the path used by MsBuilderific to create relative build/copy paths
        /// </summary>
        String RelativeToPath { get;  }

        /// <summary>
        /// Gets or sets the build output file
        /// </summary>
        String OutputFile { get;  }

        /// <summary>
        /// Gets or sets the directory where projects output (.dll, .exe, .config, .pdb, .xml) will be copied, or nothing to ingnore this
        /// </summary>
        String CopyOutputTo { get;  }

        /// <summary>
        /// Gets or sets a value indicating if we generate msbuild packages 
        /// </summary>
        bool GeneratePackagesOnBuild { get;  }

        /// <summary>
        /// Gets or sets a value indicating if we generate specific build/rebuild/clean targets for web projects
        /// </summary>
        bool GenerateSpecificTargetForWebProject { get;  }

        /// <summary>
        /// Gets or sets a value indicating if MsBuilderific should look for .csproj
        /// </summary>
        bool CSharpSupport { get;  }

        /// <summary>
        /// Gets or sets a value indicating if MsBuilderific should look for .vbproj
        /// </summary>
        bool VbNetSupport { get;}

        /// <summary>
        /// Gets or sets a value indicating if copy pdb to the output location
        /// </summary>
        bool CopyPdbs { get;}

        /// <summary>
        /// Gets or sets a value indicating if we add MsTest task in the resulting msbuild file
        /// </summary>
        bool GenerateMsTestTask { get; }

        /// <summary>
        /// Gets or sets the regex used to discover mstests assemblies
        /// </summary>
        String TestDiscoveryPattern { get; }
    }
}
