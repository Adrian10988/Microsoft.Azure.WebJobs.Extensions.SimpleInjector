using Microsoft.Azure.WebJobs.Host.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Extensions.SimpleInjector
{
    public class ResolutionProvider : IValueProvider
    {
        public Type Type => _instance.GetType();
        private object _instance;

        public ResolutionProvider(object instance)
        {
            _instance = instance;
        }


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
