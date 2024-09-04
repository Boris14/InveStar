using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] int newsScrollCount = 2;
    [SerializeField] float maxPulseShrinkedScale = 0.65f;
    [SerializeField] float maxPulseGrownScale = 1.7f;
    [SerializeField] Vector2 pulseMoneyChangeRange = new Vector2(200, 800);
    [SerializeField] float pusleDuration = 0.1f;

    [SerializeField] TMP_Text moneyText;
    [SerializeField] TMP_Text capitalText;
    [SerializeField] TMP_Text stocksText;

    // 0 - Left, 1 - Mid, 2 - Right
    CompanyWidget[] companyWidgets;
    NewsAutoScrollField newsScroll;
    CanvasGroup companySelectionGroup;
    CanvasGroup gameplayScreenGroup;
    WindowGraph windowGraph;
    TMP_Text companyTitleText;

    Dictionary<Transform, Coroutine> pusleCoroutines = new Dictionary<Transform, Coroutine>();

    public delegate void OnButtonClickedDelegate();
    public delegate void OnCompanySelectedDelegate(string companyName);

    // public delegate void OnStockChangeDelegate();

    public OnCompanySelectedDelegate onCompanySelectedDelegate;
    public OnButtonClickedDelegate onBuyButtonClickedDelegate;
    public OnButtonClickedDelegate onSellButtonClickedDelegate;
    // public OnStockChangeDelegate onStockChangeDelegate;

    float money = 0;
    float capital = 0;

    // Start is called before the first frame update
    void Start()
    {
        CanvasGroup[] canvasGroups = GetComponentsInChildren<CanvasGroup>();
        companySelectionGroup = GameObject.FindGameObjectWithTag("CompanySelection").GetComponent<CanvasGroup>();
        gameplayScreenGroup = GameObject.FindGameObjectWithTag("GameplayScreen").GetComponent<CanvasGroup>();

        companyWidgets = companySelectionGroup.GetComponentsInChildren<CompanyWidget>();
        for (int i = 0; i < companyWidgets.Length; i++)
        {
            companyWidgets[i].onSelectedDelegate = OnCompanySelected;
        }

        companyTitleText = gameplayScreenGroup.transform.Find("CompanyTitle").GetComponent<TMP_Text>();
        newsScroll = gameplayScreenGroup.GetComponentInChildren<NewsAutoScrollField>();
        windowGraph = gameplayScreenGroup.GetComponentInChildren<WindowGraph>();

        windowGraph.onBuyButtonClickedDelegate = OnBuyButtonClicked;
        windowGraph.onSellButtonClickedDelegate = OnSellButtonClicked;

        companySelectionGroup.gameObject.SetActive(false);
        gameplayScreenGroup.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBuyButtonClicked()
    {
        onBuyButtonClickedDelegate();
    }

    public void OnSellButtonClicked()
    {
        onSellButtonClickedDelegate();
    }

    public void ActivateCompanySelection(Company[] companies)
    {
        companySelectionGroup.gameObject.SetActive(true);
        for (int i = 0; i < companyWidgets.Length; i++)
        {
            companyWidgets[i].Initialize(companies[i].name, companies[i].type, companies[i].description);
        }
    }

    public void SetActionButtonsText(float buyPrice, float sellPrice)
    {
        string buyText = "Buy\n(" + Math.Floor(buyPrice) + "$)";
        string sellText = "Sell\n(" + Math.Floor(sellPrice) + "$)";

        windowGraph.SetActionButtonsText(buyText, sellText);
    }

    public void SetCompanyTitle(string companyName, CompanyType type)
    {
        companyTitleText.text = companyName + " (" + type.ToString() + ")";
    }

    public float ShowNews(News news)
    {
        return newsScroll.ActivateScroll(news.content, newsScrollCount);
    }

    public void SetMoneyDisplayText(float inMoney, float inCapital, float stocks, float stockPrice)
    {
        moneyText.text = "Money:\n" + inMoney.ToString("F2") + "$";
        capitalText.text = "Capital:\n" + inCapital.ToString("F2") + "$";
        stocksText.text = "Stocks:\n" + stocks.ToString("F1") + "\n(x " + stockPrice.ToString("F0") + "$)";

        PlayPulseAnimation(moneyText, inMoney - money);
        PlayPulseAnimation(capitalText, inCapital - capital);

        money = inMoney;
        capital = inCapital;
    }

    public void AddGraphValue(int amount)
    {
        windowGraph.AddNextStockValue(amount);
    }

    public void PlayPulseAnimation(TMP_Text textToPusle, float changedAmount)
    {
        changedAmount = Math.Clamp(changedAmount, -pulseMoneyChangeRange.y, pulseMoneyChangeRange.y);

        if(Math.Abs(changedAmount) < pulseMoneyChangeRange.x)
        {
            return;
        }

        float pulseEndScale = changedAmount < 0 ? Utilities.Map(changedAmount, -pulseMoneyChangeRange.y, 0, maxPulseShrinkedScale, 1) :
            Utilities.Map(changedAmount, 0, pulseMoneyChangeRange.y, 1, maxPulseGrownScale);

        Transform textTransform = textToPusle.transform;
        if(pusleCoroutines.ContainsKey(textTransform) && pusleCoroutines[textTransform] != null)
        {
            StopCoroutine(pusleCoroutines[textTransform]);
            textTransform.localScale = Vector3.one;
        }

        pusleCoroutines[textTransform] = StartCoroutine(Pusle(textTransform, pulseEndScale, pusleDuration));
    }

    IEnumerator Pusle(Transform textTransform, float endScale, float duration)
    {
        float updateRate = 0.02f;
        float originalScale = textTransform.localScale.x;
        float scaleChangePerTick = 2 * Math.Abs(originalScale - endScale) / (duration / updateRate);
        float halfDuration = duration / 2.0f;

        bool isShrinking = endScale < originalScale;
        bool hasReachedEndScale = false;

        while(duration > 0)
        {
            textTransform.localScale += Vector3.one * scaleChangePerTick * (isShrinking ? -1 : 1);

            if(hasReachedEndScale == false && duration <= halfDuration)
            {
                isShrinking = !isShrinking;
                hasReachedEndScale = true;
            }

            duration -= updateRate;
            yield return new WaitForSeconds(updateRate);
        }

        textTransform.localScale = Vector3.one * originalScale;
    }

    void OnCompanySelected(string companyName)
    {
        onCompanySelectedDelegate(companyName);

        companySelectionGroup.gameObject.SetActive(false);
        gameplayScreenGroup.gameObject.SetActive(true);
    }
}
