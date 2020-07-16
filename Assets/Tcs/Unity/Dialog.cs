using System;
using UniRx;
using UnityEngine;

namespace Tcs.Unity
{
    public class Dialog : MonoBehaviour
    {
        private Subject<object> _onClosed = new Subject<object>();

        public IObservable<object> AfterClosed() => _onClosed.AsObservable();

        public void Close(object data = null)
        {
            _onClosed.OnNext(data);
            _onClosed.OnCompleted();
        }
    }
}
