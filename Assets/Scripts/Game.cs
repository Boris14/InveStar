using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] float initialBuyPrice = 300;
    [SerializeField] float initialSellPrice = 300;

    [SerializeField] float maxPriceDeviationPercentNormal = 5;
    [SerializeField] Vector2 priceDeviationPercentPerAction;
    [SerializeField] Vector2 companyStockPriceLimits = new Vector2(0, 1000);
    [SerializeField] float changePriceTimerRate = 2;
    [SerializeField] int maxCombo = 5;

    // Stores all the UI (what's shown to the Player)
    HUD hud;

    bool isGameStarted = false;

    float timeUntilNextNews = 0;
    float timeUntilPriceChange = 0;

    // The maximum change in the Company's Stock Price without a Player's action
    float maxPriceDeviationNormal = 0;

    // The minimum and maximum Stock Price changes when a Player performs an action (Buy/Sell)
    Vector2 priceDeviationPerAction;

    // The Company which the Player has invested in
    Company currentCompany;

    // The currently scrolling News
    News currentNews;

    // Invested Money
    float capital = 0;
    
    // Not invested Money
    float money = 0;

    // Capital / Company's Stock Price
    float ownedStocks = 0;

    float buyPrice = 0;
    float sellPrice = 0;
    int combo = 0;

    void Start()
    {
        hud = GetComponentInChildren<HUD>();
        hud.onCompanySelectedDelegate = OnCompanySelected;
        hud.onBuyButtonClickedDelegate = OnBuyButtonClicked;
        hud.onSellButtonClickedDelegate = OnSellButtonClicked;

        money = initialMoney;
        capital = initialCapital;
        buyPrice = initialBuyPrice;
        sellPrice = initialSellPrice;

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

        CheckCondition();
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
        currentCompany.stockPrice = Math.Clamp(currentCompany.stockPrice + delta, companyStockPriceLimits.x, companyStockPriceLimits.y);

        capital = ownedStocks * currentCompany.stockPrice;

        timeUntilPriceChange = changePriceTimerRate;

        hud.AddGraphValue((int)currentCompany.stockPrice);
        UpdateMoneyUI();
    }

    void ChangeOwnedStocks(float delta, bool isGood)
    {
        if(currentCompany.stockPrice <= 0)
        {
            return;
        }
    
        capital += delta;
        money -= delta;

        ownedStocks = capital / currentCompany.stockPrice; //+ Utilities.Map(combo, 0, maxCombo, 0, 5);

        combo = Math.Min(isGood ? combo + 1 : 0, maxCombo);
    }

    void OnBuyButtonClicked()
    {
        if(money <= 0)
        {
            return;
        }

        float boughtAmount = money > buyPrice ? buyPrice : money;
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

        float soldAmount = capital > sellPrice ? sellPrice : capital;
        float priceChange = CalculatePriceChange(true);

        ChangeOwnedStocks(-soldAmount, priceChange < 0);
        ChangeStockPrice(priceChange);
    }

    void UpdateMoneyUI()
    {
        hud.SetMoneyDisplayText(money, capital, ownedStocks, currentCompany.stockPrice);

        float possibleBuyPrice = money > buyPrice ? buyPrice : money;
        float possibleSellPrice = capital > sellPrice ? sellPrice : capital;

        hud.SetActionButtonsText(possibleBuyPrice, possibleSellPrice);
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

    public void CheckCondition()
    {
        if (capital <= 0 && money <= 0)
        {
            SceneManager.LoadScene("LostScene");
        }else if (money >= 50_000)
        {
            Debug.Log("WON!");
            SceneManager.LoadScene("WinScene");
        }
    }
}
