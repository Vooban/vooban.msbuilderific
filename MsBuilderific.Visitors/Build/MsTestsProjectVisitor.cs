using System.Text;
using System.Text.RegularExpressions;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Extensions;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Visitors.Build
{
    public class MsTestsProjectVisitor : BuildOrderVisitor
    {
        private readonly MsTestsProjectVisitorOptions _options;

        public MsTestsProjectVisitor(MsTestsProjectVisitorOptions options)
        {
            _options=options;
        }

        public override string VisitBuildExeProjectTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            // Same logic for exe and library projects, defaults to same implementation
            return VisitBuildLibraryProjectTarget(project, coreOptions);
        }

        public override string VisitBuildLibraryProjectTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            if (Regex.IsMatch(project.AssemblyName, _options.TestDiscoveryPattern))
            {
                var builder = new StringBuilder();
                if (string.IsNullOrEmpty(_options.GlobalTestSettings))
                    builder.AppendLine(string.Format("		<Exec Command='\"$(MsTestLocation)\" /testcontainer:{0}\\$(TestBinFolder)\\{1}.dll /category:\"$(MsTestCategories)\" /usestderr /detail:errormessage /detail:errorstacktrace /resultsfile:\"{1}\".trx' ContinueOnError=\"$(ContinueOnTestError)\" Condition=\"$(ExecuteTests)\" />", project.GetRelativeFolderPath(coreOptions), project.AssemblyName));                
                else
                    builder.AppendLine(string.Format("		<Exec Command='\"$(MsTestLocation)\" /testcontainer:{0}\\$(TestBinFolder)\\{1}.dll /runconfig:\"{2}\" /category:\"$(MsTestCategories)\" /usestderr /detail:errormessage /detail:errorstacktrace/resultsfile:\"{1}\".trx' ContinueOnError=\"$(ContinueOnTestError)\" Condition=\"$(ExecuteTests)\" />", project.GetRelativeFolderPath(coreOptions), project.AssemblyName, _options.GlobalTestSettings));

                if(_options.GenerateTeamCityTransactionMessage)
                    builder.AppendLine(string.Format("<Message Text=\"##teamcity[importData type='mstest' path='{0}.trx']\"/>", project.AssemblyName));

                return builder.ToString();
            }

            return null;
        }

        public override bool ShallExecute(IMsBuilderificCoreOptions coreOptions)
        {
            return _options.GenerateMsTestTask && !string.IsNullOrEmpty(_options.TestDiscoveryPattern);
        }
    }
}