using System;

namespace JobSystem
{
    //Permet juste d'executer un job
    public interface IJob
    {
        void Execute();
    }

    public interface IJob<ResultType>
    {
        ResultType Execute();
    }
}
