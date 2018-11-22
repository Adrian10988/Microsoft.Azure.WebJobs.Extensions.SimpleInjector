using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using SimpleInjector;

namespace Microsoft.Azure.WebJobs.Extensions.SimpleInjector
{
    public class LifestyleInterceptor : FunctionInvocationFilterAttribute
    {
        private readonly ConcurrentDictionary<Guid, Scope> _scopeDict;

        public LifestyleInterceptor(Lifestyle lifestyle, Container container,
            ConcurrentDictionary<Guid, Scope> scopeDict)
        {
            _scopeDict = scopeDict;
        }

        public override Task OnExecutedAsync(FunctionExecutedContext executedContext,
            CancellationToken cancellationToken)
        {
            if (!_scopeDict.ContainsKey(executedContext.FunctionInstanceId))
                return base.OnExecutedAsync(executedContext, cancellationToken);

            var scope = _scopeDict[executedContext.FunctionInstanceId];
            scope.Dispose();
            _scopeDict.TryRemove(executedContext.FunctionInstanceId, out scope);

            return base.OnExecutedAsync(executedContext, cancellationToken);
        }
    }
}