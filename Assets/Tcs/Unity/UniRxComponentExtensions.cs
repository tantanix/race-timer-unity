using System;
using TMPro;
using UniRx;

namespace Tcs.Unity
{
    public static class UniRxComponentExtensions
    {
        public static IObservable<int> OnTmpValueChangedAsObservable(this TMP_Dropdown dropdown)
        {
            return Observable.CreateWithState<int, TMP_Dropdown>(dropdown, (d, observer) =>
            {
                observer.OnNext(d.value);
                return d.onValueChanged.AsObservable().Subscribe(observer);
            });
        }
    }
}
