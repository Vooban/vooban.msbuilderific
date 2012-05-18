namespace MsBuilderific.Contracts.Visitors
{
    public interface IBuildOrderVisitor
    {
        string PreVisitBuildTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);
        string VisitBuildAllTypeTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);
        string VisitBuildLibraryProjectTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);
        string VisitBuildWebProjectTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);
        string VisitBuildExeProjectTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);
        string PostVisitBuildTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);
        string PreVisitServiceTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);
        string VisitServiceTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);
        string PostVisitServiceTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);
        string PreVisitCleanTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);
        string VisitCleanTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);
        string PostVisitCleanTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions);
        int Order { get; set; }
        bool ShallExecute(IMsBuilderificCoreOptions coreOptions);
    }
}