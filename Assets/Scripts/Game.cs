using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public enum CompanyType
{
    HEALTH,
    TECH,
    ENERGY
}

[Serializable]
public struct Company
{
    [SerializeField] public string name;
    [SerializeField] public CompanyType type;
    [SerializeField] public string description;

    public Company(string name, CompanyType type, string description)
    {
        this.name = name;
        this.type = type;
        this.description = description;
    }
}

[Serializable]
public struct News
{
    [SerializeField] public string content;
    [SerializeField] public CompanyType type;
    [SerializeField] public bool isGood;

    public News(string content, CompanyType type, bool isGood)
    {
        this.content = content;
        this.type = type;
        this.isGood = isGood;
    }
}

public class Game : MonoBehaviour
{
    [SerializeField] NewsAutoScrollField newsScrollObject;
    [SerializeField] Company[] companies;
    [SerializeField] News[] news;

    HUD hud;
    News currentNews;

    bool isGameStarted = false;
    float timeUntilNextNews = 0;

    Company currentCompany;

    // Invested money
    float capital = 0;

    // Not invested money
    float money = 1000;

    // Start is called before the first frame update
    void Start()
    {
        hud = GetComponentInChildren<HUD>();

        hud.onCompanySelectedDelegate = OnCompanySelected;
        Company[] pickedCompanies = Utilities.PickRandomUniqueElements(companies, 3);
        hud.ActivateCompanySelection(pickedCompanies);
        isGameStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isGameStarted == false)
        {
            return;
        }

        if(timeUntilNextNews <= 0)
        {
            timeUntilNextNews = ActivateRandomNews();
        }
        else
        {
            timeUntilNextNews -= Time.deltaTime;
        }
    }

    // Returns the showcase duration of the picked news
    float ActivateRandomNews()
    {
        currentNews = news[Random.Range(0, news.Length)];
        return hud.ShowNews(currentNews);
    }

    public void OnCompanySelected(string companyName)
    {
        foreach(Company company in companies)
        {
            if(company.name == companyName)
            {
                currentCompany = company;
                isGameStarted = true;
                timeUntilNextNews = ActivateRandomNews();
                return;
            }
        }
    }
}
