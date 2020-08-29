using Tcs.Core;
using System;
using Tcs.Unity;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class MatDropdown : MonoBehaviour, IValidate<TMP_Dropdown>
{
    public string ErrorMessage { get; private set; }
    public bool IsValid { get; private set; }

    public Color32 InvalidBgColor = AppColors.FormFieldInvalid;
    public TMP_Text ErrorText;
    public TMP_Dropdown InnerDropdown;
    public Func<int, bool> Validator;
    
    private readonly List<Tuple<Func<TMP_Dropdown, bool>, string>> _validators = new List<Tuple<Func<TMP_Dropdown, bool>, string>>();

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        ErrorText.gameObject.SetActive(false);
    }

    public void AddValidator(Func<TMP_Dropdown, bool> validator, string errorMessage)
    {
        _validators.Add(Tuple.Create(validator, errorMessage));
    }

    public bool Validate()
    {
        IsValid = true;
        foreach (var validator in _validators)
        {
            IsValid &= validator.Item1(InnerDropdown);
            if (!IsValid)
            {
                ErrorMessage = validator.Item2;
                
                ErrorText.text = ErrorMessage;
                ErrorText.gameObject.SetActive(true);
                
                return false;
            }
        }

        ErrorText.text = "";
        ErrorText.gameObject.SetActive(false);

        return true;
    }
}
