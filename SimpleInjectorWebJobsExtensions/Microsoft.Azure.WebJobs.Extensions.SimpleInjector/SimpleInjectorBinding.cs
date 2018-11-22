using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Microsoft.Azure.WebJobs.Extensions.SimpleInjector
{
    public class SimpleInjectorBinding : IBinding
    {
        private readonly Lifestyle _lifeStyle;

        private readonly Type _type;
        private readonly Container _container;
        private readonly ConcurrentDictionary<Guid, Scope> _scopeDict;

        public SimpleInjectorBinding(Type type, Container container, Lifestyle lifestyle,
            ConcurrentDictionary<Guid, Scope> scopeDict)
        {
            _type = type;
            _container = container;
            _lifeStyle = lifestyle;
            _scopeDict = scopeDict;
        }

        public bool FromAttribute => false;


        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            return Task.FromResult<IValueProvider>(new ResolutionProvider(_container.GetInstance(_type)));
        }

        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            Debug.WriteLine($"[A]Function Instance ID: {context.FunctionInstanceId}");
            Scope scope = null;
            switch (_lifeStyle)
            {
                case AsyncScopedLifestyle _:
                    scope = AsyncScopedLifestyle.BeginScope(_container);
                    break;
                case ThreadScopedLifestyle _:
                    scope = ThreadScopedLifestyle.BeginScope(_container);
                    break;
            }

            if (scope != null) _scopeDict.TryAdd(context.FunctionInstanceId, scope);

            //is singleton
            return Task.FromResult<IValueProvider>(new ResolutionProvider(_container.GetInstance(_type)));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new ParameterDescriptor();
        }
    }
}