using System;
using System.Threading;
using System.Threading.Tasks;

namespace GitBasic
{
    public class BackgroundQueue
    {
        private Task _previousTask = Task.FromResult(true);
        private object _lockKey = new object();

        /// <summary>
        /// Adds an action to the task queue. It will be executed
        /// after all previously scheduled actions are complete.
        /// </summary>
        /// <param name="action">The action to add to the queue.</param>
        /// <returns>The background task.</returns>
        public Task QueueTask(Action action)
        {
            lock (_lockKey)
            {
                _previousTask = _previousTask.ContinueWith(
                    t => action(),
                    CancellationToken.None,
                    TaskContinuationOptions.None,
                    TaskScheduler.Default);
                return _previousTask;
            }
        }
    }
}
