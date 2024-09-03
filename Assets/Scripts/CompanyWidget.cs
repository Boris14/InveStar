using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct CompanyTypeDisplayStruct
{
    [SerializeField] public CompanyType type;
    [SerializeField] public string name;
    [SerializeField] public Color color;

    public CompanyTypeDisplayStruct(CompanyType type, Color color)
    {
        this.type = type;
        this.color = color;
        switch(type)
        {
            case CompanyType.HEALTH:
                name = "HEALTH";
                break;
            case CompanyType.TECH:
                name = "TECH";
                break;
            case CompanyType.ENERGY:
                name = "ENERGY";
                break;
            default:
                name = ""; 
                break;
        }
    }
}

public class CompanyWidget : MonoBehaviour
{
    [SerializeField] public CompanyTypeDisplayStruct[] companyTypesDisplay = new CompanyTypeDisplayStruct[3];

    TMP_Text titleText;
    TMP_Text typeText;
    TMP_Text descriptionText;

    Button button;

    public delegate void OnSelectedDelegate(string companyName);
    public OnSelectedDelegate onSelectedDelegate;

    void Start()
    {
        button = GetComponentInChildren<Button>();
        TMP_Text[] TextComponents = GetComponentsInChildren<TMP_Text>();
        titleText = TextComponents[0];
        typeText = TextComponents[1];
        descriptionText = TextComponents[2];

        button.onClick.AddListener(OnClicked);
    }

    public void Initialize(string Title, CompanyType type, string Description)
    {
        titleText.text = Title;
        descriptionText.text = Description;
        foreach(CompanyTypeDisplayStruct displayData in companyTypesDisplay)
        {
            if(displayData.type == type)
            {
                typeText.text = displayData.name;
                typeText.color = displayData.color;
            }
        }
    }

    void OnClicked()
    {
        onSelectedDelegate(titleText.text);
    }
}
