using Tcs.Unity;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.TMP_InputField;

public class MatInput : MonoBehaviour
{
    public Color32 ValidBgColor = AppColors.FormFieldValid;
    public Color32 InvalidBgColor = AppColors.FormFieldInvalid;

    public TMP_InputField InnerInput;
    public TMP_Text LabelText;
    public TMP_Text ErrorText;
    public Image Line;
    public ContentType contentType = ContentType.Standard;
    public bool IsRequired;
    public string text
    {
        get { return InnerInput.text; }
        set { InnerInput.text = value; }
    }

    private bool _isPristine;
    private Color32 _innerInputImageDefaultColor;
    private Color32 _labelTextDefaultColor;
    private string _currentText;

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
        if (InnerInput.isFocused)
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
        _isPristine = true;
        _currentText = "";
        InnerInput.text = _currentText;
        ErrorText.gameObject.SetActive(false);
        LabelText.gameObject.SetActive(false);

        LabelText.color = _labelTextDefaultColor;
        Line.color = _innerInputImageDefaultColor;
    }

    public void Validate()
    {
        var requiredPassed = true;
        if (IsRequired)
        {
            requiredPassed = !string.IsNullOrEmpty(_currentText) && !string.IsNullOrWhiteSpace(_currentText);  
        }

        var isValid = requiredPassed;
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
