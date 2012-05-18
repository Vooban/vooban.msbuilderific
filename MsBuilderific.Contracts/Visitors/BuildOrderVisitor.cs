namespace MsBuilderific.Contracts.Visitors
{
    public abstract class BuildOrderVisitor : IBuildOrderVisitor
    {
        public virtual string PreVisitBuildTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            return null;
        }

        public virtual string VisitBuildAllTypeTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            return null;
        }

        public virtual string VisitBuildLibraryProjectTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            return null;
        }

        public virtual string VisitBuildWebProjectTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            return null;
        }

        public virtual string VisitBuildExeProjectTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            return null;
        }  

        public virtual string PostVisitBuildTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            return null;
        }

        public virtual string PreVisitServiceTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            return null;
        }

        public virtual string VisitServiceTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            return null;
        }

        public virtual string PostVisitServiceTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            return null;
        }

        public virtual string PreVisitCleanTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            return null;
        }

        public virtual string VisitCleanTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            return null;
        }

        public virtual string PostVisitCleanTarget(VisualStudioProject project, IMsBuilderificCoreOptions coreOptions)
        {
            return null;
        }

        public int Order { get; set; }

        public virtual bool ShallExecute(IMsBuilderificCoreOptions coreOptions)
        {
            return true;
        }
    }
}
