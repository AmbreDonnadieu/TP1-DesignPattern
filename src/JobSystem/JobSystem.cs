﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace JobSystem
{
    public class JobSystem
    {
        private List<Thread> threads = new List<Thread>();
        private Queue<Action> pendingJobs = new Queue<Action>();
        private bool stopRequested = false;

        //Constructeur qui initialise le nombre de thread du job system en fonction du nombre de processeur dispo sur la machine en cours
        public JobSystem()
        {
            for(int i = 0; i < Environment.ProcessorCount; i++)
            {
                threads.Add(new Thread(ThreadWorker));
            }
        }


        //Fonction qui tourne sur tous les threads pour savoir s'il y a besoin de prendre une nouvelle tâche à la fin de la précédente
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

        // Ajout d'une tâche à la liste des jobs à faire (communs à tous les threads)
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
