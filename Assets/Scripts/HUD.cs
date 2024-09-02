using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] int newsScrollCount = 2;
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

    public delegate void OnButtonClickedDelegate();
    public delegate void OnCompanySelectedDelegate(string companyName);

    public OnCompanySelectedDelegate onCompanySelectedDelegate;
    public OnButtonClickedDelegate onBuyButtonClickedDelegate;
    public OnButtonClickedDelegate onSellButtonClickedDelegate;

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

    public void SetCompanyTitle(string companyName, CompanyType type)
    {
        companyTitleText.text = companyName + " (" + type.ToString() + ")";
    }

    public float ShowNews(News news)
    {
        return newsScroll.ActivateScroll(news.content, newsScrollCount);
    }

    public void SetMoneyDisplayText(float money, float capital, float stocks, float stockPrice)
    {
        moneyText.text = "Money:\n" + money.ToString("F2") + "$";
        capitalText.text = "Capital:\n" + capital.ToString("F2") + "$";
        stocksText.text = "Stocks:\n" + stocks.ToString("F1") + "\n(x " + stockPrice.ToString("F0") + "$)";
    }

    void OnCompanySelected(string companyName)
    {
        onCompanySelectedDelegate(companyName);

        companySelectionGroup.gameObject.SetActive(false);
        gameplayScreenGroup.gameObject.SetActive(true);
    }
}
