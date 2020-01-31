using System;
using System.Collections.Generic;
using System.Threading;

namespace JobSystem
{
    public class JobSystem
    {
        private List<Thread> threads = new List<Thread>();
        private Queue<Action> pendingJobs = new Queue<Action>();
        private bool stopRequested = false;

        public JobSystem()
        {
            for(int i = 0; i < Environment.ProcessorCount; i++)
            {
                threads.Add(new Thread(ThreadWorker));
            }
        }

        private void ThreadWorker()
        {
            while(stopRequested == false)
            {
                Action job;
                if(pendingJobs.TryDequeue(out job)) // thread safe way
                {
                    job(); // execute job
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        public IPromise AddJob(IJob job)
        {
            var promise = new Promise();

            Action jobAction = () => {
                try {
                    job.Execute();
                    promise.Resolve();
                } catch (Exception ex) {
                    promise.Reject(ex);
                }
            };

            pendingJobs.Enqueue(jobAction);

            return promise;
        }

        public IPromise<ResultType> AddJob<ResultType>(IJob<ResultType> job)
        {
            var promise = new Promise<ResultType>();

            Action jobAction = () => {
                try {
                    promise.Resolve(job.Execute());
                } catch (Exception ex) {
                    promise.Reject(ex);
                }
            };

            pendingJobs.Enqueue(jobAction);

            return promise;
        }

        public void Stop()
        {
            stopRequested = true;
        }
    }
}
