using System;

namespace JobSystem
{
    public interface IJob
    {
        void Execute();
    }

    public interface IJob<ResultType>
    {
        ResultType Execute();
    }
}
