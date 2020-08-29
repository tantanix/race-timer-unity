using System;
using Tcs.Utils;
using TMPro;

namespace Assets.Tcs.Core.Validators
{
    public static class Validators
    {
        public static Func<TMP_InputField, bool> RequiredInputField = (input) => !string.IsNullOrEmpty(input.text) && !string.IsNullOrWhiteSpace(input.text);
        public static Func<TMP_InputField, bool> RequiredAndValidNumberInputField = (input) => int.TryParse(input.text, out int value);
        public static Func<TMP_Dropdown, bool> RequiredDropdown = (dropdown) => dropdown.value != 0;
        public static Func<TMP_InputField, bool> EmailInputField = (input) => RegexUtil.IsValidEmail(input.text);
    }
}
