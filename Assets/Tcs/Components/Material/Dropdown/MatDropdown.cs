using Tcs.Unity;
using TMPro;
using UnityEngine;

public class MatDropdown : MonoBehaviour
{
    public Color32 ValidBgColor = AppColors.FormFieldValid;
    public Color32 InvalidBgColor = AppColors.FormFieldInvalid;

    public TMP_Text ErrorText;
    public TMP_Dropdown InnerDropdown;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        ErrorText.gameObject.SetActive(false);
    }
}
