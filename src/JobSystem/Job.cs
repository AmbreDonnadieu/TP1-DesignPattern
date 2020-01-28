using System;

namespace JobSystem
{
    public interface IJob<ResultType>
    {
        ResultType Execute();
    }
}
