using System;
using CommandLine;
using MsBuilderific.Core;
using MsBuilderific.Visitors.Build;
using MsBuilderific.Visitors.Clean;

namespace MsBuilderific.Console
{
    class Program
    {
        static void Main(string[] args)
        {            
            var options = new Options();

            if (!CommandLineParser.Default.ParseArguments(args, options, System.Console.Out))
                Environment.Exit(1);

            var finder = new ProjectDependencyFinder(options.VbNetSupport, options.CSharpSupport);

            if (options.ExclusionPatterns != null)
            {
                foreach (var exclusion in options.ExclusionPatterns)
                    finder.AddExclusionPattern(exclusion);
            }

            var buildOder = finder.GetDependencyOrder(options.RootFolder, options.GraphFilename);

            var generator = new MsBuildFileCore();

            generator.AcceptVisitor(new CleanBuildArtefactsVisitor());
            generator.AcceptVisitor(new MsBuildProjectVisitor());
            generator.AcceptVisitor(new MsDeployProjectVisitor());
            generator.AcceptVisitor(new MsTestsProjectVisitor());
            generator.AcceptVisitor(new CopyProjectOutputVisitor());
            generator.AcceptVisitor(new CopyRessourcesVisitor(new RessourceFinder()));
            generator.AcceptVisitor(new CopyMsDeployPackagesVisitor());
            generator.AcceptVisitor(new DeleteBinAfterPackagingVisitor());

            generator.WriteBuildScript(buildOder, options);
        }
    }
}
