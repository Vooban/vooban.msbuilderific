using Microsoft.Practices.Unity;
using MsBuilderific.Contracts;

namespace MsBuilderific.Core.VisualStudio.V2010
{
    /// <summary>
    /// Extension that will register Visual Studio 2010 support for MsBuilderific with the 2010 registration name
    /// </summary>
    public class VisualStudio2010ContainerExtension : UnityContainerExtension
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
            Container.RegisterType<IVisualStudioProjectLoader, VisualStudio2010ProjectLoader>("2010");
            Container.RegisterType<IVisualStudioProjectRessourceFinder, VisualStudio2010RessourceFinder>("2010");
        }
    }
}
