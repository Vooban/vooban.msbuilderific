using Microsoft.Practices.Unity;
using MsBuilderific.Common;
using MsBuilderific.Contracts;
using MsBuilderific.Contracts.Visitors;

namespace MsBuilderific.Core
{
    /// <summary>
    /// Unity container extension that will be used to register core functionality into the container
    /// </summary>
    public class MsBuilderificCoreContainerExtension : UnityContainerExtension
    {
        /// <summary>
        /// Initial the container with this extension's functionality.
        /// </summary>
        /// <remarks>
        /// When overridden in a derived class, this method will modify the given
        ///             <see cref="T:Microsoft.Practices.Unity.ExtensionContext"/> by adding strategies, policies, etc. to
        ///             install it's functions into the container.
        /// </remarks>
        protected override void Initialize()
        {
            Injection.Engine.RegisterType<IMsBuilderificCoreOptions, MsBuilderificCoreOptions>();

            var injectedBuilders = new InjectionConstructor(Injection.Engine.ResolveAll<IBuildOrderVisitor>());
            Injection.Engine.RegisterType<IMsBuildFileCore, MsBuildFileCore>(injectedBuilders);

            injectedBuilders = new InjectionConstructor(Injection.Engine.ResolveAll<IVisualStudioProjectLoader>());
            Injection.Engine.RegisterType<IProjectDependencyFinder, ProjectDependencyFinder>(injectedBuilders);
        }
    }
}
