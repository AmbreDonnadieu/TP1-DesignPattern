using System;
using System.Collections.Generic;

namespace JobSystem
{
    //on a utilis� le design pattern "Promesse"
    //ainsi, la m�thode qui effectue des modifications des donn�es "promet" de renvoyer un resultat/objet quoiqu'il arrive
    //cela d'�viter d'avoir une multitude de catch exception qui boucle sur eux-m�me


    //PromiseState correspond � l'etat de la promesse/requ�te de d�part
    public enum PromiseState
    {
        Pending,    
        Rejected, 
        Resolved,    
    };


    //cet interface sera appel� � chaque d�but de t�che 
    //il permet de prot�ger l'acc�s aux donn�es des t�ches en cours sur le thread puisque seul l'essentiel est accessible
    public interface IPromise // la m�thode "promet" de renvoyer un objet quoiqu'il arrive
    {
        //Est appel� quand la promesse est un succ�s (est compos� du callback de fin)
        IPromise Then(Action onResolved);

        //idem que le then mais quand il y a une exception (que la promesse est un echec entre autre)
        IPromise Catch(Action<Exception> onRejected);

        //Appelle le callback qu'il y ait une erreur ou non 
        IPromise Finally(Action onComplete);

        //Pour afficher les exceptions rien n'a �t� catch
        void Done();
    }


    //C'est la classe compl�te de la promesse avec toutes les m�thodes de modifications 
    //Cette classe h�rite de l'interface et impl�mente r�ellement les m�thodes de l'interface
    //cette partie est pour les promesses qui ne retournent rien et dont on a juste besoin du suivit de l'execution de la t�che
    public class Promise : IPromise
    {
        private Exception rejectedException;

        private List<Action> resolvedCallbacks = null;

        private List<Action<Exception>> rejectedCallbacks = null;

        private PromiseState state = PromiseState.Pending;

        //Permet de mettre la valeur de l'etat de la t�che en cours 
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

        //Quand l'action a une erreur, �a renvoie une exception
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

        //c'est l'appel du callback quand l'action est finie
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

        //renvoie une exception s'il y a une erreur dans l'execution de la t�che
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

        //Envoie l'etat de la t�che en cours qu'il y ait eu succ�s ou erreur lors du traitement
        public IPromise Finally(Action onComplete)
        {
            throw new NotImplementedException(); // TODO
        }

        public void Done()
        {
            throw new NotImplementedException(); // TODO
        }
    }



    //pour retourner les promesses qui retournent une valeur � la fin de l'execution de la t�che
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

        //Quand l'action a une erreur, �a renvoie une exception
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
