using System.Text.RegularExpressions;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Extensions;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class MsTestsProjectVisitor : BuildOrderVisitor
    {
        public override string VisitBuildExeProjectTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            // Same logic for exe and library projects, defaults to same implementation
            return VisitBuildLibraryProjectTarget(project, coreOptions);
        }

        public override string VisitBuildLibraryProjectTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {            
            if (Regex.IsMatch(project.AssemblyName, coreOptions.TestDiscoveryPattern))
            {
                return string.Format("		<Exec Command='\"$(MsTestLocation)\" /testcontainer:{0}\\$(TestBinFolder)\\{1}.dll /runconfig:$(MsTestGlobalSettingsFile) /category:\"$(MsTestCategories)\" /usestderr /detail:errormessage /detail:errorstacktrace' ContinueOnError=\"$(ContinueOnTestError)\" Condition=\"$(ExecuteTests)\" />", project.GetRelativeFolderPath(coreOptions), project.AssemblyName);                
            }

            return null;
        }

        public override bool ShallExecute(IMsBuilderificCoreOptions coreOptions)
        {
            return coreOptions.GenerateMsTestTask && !string.IsNullOrEmpty(coreOptions.TestDiscoveryPattern);
        }
    }
}