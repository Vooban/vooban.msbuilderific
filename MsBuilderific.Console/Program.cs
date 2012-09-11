using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using CommandLine.Text;
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

            var options = GetCommandLineOptions(args);
            var buildOder = GetBuildOder(options);
            var generator = Injection.Engine.Resolve<IMsBuildFileCore>();

            generator.WriteBuildScript(buildOder, options);
        }

        private static List<VisualStudioProject> GetBuildOder(IMsBuilderificCoreOptions options)
        {
            var finder = Injection.Engine.Resolve<IProjectDependencyFinder>();

            if (options.ProjectDetectionExclusionPatterns != null)
            {
                foreach (var exclusion in options.ProjectDetectionExclusionPatterns)
                    finder.AddExclusionPattern(exclusion);
            }

            return finder.GetDependencyOrder(options);
        }

        private static IMsBuilderificCoreOptions GetCommandLineOptions(string[] args)
        {
            var options = Injection.Engine.Resolve<IMsBuilderificCoreOptions>();
            var visitorOptions = Injection.Engine.ResolveAll<IVisitorOptions>();
            var writer = new StringWriter();

            if (args != null && args.Length > 0)
            {
                var parserSettings = new CommandLineParserSettings(false, false, true, System.Console.Out);
                var parser = new CommandLineParser(parserSettings);

                if (!parser.ParseArguments(args, options, writer))
                {
                    visitorOptions.ForEach(v => parser.ParseArguments(args, v, writer));
                    System.Console.WriteLine(writer.ToString());
                    Environment.Exit(1);
                }
                else
                {
                    visitorOptions.ForEach(v =>
                                               {
                                                   if (!parser.ParseArguments(args, v, writer))
                                                       Environment.Exit(1);

                                                   Injection.Engine.RegisterInstance(v.GetType(), v);
                                               });
                }
            }
            else
            {
                var commandLineOptionsCore = options as CommandLineOptionsBase;
                if (commandLineOptionsCore != null)
                {
                    var help = HelpText.AutoBuild(commandLineOptionsCore, current => HelpText.DefaultParsingErrorsHandler(commandLineOptionsCore, current));
                    System.Console.WriteLine(help);

                    visitorOptions.ForEach(v =>
                        {
                            var visitorOptionBase = v as CommandLineOptionsBase;
                            if (visitorOptionBase == null) 
                                return;

                            help = HelpText.AutoBuild(v, current => HelpText.DefaultParsingErrorsHandler(visitorOptionBase, current));
                            System.Console.WriteLine(help);
                        });
                }

                Environment.Exit(2);
            }
            return options;
        }

        private static void ConfigureContainer()
        {
            Injection.Engine.AddNewExtension<MsBuilderificCoreContainerExtension>();
            Injection.Engine.AddNewExtension<VisualStudio2010ContainerExtension>();
        }
    }
}
