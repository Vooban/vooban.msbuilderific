namespace MsBuilderific.Contracts.Visitors
{
    public interface IBuildOrderVisitor
    {
        string PreVisitBuildTarget(VisualStudioProject project, IMsBuilderificOptions options);
        string VisitBuildAllTypeTarget(VisualStudioProject project, IMsBuilderificOptions options);
        string VisitBuildLibraryProjectTarget(VisualStudioProject project, IMsBuilderificOptions options);
        string VisitBuildWebProjectTarget(VisualStudioProject project, IMsBuilderificOptions options);
        string VisitBuildExeProjectTarget(VisualStudioProject project, IMsBuilderificOptions options);
        string PostVisitBuildTarget(VisualStudioProject project, IMsBuilderificOptions options);
        string PreVisitServiceTarget(VisualStudioProject project, IMsBuilderificOptions options);
        string VisitServiceTarget(VisualStudioProject project, IMsBuilderificOptions options);
        string PostVisitServiceTarget(VisualStudioProject project, IMsBuilderificOptions options);
        string PreVisitCleanTarget(VisualStudioProject project, IMsBuilderificOptions options);
        string VisitCleanTarget(VisualStudioProject project, IMsBuilderificOptions options);
        string PostVisitCleanTarget(VisualStudioProject project, IMsBuilderificOptions options);
        int Order { get; set; }
        bool ShallExecute(IMsBuilderificOptions options);
    }
}