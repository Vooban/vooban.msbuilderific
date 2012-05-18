using System;
using System.Collections.Generic;

namespace MsBuilderific.Contracts
{
    public interface IMsBuilderificCoreOptions : IMsBuilderificOptions
    {
        /// <summary>
        /// Gets or sets the build output path
        /// </summary>
        String MsBuildFileOuputPath { get;  }

        /// <summary>
        /// Gets or sets the list of file/folder exclusions that will be excluded from the dependency detection and build file generation process
        /// </summary>
        List<String> ProjectDetectionExclusionPatterns { get;  }

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
        String MsBuildOutputFilename { get;  }

        /// <summary>
        /// Gets or sets the directory where projects output (.dll, .exe, .config, .pdb, .xml) will be copied, or nothing to ingnore this
        /// </summary>
        String CopyOutputTo { get;  }

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
    }
}
