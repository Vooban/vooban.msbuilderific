using System;
using System.Linq;
using CommandLine;
using Microsoft.Practices.ObjectBuilder2;
using MsBuilderific.Common;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Visitors;
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
                var parserSettings = new CommandLineParserSettings(false, false, true, System.Console.Out);
                var parser = new CommandLineParser(parserSettings);

                if (!parser.ParseArguments(args, options, System.Console.Out))
                    Environment.Exit(1);

                var visitorOptions = Injection.Engine.ResolveAll<IVisitorOptions>();
                visitorOptions.ForEach(v =>
                {
                    if (!parser.ParseArguments(args, v, System.Console.Out))
                        Environment.Exit(1);

                    Injection.Engine.RegisterInstance(v.GetType(), v);
                });           
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
