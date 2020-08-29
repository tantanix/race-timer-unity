using System;
using System.Collections.Generic;
using Tcs.Core;
using Tcs.Unity;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.TMP_InputField;

public class MatInput : MonoBehaviour, IValidate<TMP_InputField>
{
    public string ErrorMessage { get; private set; }
    public bool IsValid { get; private set; }
    public string text { get { return InnerInput.text; } set { InnerInput.text = value; } }

    public Color32 InvalidBgColor = AppColors.FormFieldInvalid;
    public TMP_InputField InnerInput;
    public TMP_Text LabelText;
    public TMP_Text ErrorText;
    public Image Line;
    public ContentType contentType = ContentType.Standard;
    
    private bool _isPristine;
    private Color32 _innerInputImageDefaultColor;
    private Color32 _labelTextDefaultColor;
    private string _currentText;
    private readonly List<Tuple<Func<TMP_InputField, bool>, string>> _validators = new List<Tuple<Func<TMP_InputField, bool>, string>>();

    private void Awake()
    {
        _innerInputImageDefaultColor = Line.color;
        _labelTextDefaultColor = LabelText.color;

        ErrorText.color = InvalidBgColor;
        
        Initialize();

        InnerInput.contentType = contentType;
        InnerInput
            .OnValueChangedAsObservable()
            .TakeUntilDestroy(this)
            .Subscribe(CheckInputValue);
    }

    private void Update()
    {
        if (InnerInput.isFocused || InnerInput.text != "")
        {
            if (!LabelText.isActiveAndEnabled)
            {
                LabelText.gameObject.SetActive(true);
                InnerInput.placeholder.gameObject.SetActive(false);
            }
        }
        else
        {
            if (LabelText.isActiveAndEnabled)
            {
                if (string.IsNullOrEmpty(_currentText))
                    LabelText.gameObject.SetActive(false);

                InnerInput.placeholder.gameObject.SetActive(true);
            }
        }
    }

    public void Initialize()
    {
        ErrorText.gameObject.SetActive(false);
        LabelText.gameObject.SetActive(false);
        InnerInput.placeholder.gameObject.SetActive(true);

        _isPristine = true;
        _currentText = "";
        InnerInput.text = _currentText;
        
        LabelText.color = _labelTextDefaultColor;
        Line.color = _innerInputImageDefaultColor;
    }

    public void AddValidator(Func<TMP_InputField, bool> validator, string errorMessage)
    {
        _validators.Add(Tuple.Create(validator, errorMessage));
    }

    public bool Validate()
    {
        IsValid = true;
        foreach (var validator in _validators)
        {
            IsValid &= validator.Item1(InnerInput);
            if (!IsValid)
            {
                ErrorMessage = validator.Item2;
                ValidityUpdate(false);

                return false;
            }
        }

        ErrorMessage = "";
        ValidityUpdate(true);

        return true;
    }

    private void ValidityUpdate(bool isValid)
    {
        ErrorText.text = ErrorMessage;
        ErrorText.gameObject.SetActive(!isValid);
        LabelText.gameObject.SetActive(!string.IsNullOrEmpty(_currentText));
        LabelText.color = isValid ? _labelTextDefaultColor : InvalidBgColor;
        Line.color = isValid ? _innerInputImageDefaultColor : InvalidBgColor;
    }

    private void CheckInputValue(string value)
    {
        _currentText = value;

        if (!_isPristine)
            Validate();

        _isPristine = false;
    }

}
