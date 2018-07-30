using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Extensions.SimpleInjector
{
    public static class JobHostHelperExtensions
    {
        public static void UseSimpleInjector(this JobHostConfiguration config, Action<Container, object> bootstrapFunction, object additionalParam,
            Lifestyle lifeStyle)
        {
            config.RegisterExtensionConfigProvider(new SimpleInjectorConfiguration(bootstrapFunction, additionalParam, lifeStyle, config));
        }
    }
}
