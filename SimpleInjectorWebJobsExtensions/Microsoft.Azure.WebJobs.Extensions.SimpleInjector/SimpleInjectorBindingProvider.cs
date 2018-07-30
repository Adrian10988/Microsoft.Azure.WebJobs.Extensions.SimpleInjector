using Microsoft.Azure.WebJobs.Host.Bindings;
using SimpleInjector;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Extensions.SimpleInjector
{
    public class SimpleInjectorBindingProvider : IBindingProvider
    {
        private Container _container;
        private Lifestyle _lifeStyle;
        private readonly ConcurrentDictionary<Guid, Scope> _scopeDict;
        public SimpleInjectorBindingProvider(Container container, Lifestyle lifestyle, ConcurrentDictionary<Guid, Scope> scopeDict)
        {
            _container = container;
            _lifeStyle = lifestyle;
            _scopeDict = scopeDict;
        }

        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context is null");

            var type = context.Parameter.ParameterType;

            //If there are no types registered in the container then do not even try to bind to the parameter
            var registrations = _container.GetCurrentRegistrations();

            if (!registrations.Any(a => a.ServiceType == type))
                return Task.FromResult<IBinding>(null);

            //it seems we have a registration for the parameter, continue along and try to bind the value from the container
            return Task.FromResult<IBinding>(new SimpleInjectorBinding(type, _container, _lifeStyle, _scopeDict));
        }
    }
}
