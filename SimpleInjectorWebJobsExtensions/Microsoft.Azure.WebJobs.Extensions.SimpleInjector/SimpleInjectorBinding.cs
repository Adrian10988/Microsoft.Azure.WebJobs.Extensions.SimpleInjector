using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Protocols;
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
    public class SimpleInjectorBinding : IBinding
    {
        public bool FromAttribute => false;

        private readonly Type _type;
        private Container _container;
        private readonly Lifestyle _lifeStyle;
        private ConcurrentDictionary<Guid, Scope> _scopeDict;
        public SimpleInjectorBinding(Type type, Container container, Lifestyle lifestyle, ConcurrentDictionary<Guid, Scope> scopeDict)
        {
            _type = type;
            _container = container;
            _lifeStyle = lifestyle;
            _scopeDict = scopeDict;
        }



        public Task<IValueProvider> BindAsync(object value, ValueBindingContext context)
        {
            return Task.FromResult<IValueProvider>(new ResolutionProvider(_container.GetInstance(_type)));
        }

        public Task<IValueProvider> BindAsync(BindingContext context)
        {
            System.Diagnostics.Debug.WriteLine($"[A]Function Instance ID: {context.FunctionInstanceId}");
            Scope scope = null;
            if (_lifeStyle is AsyncScopedLifestyle)
                scope = AsyncScopedLifestyle.BeginScope(_container);
            else if(_lifeStyle is ThreadScopedLifestyle)
                scope = ThreadScopedLifestyle.BeginScope(_container);
            
            if(scope != null)
            {
                _scopeDict.TryAdd(context.FunctionInstanceId, scope);
            }

            //is singleton
            return Task.FromResult<IValueProvider>(new ResolutionProvider(_container.GetInstance(_type)));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new ParameterDescriptor();
        }
    }
}
