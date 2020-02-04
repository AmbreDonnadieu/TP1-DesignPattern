using System;

namespace Core
{
    public interface Decorable<ReturnT> {
        ReturnT Operation();
    }

    public abstract class Decorator<ReturnT>: Decorable<ReturnT> 
    {
        protected Decorable<ReturnT> Decorable;

        public Decorator(Decorable<ReturnT> decorable)
        {
            Decorable = decorable;
        }

        public abstract ReturnT Operation();
    }

    public class Compression: Decorator<string>
    {
        public Compression(Decorable<string> decorable): base(decorable)
        {
        }

        public override string Operation()
        {
            return "Compressed:" + Decorable.Operation();
        }
    }

    public class Encryption: Decorator<string>
    {
        public Encryption(Decorable<string> decorable): base(decorable)
        {
        }

        public override string Operation()
        {
            return "Encrypted:" + Decorable.Operation();
        }
    }
}
