using System.Collections.Generic;

namespace MsBuilderific.Contracts.Visitors
{
    /// <summary>
    /// Defines what a visitors can achieve on the MsBuilderific platform
    /// </summary>
    public interface IBuildOrderVisitor
    {
        /// <summary>
        /// Called just before the build operation on the <paramref name="project"/>
        /// </summary>
        /// <param name="project">The project that will be visited for build</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>The msbuild syntax to add to the build file</returns>
        string PreVisitBuildTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);

        /// <summary>
        /// Call when it is time to add build information to the build file for the <paramref name="project"/>, for each possible targets
        /// </summary>
        /// <param name="project">The project that is being visited for build</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>The msbuild syntax to add to the build file</returns>
        string VisitBuildAllTypeTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);

        /// <summary>
        /// Call for project library when it is time to add build information to the build file for the <paramref name="project"/>, for each possible targets
        /// </summary>
        /// <param name="project">The project that is being visited for build</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>The msbuild syntax to add to the build file</returns>
        string VisitBuildLibraryProjectTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);
        
        /// <summary>
        /// Call for web project when it is time to add build information to the build file for the <paramref name="project"/>, for each possible targets
        /// </summary>
        /// <param name="project">The project that is being visited for build</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>The msbuild syntax to add to the build file</returns>
        string VisitBuildWebProjectTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);

        /// <summary>
        /// Call for program projects (exe) when it is time to add build information to the build file for the <paramref name="project"/>, for each possible targets
        /// </summary>
        /// <param name="project">The project that is being visited for build</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>The msbuild syntax to add to the build file</returns>
        string VisitBuildExeProjectTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);

        /// <summary>
        /// Called just after the build operation on the <paramref name="project"/>
        /// </summary>
        /// <param name="project">The project that was visited for build</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>The msbuild syntax to add to the build file</returns>
        string PostVisitBuildTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);

        /// <summary>
        /// Called just before adding specific instructions for a <paramref name="project"/> that contains WCF services
        /// </summary>
        /// <param name="project">The project that will be visited for build for the specific Service target</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>The msbuild syntax to add to the build file</returns>
        string PreVisitServiceTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);

        /// <summary>
        /// Called when it is time to add build specific instructions for a <paramref name="project"/> that contains WCF services
        /// </summary>
        /// <param name="project">The project that will be visited for build for the specific Service target</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>The msbuild syntax to add to the build file</returns>
        string VisitServiceTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);

        /// <summary>
        /// Called just after adding specific instructions for a <paramref name="project"/> that contains WCF services
        /// </summary>
        /// <param name="project">The project that will be visited for build for the specific Service target</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>The msbuild syntax to add to the build file</returns>
        string PostVisitServiceTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);

        /// <summary>
        /// Called just before adding specific instructions for the Clean target of the <paramref name="project"/>
        /// </summary>
        /// <param name="project">The project that will be visited for build for the specific Clean target</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>The msbuild syntax to add to the build file</returns>
        string PreVisitCleanTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);
        
        /// <summary>
        /// Called just when it is time to addi specific instructions for the Clean target of the <paramref name="project"/>
        /// </summary>
        /// <param name="project">The project that will be visited for build for the specific Clean target</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>The msbuild syntax to add to the build file</returns>
        string VisitCleanTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);

        /// <summary>
        /// Called just after adding specific instructions for the Clean target of the <paramref name="project"/>
        /// </summary>
        /// <param name="project">The project that will be visited for build for the specific Clean target</param>
        /// <param name="coreOptions">The application core options</param>
        /// <returns>The msbuild syntax to add to the build file</returns>
        string PostVisitCleanTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);
        
        /// <summary>
        /// Gets or sets the order in which this visitor shall be called compared to the other visitors you have
        /// defined in your configuration file
        /// </summary>
        int Order { get; set; }
        
        /// <summary>
        /// Get a value indicating whether or not the visitor should execute, based on the core options specified
        /// </summary>
        /// <param name="coreOptions">The application core options</param>
        /// <returns><c>true</c> if the visitor shall be called, <c>false</c> otherwise</returns>
        bool ShallExecute(IMsBuilderificCoreOptions coreOptions);

        /// <summary>
        /// Allow the visitors to add properties in the main property group of the MsBuild file
        /// </summary>
        /// <returns>A dictionary of property name/value to add the MsBuild property group section</returns>
        IDictionary<string, string> GetVisitorProperties();
    }
}