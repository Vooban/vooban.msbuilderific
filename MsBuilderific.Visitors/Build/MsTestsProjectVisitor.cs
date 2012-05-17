using System.Text.RegularExpressions;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Extensions.Extensions;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class MsTestsProjectVisitor : BuildOrderVisitor
    {
        public override string VisitBuildExeProjectTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            // Same logic for exe and library projects, defaults to same implementation
            return VisitBuildLibraryProjectTarget(project, options);
        }

        public override string VisitBuildLibraryProjectTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {            
            if (Regex.IsMatch(project.AssemblyName, options.TestDiscoveryPattern))
            {
                return string.Format("		<Exec Command='\"$(MsTestLocation)\" /testcontainer:{0}\\$(TestBinFolder)\\{1}.dll /runconfig:$(MsTestGlobalSettingsFile) /category:\"$(MsTestCategories)\" /usestderr /detail:errormessage /detail:errorstacktrace' ContinueOnError=\"$(ContinueOnTestError)\" Condition=\"$(ExecuteTests)\" />", project.GetRelativeFolderPath(options), project.AssemblyName);                
            }

            return null;
        }

        public override bool ShallExecute(IMsBuilderificOptions options)
        {
            return options.GenerateMsTestTask && !string.IsNullOrEmpty(options.TestDiscoveryPattern);
        }
    }
}