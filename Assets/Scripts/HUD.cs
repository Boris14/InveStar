using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] int newsScrollCount = 2;
    [SerializeField] TMP_Text moneyText;
    [SerializeField] TMP_Text capitalText;

    // 0 - Left, 1 - Mid, 2 - Right
    CompanyWidget[] companyWidgets;
    NewsAutoScrollField newsScroll;
    CanvasGroup companySelectionGroup;
    CanvasGroup gameplayScreenGroup;
    WindowGraph windowGraph;

    public delegate void OnCompanySelectedDelegate(string companyName);
    public OnCompanySelectedDelegate onCompanySelectedDelegate;

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

        newsScroll = gameplayScreenGroup.GetComponentInChildren<NewsAutoScrollField>();
        windowGraph = gameplayScreenGroup.GetComponentInChildren<WindowGraph>();

        companySelectionGroup.gameObject.SetActive(false);
        gameplayScreenGroup.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateCompanySelection(Company[] companies)
    {
        companySelectionGroup.gameObject.SetActive(true);
        for (int i = 0; i < companyWidgets.Length; i++)
        {
            companyWidgets[i].Initialize(companies[i].name, companies[i].type, companies[i].description);
        }
    }

    public float ShowNews(News news)
    {
        return newsScroll.ActivateScroll(news.content, newsScrollCount);
    }

    public void setMoneyDisplayText(float money, float capital)
    {
        moneyText.text = "Money:\n" + money.ToString() + "$";
        capitalText.text = "Capital:\n" + capital.ToString() + "$";
    }

    void OnCompanySelected(string companyName)
    {
        onCompanySelectedDelegate(companyName);

        companySelectionGroup.gameObject.SetActive(false);
        gameplayScreenGroup.gameObject.SetActive(true);
    }
}
