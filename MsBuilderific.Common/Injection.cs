using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace MsBuilderific.Common
{
    public class Injection
    {
        private static UnityContainer _engine;

        public static IUnityContainer Engine
        {
            get
            {
                if (_engine == null)
                {
                    _engine = new UnityContainer();
                    _engine.LoadConfiguration();
                }

                return _engine;
            }
        }
    }
}
