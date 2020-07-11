using System;
using System.Collections.Generic;

namespace Tcs.RaceTimer.Observables
{
    public class Observable<T> : IObservable<T>
    {
        private readonly List<IObserver<T>> _observers = new List<IObserver<T>>();
        
        public virtual IDisposable Subscribe(IObserver<T> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }

            return new Subscription<T>(_observers, observer);
        }

        public virtual void Next(T item)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(item);
            }
        }
    }
}
