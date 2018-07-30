using Microsoft.Azure.WebJobs.Host;
using SimpleInjector;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Extensions.SimpleInjector
{
    public class LifestyleInterceptor : FunctionInvocationFilterAttribute
    {
        private List<Scope> _previousScopes = new List<Scope>();
        private readonly Lifestyle _lifestyle;
        private readonly Container _container;
        private readonly ConcurrentDictionary<Guid, Scope> _scopeDict;
        public LifestyleInterceptor(Lifestyle lifestyle, Container container, ConcurrentDictionary<Guid, Scope> scopeDict)
        {
            _lifestyle = lifestyle;
            _container = container;
            _scopeDict = scopeDict;
            
        }

        //This may be useful later
        //public override Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
        //{


        //    return base.OnExecutingAsync(executingContext, cancellationToken);
        //}

        public override Task OnExecutedAsync(FunctionExecutedContext executedContext, CancellationToken cancellationToken)
        {
            System.Diagnostics.Debug.WriteLine($"[B]Function Instance ID: {executedContext.FunctionInstanceId}");
            if (_scopeDict.ContainsKey(executedContext.FunctionInstanceId))
            {
                var scope = _scopeDict[executedContext.FunctionInstanceId];
                scope.Dispose();
                _scopeDict.TryRemove(executedContext.FunctionInstanceId, out scope);
            }
                
            return base.OnExecutedAsync(executedContext, cancellationToken);
        }

    }
}
