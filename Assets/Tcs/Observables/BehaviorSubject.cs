using System;
using System.Collections.Generic;
using System.Linq;
using Tcs.RaceTimer.Observables;

namespace Tcs.Observables
{
    public class BehaviorSubject<T> : Observable<T>
    {
        private T _lastValue;
        private List<Observable<T>> _copies;

        public BehaviorSubject(T initialValue)
        {
            _lastValue = initialValue;
        }

        public override IDisposable Subscribe(IObserver<T> observer)
        {
            var subscription = base.Subscribe(observer);

            if (_lastValue != null)
                observer.OnNext(_lastValue);

            return subscription;
        }

        public override void Next(T item)
        {
            base.Next(item);
            
            // Inform copies
            if (_copies != null && _copies.Any())
                foreach (var copy in _copies)
                    copy.Next(item);

            _lastValue = item;
        }

        public IObservable<T> AsObservable()
        {
            if (_copies == null)
                _copies = new List<Observable<T>>();

            var copiedObs = new Observable<T>();
            _copies.Add(copiedObs);

            return copiedObs;
        }
    }
}
