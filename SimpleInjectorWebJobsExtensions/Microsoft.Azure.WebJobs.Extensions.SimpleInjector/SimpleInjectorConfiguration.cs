using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Config;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Extensions.SimpleInjector
{
    public class SimpleInjectorConfiguration : IExtensionConfigProvider
    {
        private readonly object _syncLock = new object();
        private object _additionalParam;
        private Action<Container, object> _bootstrapFunction;
        private Container _container;
        private Lifestyle _lifestyle;
        private readonly JobHostConfiguration _config;

        public SimpleInjectorConfiguration(Action<Container, object> bootstrapFunction, object additionalParam, Lifestyle lifestyle,
            JobHostConfiguration config)
        {
            _bootstrapFunction = bootstrapFunction;
            _additionalParam = additionalParam;
            _lifestyle = lifestyle;
            _config = config;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            InitializeContainer(context);
        }

        private void InitializeContainer(ExtensionConfigContext context)
        {
            //System.Diagnostics.Debug.WriteLine("InitializeContainer called");
            if (_container != null)
                return;

            lock (_syncLock)
            {

                _container = new Container();

                var dict = new ConcurrentDictionary<Guid, Scope>();

                //now add lifestyle interceptor
                var extensions = _config.GetService<IExtensionRegistry>();
                extensions.RegisterExtension<IFunctionInvocationFilter>(new LifestyleInterceptor(_lifestyle, _container, dict));

                _bootstrapFunction(_container, _additionalParam);
                context.Config.RegisterBindingExtension(new SimpleInjectorBindingProvider(_container, _lifestyle, dict));
            }
        }
    }
}
