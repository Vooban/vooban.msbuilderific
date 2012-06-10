using System.Collections.Generic;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="MsTestsProjectVisitor"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public MsTestsProjectVisitor(MsTestsProjectVisitorOptions options)
        {
            _options=options;
        }

        /// <summary>
        /// Call for program projects (exe) when it is time to add build information to the build file for the <paramref name="project"/>, for each possible targets
        /// </summary>
        /// <param name="project">The project that is being visited for build</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>
        /// The msbuild syntax to add to the build file
        /// </returns>
        public override string VisitBuildExeProjectTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            // Same logic for exe and library projects, defaults to same implementation
            return VisitBuildLibraryProjectTarget(project, coreOptions);
        }

        /// <summary>
        /// Call for project library when it is time to add build information to the build file for the <paramref name="project"/>, for each possible targets
        /// </summary>
        /// <param name="project">The project that is being visited for build</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>
        /// The msbuild syntax to add to the build file
        /// </returns>
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

        /// <summary>
        /// Get a value indicating whether or not the visitor should execute, based on the core options specified
        /// </summary>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>
        ///   <c>true</c> if the visitor shall be called, <c>false</c> otherwise
        /// </returns>
        public override bool ShallExecute(IMsBuilderificCoreOptions coreOptions)
        {
            return _options.GenerateMsTestTask && !string.IsNullOrEmpty(_options.TestDiscoveryPattern);
        }

        /// <summary>
        /// Allow the visitors to add properties in the main property group of the MsBuild file
        /// </summary>
        /// <returns>
        /// A dictionary of property name/value to add the MsBuild property group section
        /// </returns>
        public override IDictionary<string, string> GetVisitorProperties()
        {
            var properties = new Dictionary<string, string>
                                 {
                                     {"ExecuteTests", "True"},
                                     {"ContinueOnTestError", "False"},
                                     {"MsTestCategories", "Unit"},
                                     {"TestBinFolder", @"bin\Debug"},
                                     {"MsTestLocation", @"%ProgramFiles%\Microsoft Visual Studio 10.0\Common7\IDE\mstest.exe"},
                                 };

            return properties;
        }
    }
}