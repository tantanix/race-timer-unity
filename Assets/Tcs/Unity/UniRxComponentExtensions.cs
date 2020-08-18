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

        public static IObservable<string> OnValueChangedAsObservable(this TMP_InputField inputField)
        {
            return Observable.CreateWithState<string, TMP_InputField>(inputField, (i, observer) =>
            {
                observer.OnNext(i.text);
                return i.onValueChanged.AsObservable().Subscribe(observer);
            });
        }

        public static IObservable<TimeSpan?> OnValueChangedAsObservable(this TimeOfDayInput timeOfDay)
        {
            return Observable.CreateWithState<TimeSpan?, TimeOfDayInput>(timeOfDay, (i, observer) =>
            {
                observer.OnNext(i.CurrentTimeSpan);
                return i.OnValueChanged.AsObservable().Subscribe(observer);
            });
        }
    }
}
