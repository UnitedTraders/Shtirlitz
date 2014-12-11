using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Shtirlitz.Reporter;
using Shtirlitz.Reporter.ThreadsDumpReporter;
using Xunit;


namespace Shtirlitz.Tests
{
    public class ThreadsReporterTest : ShtirlitzBaseTestClass
    {
        private readonly ThreadsDumpReporter threadsReporter;


        public ThreadsReporterTest()
            : this(new ThreadsDumpReporter())
        { }


        public ThreadsReporterTest(ThreadsDumpReporter reporter)
            : base(new List<IReporter> { reporter })
        {
            this.threadsReporter = reporter;
        }


        [Fact]
        public void CreateThreadsDump()
        {
            //------------------------- Test for ThreadDump
            string name = "Thread test name";
            int managedThreadId = 743;
            StackTrace stackTrace = new StackTrace();
            ThreadDump dump = new ThreadDump(name, managedThreadId, stackTrace);
            Assert.Same(name, dump.Name);
            Assert.Equal<int>(managedThreadId, dump.ManagedThreadId);
            Assert.Same(stackTrace, dump.StackTrace);

            //------------------------- Test for ThreadsDumpReporter

            // test thread 1
            Thread thread1 = Thread.CurrentThread; // get thread 1
            threadsReporter.RegisterThread(thread1); // register thread 1

            var dumps = threadsReporter.CreateDumps(true);

            // test dump for thread 1
            Assert.Equal(1, dumps.Count);

            Assert.Equal(dumps[0].ManagedThreadId, thread1.ManagedThreadId);
            Assert.Equal(dumps[0].Name, thread1.Name);
            Assert.NotNull(dumps[0].StackTrace);
            Assert.True(dumps[0].StackTrace.FrameCount > 0);

            // test threads 1 + 2
            Thread thread2 = new Thread(() => { Thread.Sleep(1000); }); // create thread 2
            thread2.Start();

            threadsReporter.RegisterThread(thread2); // register thread 2
            threadsReporter.RegisterThread(thread2); // test double register

            dumps = threadsReporter.CreateDumps(true);

            // test dump for thread 1 + thread 2
            Assert.Equal(2, dumps.Count);

            Assert.Equal(dumps[0].ManagedThreadId, thread1.ManagedThreadId);
            Assert.Equal(dumps[0].Name, thread1.Name);
            Assert.NotNull(dumps[0].StackTrace);
            Assert.True(dumps[0].StackTrace.FrameCount > 0);

            Assert.Equal(dumps[1].ManagedThreadId, thread2.ManagedThreadId);
            Assert.Equal(dumps[1].Name, thread2.Name);
            Assert.NotNull(dumps[1].StackTrace);
            Assert.True(dumps[1].StackTrace.FrameCount > 0);

            // test stopped thread 2
            thread2.Join(); // wait for terminate thread 2

            threadsReporter.UnregisterThread(thread1);

            dumps = threadsReporter.CreateDumps(true);

            // test dump for thread 2
            Assert.Equal(1, dumps.Count);

            Assert.Equal(dumps[0].ManagedThreadId, thread2.ManagedThreadId);
            Assert.Equal(dumps[0].Name, thread2.Name);
            Assert.Null(dumps[0].StackTrace);

            // test empty
            threadsReporter.UnregisterThread(thread2);
            threadsReporter.UnregisterThread(thread2); // test double unregister

            dumps = threadsReporter.CreateDumps(true);

            // test dump for thread 2
            Assert.Equal(0, dumps.Count);

            //------------------------- Test reporter files
            thread1 = Thread.CurrentThread;
            thread2 = new Thread(() => { Thread.Sleep(3000); });
            thread2.Start();

            threadsReporter.RegisterThread(thread1);
            threadsReporter.RegisterThread(thread2);

            // act
            RunSynchronously(false);

            Assert.True(File.Exists(Path.Combine(RootPath, ThreadsDumpReporter.PathSuffix, string.Format("{0}.txt", thread1.ManagedThreadId))));
            Assert.True(File.Exists(Path.Combine(RootPath, ThreadsDumpReporter.PathSuffix, string.Format("{0}.txt", thread2.ManagedThreadId))));

            thread2.Join();
        }
    }
}