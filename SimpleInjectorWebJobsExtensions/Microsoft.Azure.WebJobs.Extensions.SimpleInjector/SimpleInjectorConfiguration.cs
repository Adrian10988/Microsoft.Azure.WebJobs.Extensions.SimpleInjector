using System;
using System.Collections.Concurrent;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Config;
using SimpleInjector;

namespace Microsoft.Azure.WebJobs.Extensions.SimpleInjector
{
    public class SimpleInjectorConfiguration : IExtensionConfigProvider
    {
        private readonly JobHostConfiguration _config;
        private readonly Container _container;
        private readonly Lifestyle _lifestyle;
        private readonly object _syncLock = new object();
        private bool _initialized;

        public SimpleInjectorConfiguration(Action<Container, object> bootstrapFunction, object additionalParam,
            Lifestyle lifestyle,
            JobHostConfiguration config)
        {
            _lifestyle = lifestyle;
            _config = config;

            _container = new Container();
            bootstrapFunction(_container, additionalParam);
        }

        public SimpleInjectorConfiguration(Container container, Lifestyle lifestyle, JobHostConfiguration config)
        {
            _container = container;
            _lifestyle = lifestyle;
            _config = config;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            lock (_syncLock)
            {
                if (_initialized) return;
                var dict = new ConcurrentDictionary<Guid, Scope>();

                var extensions = _config.GetService<IExtensionRegistry>();
                extensions.RegisterExtension<IFunctionInvocationFilter>(
                    new LifestyleInterceptor(_lifestyle, _container, dict));
                context.Config.RegisterBindingExtension(
                    new SimpleInjectorBindingProvider(_container, _lifestyle, dict));
                _initialized = true;
            }
        }
    }
}