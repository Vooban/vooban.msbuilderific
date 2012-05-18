using Microsoft.Practices.Unity;

namespace MsBuilderific.Common
{
    public class Injection
    {
        private static IUnityContainer _engine;

        public static IUnityContainer Engine
        {
            get
            {
                if (_engine == null)
                {
                    _engine = new UnityContainer();
                }

                return _engine;
            }
        }
    }
}
