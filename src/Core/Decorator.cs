using System;

namespace Core
{
    public interface Decorable<ReturnT> {
        ReturnT Operation();
    }

    public abstract class Decorator<ReturnT>: Decorable<ReturnT> 
    {
        protected Decorable<ReturnT> Decorable;

        Decorator(Decorable<ReturnT> decorable)
        {
            Decorable = decorable;
        }
    }

    public class Compression: Decorator<string>
    {
        string Operation()
        {
            return "Compressed:" + Decorable.Operation();
        }
    }

    public class Encryption: Decorator<string>
    {
        string Operation()
        {
            return "Encrypted:" + Decorable.Operation();
        }
    }
}
