using System;
using System.Threading;
using System.Threading.Tasks;

namespace Some.Lambda.Integrations
{
    public class ActionWaiter
    {
        private const int TimeoutMilliseconds = 120000; //2 minutes
        private const double MillisecondsDelay = 3000; //3 seconds

        public T Wait<T>(Func<T> func, Predicate<T> condition, int timeoutMilliseconds = TimeoutMilliseconds)
        {
            using (var cancellationTokenSource = new CancellationTokenSource(timeoutMilliseconds))
            {
                try
                {
                    var action = InvokeAction(func, condition, cancellationTokenSource.Token);

                    return action;
                }
                catch (OperationCanceledException)
                {
                    throw new TimeoutException("Timeout exception waiting for the action to finish");
                }
            }
        }

        private T InvokeAction<T>(Func<T> func, Predicate<T> condition, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var objToEvaluate = func.Invoke();
                var result = condition.Invoke(objToEvaluate);

                if (result)
                {
                    return objToEvaluate;
                }

                Task.Delay(TimeSpan.FromMilliseconds(MillisecondsDelay), cancellationToken).GetAwaiter().GetResult();
            }

            throw new OperationCanceledException();
        }
    }
}
