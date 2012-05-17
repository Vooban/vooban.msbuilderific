namespace MsBuilderific.Contracts.Visitors
{
    public abstract class BuildOrderVisitor
    {
        public virtual string PreVisitBuildTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            return null;
        }

        public virtual string VisitBuildAllTypeTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            return null;
        }

        public virtual string VisitBuildLibraryProjectTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            return null;
        }

        public virtual string VisitBuildWebProjectTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            return null;
        }

        public virtual string VisitBuildExeProjectTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            return null;
        }  

        public virtual string PostVisitBuildTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            return null;
        }

        public virtual string PreVisitServiceTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            return null;
        }

        public virtual string VisitServiceTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            return null;
        }

        public virtual string PostVisitServiceTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            return null;
        }

        public virtual string PreVisitCleanTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            return null;
        }

        public virtual string VisitCleanTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            return null;
        }

        public virtual string PostVisitCleanTarget(VisualStudioProject project, IMsBuilderificOptions options)
        {
            return null;
        }

        public int Order { get; set; }

        public virtual bool ShallExecute(IMsBuilderificOptions options)
        {
            return true;
        }
    }
}
