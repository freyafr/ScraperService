using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScraperService.Helpers
{
    public interface IStateMonitor
    {
        bool OptainLock();
        void ReleaseLock();
    }
    public class StateMonitor: IStateMonitor
    {
        private readonly object _lockObject = new object();
        private volatile bool _isBusy;
        public bool OptainLock()
        {
            if (_isBusy)
                return false;
            lock (_lockObject)
            {
                if (_isBusy)
                    return false;
                _isBusy = true;
                return true;
            }
        }

        public void ReleaseLock()
        {
            _isBusy = false;
        }
    }
}
