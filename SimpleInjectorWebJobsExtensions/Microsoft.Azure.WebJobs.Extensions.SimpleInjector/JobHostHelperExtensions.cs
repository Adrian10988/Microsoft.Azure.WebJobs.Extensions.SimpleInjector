using System;
using SimpleInjector;

namespace Microsoft.Azure.WebJobs.Extensions.SimpleInjector
{
    public static class JobHostHelperExtensions
    {
        public static void UseSimpleInjector(this JobHostConfiguration config,
            Action<Container, object> bootstrapFunction, object additionalParam,
            Lifestyle lifeStyle)
        {
            config.RegisterExtensionConfigProvider(new SimpleInjectorConfiguration(bootstrapFunction, additionalParam,
                lifeStyle, config));
        }

        public static void UseSimpleInjector(this JobHostConfiguration config, Container container, Lifestyle lifeStyle)
        {
            config.RegisterExtensionConfigProvider(new SimpleInjectorConfiguration(container, lifeStyle, config));
        }
    }
}