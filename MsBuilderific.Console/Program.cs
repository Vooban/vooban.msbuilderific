using System;
using CommandLine;
using MsBuilderific.Common;
using MsBuilderific.Contracts;
using MsBuilderific.Core;
using MsBuilderific.Core.VisualStudio.V2010;
using Microsoft.Practices.Unity;

namespace MsBuilderific.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigureContainer();

            var options = Injection.Engine.Resolve<IMsBuilderificCoreOptions>(); 

            if (args != null && args.Length>0)
            {
                if (!CommandLineParser.Default.ParseArguments(args, options, System.Console.Out))
                    Environment.Exit(1);
            }

            var finder = Injection.Engine.Resolve<IProjectDependencyFinder>();

            if (options.ProjectDetectionExclusionPatterns != null)
            {
                foreach (var exclusion in options.ProjectDetectionExclusionPatterns)
                    finder.AddExclusionPattern(exclusion);
            }

            var buildOder = finder.GetDependencyOrder(options);
            var generator = Injection.Engine.Resolve<IMsBuildFileCore>();

            generator.WriteBuildScript(buildOder, options);
        }

        private static void ConfigureContainer()
        {
            Injection.Engine.AddNewExtension<MsBuilderificCoreContainerExtension>();
            Injection.Engine.AddNewExtension<VisualStudio2010ContainerExtension>();
        }
    }
}
