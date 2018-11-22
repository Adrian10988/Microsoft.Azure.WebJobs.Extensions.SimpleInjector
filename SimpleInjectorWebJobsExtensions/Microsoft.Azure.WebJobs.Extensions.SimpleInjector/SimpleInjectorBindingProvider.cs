using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using SimpleInjector;

namespace Microsoft.Azure.WebJobs.Extensions.SimpleInjector
{
    public class SimpleInjectorBindingProvider : IBindingProvider
    {
        private readonly ConcurrentDictionary<Guid, Scope> _scopeDict;
        private readonly Container _container;
        private readonly Lifestyle _lifeStyle;

        public SimpleInjectorBindingProvider(Container container, Lifestyle lifestyle,
            ConcurrentDictionary<Guid, Scope> scopeDict)
        {
            _container = container;
            _lifeStyle = lifestyle;
            _scopeDict = scopeDict;
        }

        public Task<IBinding> TryCreateAsync(BindingProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var type = context.Parameter.ParameterType;

            //If there are no types registered in the container then do not even try to bind to the parameter
            var registrations = _container.GetCurrentRegistrations();

            return Task.FromResult<IBinding>(registrations.All(a => a.ServiceType != type)
                ? null
            //it seems we have a registration for the parameter, continue along and try to bind the value from the container
                : new SimpleInjectorBinding(type, _container, _lifeStyle, _scopeDict));
        }
    }
}