using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReaderWriterLockTiny
{
    class Program
    {
        const int incrementTimes = 5;
        const int iterations = 10000000;
        static int num, collisions;
        static ReaderWriterLockTiny _tinyLock = new ReaderWriterLockTiny();
        static ReaderWriterLockSlim _slimLock = new ReaderWriterLockSlim();

        static void WriteUnlocked()
        {
            for (int j = 0; j < iterations; j++)
                for (int i = 0; i < incrementTimes; i++)
                    num++;
        }

        static void ReadUnlocked()
        {
            for (int j = 0; j < iterations; j++)
                if (num % incrementTimes != 0)
                    collisions++;
        }

        static void WriteTiny()
        {
            for (int j = 0; j < iterations; j++)
            {
                _tinyLock.EnterWriteLock();
                try
                {
                    for (int i = 0; i < incrementTimes; i++)
                        num++;
                }
                finally
                {
                    _tinyLock.ExitWriteLock();
                }
            }
        }

        static void ReadTiny()
        {
            for (int j = 0; j < iterations; j++)
            {
                _tinyLock.EnterReadLock();
                try
                {
                if (num % incrementTimes != 0)
                    collisions++;
                }
                finally
                {
                    _tinyLock.ExitReadLock();
                }
            }
        }

        static void WriteSlim()
        {
            for (int j = 0; j < iterations; j++)
            {
                _slimLock.EnterWriteLock();
                try
                {
                    for (int i = 0; i < incrementTimes; i++)
                        num++;
                }
                finally
                {
                    _slimLock.ExitWriteLock();
                }
            }
        }

        static void ReadSlim()
        {
            for (int j = 0; j < iterations; j++)
            {
                _slimLock.EnterReadLock();
                try
                {
                    if (num % incrementTimes != 0)
                        collisions++;
                }
                finally
                {
                    _slimLock.ExitReadLock();
                }
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Executing units: " + Environment.ProcessorCount);
            Test(new Action[] { ReadUnlocked, WriteUnlocked }, "Unlocked");
            Test(new Action[] { ReadSlim, WriteSlim }, _slimLock.GetType().Name);
            Test(new Action[] { ReadTiny, WriteTiny }, _tinyLock.GetType().Name);
        }

        private static Action[] Test(Action[] tasks, string test)
        {
            num = collisions = 0;
            tasks = Enumerable.Repeat(tasks, 10).SelectMany(x => x).ToArray();
            var dt = DateTime.Now;
            Parallel.Invoke(tasks);
            var dt2 = DateTime.Now;
            Console.WriteLine();
            Console.WriteLine(test + ":");
            Console.WriteLine("msec: " + (dt2 - dt).TotalMilliseconds);
            Console.WriteLine("read collisions: " + collisions);
            Console.WriteLine("result: " + num);
            Console.WriteLine("expected result: " + iterations * incrementTimes * tasks.Length / 2);
            return tasks;
        }

    }
}
