using System;
using System.Collections.Generic;
using System.Timers;

namespace GitBasic
{
    public class ThrottledAction
    {
        public ThrottledAction(double throttleTime = 500)
        {
            _timer.Interval = throttleTime;
            _timer.AutoReset = false;
            _timer.Elapsed += _timer_Elapsed;
        }

        public void Execute(Action action)
        {
            lock (_lockKey)
            {
                _timer.Stop();                
                _actions.Add(action);
                _timer.Start();
            }
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_lockKey)
            {
                _actions.ForEach(action => action.Invoke());
            }
        }

        private Timer _timer = new Timer();
        private HashSet<Action> _actions = new HashSet<Action>();
        private object _lockKey = new object();
    }
}
