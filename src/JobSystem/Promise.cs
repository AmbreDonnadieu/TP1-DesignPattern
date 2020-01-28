using System;
using System.Collections.Generic;

namespace JobSystem
{
    public enum PromiseState
    {
        Pending,    
        Rejected, 
        Resolved,    
    };

    public interface IPromise
    {
        IPromise Then(Action onResolved);

        IPromise Catch(Action<Exception> onRejected);

        IPromise Finally(Action onComplete);

        void Done();
    }

    public class Promise : IPromise
    {
        private Exception rejectedException;

        private List<Action> resolvedCallbacks = null;

        private List<Action<Exception>> rejectedCallbacks = null;

        private PromiseState state = PromiseState.Pending;

        public void Resolve()
        {
            if (state != PromiseState.Pending)
            {
                throw new Exception(
                    "Attempt to reject a promise that is already in state: " + state 
                    + ", a promise can only be resolved when it is still in state: " 
                    + PromiseState.Pending
                );
            }

            state = PromiseState.Resolved;

            if (resolvedCallbacks != null)
            {
                foreach (Action callback in resolvedCallbacks)
                {
                    callback();
                }

                resolvedCallbacks = null;
            }
        }

        public void Reject(Exception ex)
        {
            if (state != PromiseState.Pending)
            {
                throw new Exception(
                    "Attempt to reject a promise that is already in state: " + state 
                    + ", a promise can only be rejected when it is still in state: " 
                    + PromiseState.Pending
                );
            }

            rejectedException = ex;
            state = PromiseState.Rejected;

            if (rejectedCallbacks != null)
            {
                foreach (Action<Exception> callback in rejectedCallbacks)
                {
                    callback(ex);
                }

                rejectedCallbacks = null;
            }
        }

        public IPromise Then(Action onResolved)
        {
            if(state == PromiseState.Resolved)
            {
                onResolved();
            }
            else
            {
                if(resolvedCallbacks == null)
                {
                    resolvedCallbacks = new List<Action>();
                }

                resolvedCallbacks.Add(onResolved);
            }

            return this;
        }

        public IPromise Catch(Action<Exception> onRejected)
        {
            if(state == PromiseState.Rejected)
            {
                onRejected(rejectedException);
            }
            else
            {
                if(rejectedCallbacks == null)
                {
                    rejectedCallbacks = new List<Action<Exception>>();
                }

                rejectedCallbacks.Add(onRejected);
            }

            return this;
        }

        public IPromise Finally(Action onComplete)
        {
            throw new NotImplementedException(); // TODO
        }

        public void Done()
        {
            throw new NotImplementedException(); // TODO
        }
    }

    public interface IPromise<PromisedType>
    {
        IPromise<PromisedType> Then(Action<PromisedType> onResolved);

        IPromise<PromisedType> Catch(Action<Exception> onRejected);

        IPromise<PromisedType> Finally(Action onComplete);

        void Done();
    }

    public class Promise<PromisedType> : IPromise<PromisedType>
    {
        private Exception rejectedException;

        private PromisedType resolvedValue;

        private List<Action<PromisedType>> resolvedCallbacks = null;

        private List<Action<Exception>> rejectedCallbacks = null;

        private PromiseState state = PromiseState.Pending;

        public void Resolve(PromisedType value)
        {
            if (state != PromiseState.Pending)
            {
                throw new Exception(
                    "Attempt to reject a promise that is already in state: " + state 
                    + ", a promise can only be resolved when it is still in state: " 
                    + PromiseState.Pending
                );
            }

            resolvedValue = value;
            state = PromiseState.Resolved;

            if (resolvedCallbacks != null)
            {
                foreach (Action<PromisedType> callback in resolvedCallbacks)
                {
                    callback(value);
                }

                resolvedCallbacks = null;
            }
        }

        public void Reject(Exception ex)
        {
            if (state != PromiseState.Pending)
            {
                throw new Exception(
                    "Attempt to reject a promise that is already in state: " + state 
                    + ", a promise can only be rejected when it is still in state: " 
                    + PromiseState.Pending
                );
            }

            rejectedException = ex;
            state = PromiseState.Rejected;

            if (rejectedCallbacks != null)
            {
                foreach (Action<Exception> callback in rejectedCallbacks)
                {
                    callback(ex);
                }

                rejectedCallbacks = null;
            }
        }

        public IPromise<PromisedType> Then(Action<PromisedType> onResolved)
        {
            if(state == PromiseState.Resolved)
            {
                onResolved(resolvedValue);
            }
            else
            {
                if(resolvedCallbacks == null)
                {
                    resolvedCallbacks = new List<Action<PromisedType>>();
                }

                resolvedCallbacks.Add(onResolved);
            }

            return this;
        }

        public IPromise<PromisedType> Catch(Action<Exception> onRejected)
        {
            if(state == PromiseState.Rejected)
            {
                onRejected(rejectedException);
            }
            else
            {
                if(rejectedCallbacks == null)
                {
                    rejectedCallbacks = new List<Action<Exception>>();
                }

                rejectedCallbacks.Add(onRejected);
            }

            return this;
        }

        public IPromise<PromisedType> Finally(Action onComplete)
        {
            throw new NotImplementedException(); // TODO
        }

        public void Done()
        {
            throw new NotImplementedException(); // TODO
        }
    }
}
