using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;

namespace Microsoft.Azure.WebJobs.Extensions.SimpleInjector
{
    public class ResolutionProvider : IValueProvider
    {
        private readonly object _instance;

        public ResolutionProvider(object instance)
        {
            _instance = instance;
        }

        public Type Type => _instance.GetType();


        public Task<object> GetValueAsync()
        {
            return Task.FromResult(_instance);
        }

        public string ToInvokeString()
        {
            return Type.ToString();
        }
    }
}