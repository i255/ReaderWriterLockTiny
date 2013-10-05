using System;
using System.Threading;

namespace ReaderWriterLockTiny
{
    /// <summary>
    /// Hierarchical locks are not supported!!!
    /// </summary>
    struct ReaderWriterLockTiny
    {
        // if lock is above this value then somebody has a write lock
        const int _writerLock = 1000000;
        // lock state counter
        int _lock;

        public void EnterReadLock()
        {
            var w = new SpinWait();
            var tmpLock = _lock;
            while (tmpLock >= _writerLock ||
                tmpLock != Interlocked.CompareExchange(ref _lock, tmpLock + 1, tmpLock))
            {
                w.SpinOnce();
                tmpLock = _lock;
            }
        }

        public void EnterWriteLock()
        {
            var w = new SpinWait();

            while (0 != Interlocked.CompareExchange(ref _lock, _writerLock, 0))
            {
                w.SpinOnce();
            }
        }

        /// <summary>
        /// read lock must be aquired prior to calling this!!! we do not check it !!!!
        /// </summary>
        public void UpgradeToWrite()
        {
            var w = new SpinWait();

            while (1 != Interlocked.CompareExchange(ref _lock, _writerLock + 1, 1))
            {
                w.SpinOnce();
            }
        }

        public void DowngradeToRead()
        {
            _lock = 1;
        }

        public void ExitReadLock()
        {
            Interlocked.Decrement(ref _lock);
        }

        public void ExitWriteLock()
        {
            _lock = 0;
        }

        public override string ToString()
        {
            return "lock counter: " + _lock;
        }

    }
}
