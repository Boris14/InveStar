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

[System.Serializable]
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

[System.Serializable]
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

    [SerializeField] int newsScrollCount = 2;

    NewsAutoScrollField newsScroll;
    News currentNews;

    float timeUntilNextNews = 0.5f;

    // Invested money
    float capital = 0;

    // Not invested money
    float money = 1000;

    // Start is called before the first frame update
    void Start()
    {
        newsScroll = GetComponentInChildren<NewsAutoScrollField>();
    }

    // Update is called once per frame
    void Update()
    {
        if(timeUntilNextNews <= 0)
        {
            timeUntilNextNews = activateRandomNews();
        }
        else
        {
            timeUntilNextNews -= Time.deltaTime;
        }
    }

    // Returns the showcase duration of the picked news
    float activateRandomNews()
    {
        currentNews = news[Random.Range(0, news.Length)];
        return newsScroll.ActivateScroll(currentNews.content, newsScrollCount);
    }
}
