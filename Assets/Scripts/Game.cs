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
    [SerializeField] public float stockPrice;

    public Company(string name, CompanyType type, string description, float stockPrice)
    {
        this.name = name;
        this.type = type;
        this.description = description;
        this.stockPrice = stockPrice;
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
    [SerializeField] Company[] companies;
    [SerializeField] News[] news;
    [SerializeField] float initialNewsDelay = 4;
    [SerializeField] float initialCapital = 5000;
    [SerializeField] float initialMoney = 5000;
    [SerializeField] float maxPriceDeviationPercentNormal = 5;
    [SerializeField] Vector2 priceDeviationPercentPerAction;
    [SerializeField] float changePriceTimerRate = 2;
    [SerializeField] float actionPrice = 300;
    [SerializeField] int maxCombo = 5;

    HUD hud;

    bool isGameStarted = false;
    
    float timeUntilNextNews = 0;
    float timeUntilPriceChange = 0;

    float maxPriceDeviationNormal = 0;
    Vector2 priceDeviationPerAction;

    Company currentCompany;
    News currentNews;

    // Invested Money
    float capital = 0;
    
    // Not invested Money
    float money = 0;

    // Capital / Company's Stock Price
    float ownedStocks = 0;

    int combo = 0;

    // Start is called before the first frame update
    void Start()
    {
        hud = GetComponentInChildren<HUD>();
        hud.onCompanySelectedDelegate = OnCompanySelected;
        hud.onBuyButtonClickedDelegate = OnBuyButtonClicked;
        hud.onSellButtonClickedDelegate = OnSellButtonClicked;

        money = initialMoney;
        capital = initialCapital;

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

        if (timeUntilPriceChange <= 0)
        {
            ChangeStockPrice(CalculatePriceChange(false));
        }
        else
        {
            timeUntilPriceChange -= Time.deltaTime;
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

    float CalculatePriceChange(bool isActionInitated)
    {
        float riseChance = 0.5f;
     
        // The news influence the rise chance
        if (currentCompany.type == currentNews.type)
        {
            riseChance += currentNews.isGood ? 0.2f : -0.2f;
        }

        float changeAmount = isActionInitated ? Random.Range(priceDeviationPerAction.x, priceDeviationPerAction.y) :
            Random.Range(0, maxPriceDeviationNormal);
        
        float result = (Random.value < riseChance ? 1 : -1) * changeAmount;
        return result;
    }

    void ChangeStockPrice(float delta)
    {
        currentCompany.stockPrice += delta;

        capital = ownedStocks * currentCompany.stockPrice;

        timeUntilPriceChange = changePriceTimerRate;

        UpdateMoneyUI();
    }

    void ChangeOwnedStocks(float delta, bool isGood)
    {
        ownedStocks += delta / currentCompany.stockPrice + Utilities.Map(combo, 0, maxCombo, 0, 5);
        money -= delta;

        combo = Math.Min(isGood ? combo + 1 : 0, maxCombo);

        UpdateMoneyUI();
    }

    void OnBuyButtonClicked()
    {
        if(money <= 0)
        {
            return;
        }

        float boughtAmount = money > actionPrice ? actionPrice : money;
        float priceChange = CalculatePriceChange(true);
        
        ChangeOwnedStocks(boughtAmount, priceChange > 0);
        ChangeStockPrice(priceChange);
    }

    void OnSellButtonClicked()
    {
        if (capital <= 0)
        {
            return;
        }

        float soldAmount = capital > actionPrice ? actionPrice : capital;
        float priceChange = CalculatePriceChange(true);

        ChangeOwnedStocks(-soldAmount, priceChange < 0);
        ChangeStockPrice(priceChange);
    }

    void UpdateMoneyUI()
    {
        float stocks = capital / currentCompany.stockPrice;

        hud.SetMoneyDisplayText(money, capital, stocks, currentCompany.stockPrice);
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

                maxPriceDeviationNormal = company.stockPrice * maxPriceDeviationPercentNormal / 100;
                priceDeviationPerAction = company.stockPrice * priceDeviationPercentPerAction / 100;

                ownedStocks = initialCapital / company.stockPrice;

                timeUntilNextNews = initialNewsDelay;
                timeUntilPriceChange = changePriceTimerRate;
                
                isGameStarted = true;

                UpdateMoneyUI();
                hud.SetCompanyTitle(company.name, company.type);

                return;
            }
        }
    }
}
