using System;
using System.Collections.Generic;

namespace JobSystem
{
    public class JobSystem
    {
        private Queue<Action> pendingJobs = new Queue<Action>();

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
    }
}
