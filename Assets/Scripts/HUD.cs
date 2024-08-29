using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] int newsScrollCount = 2;

    // 0 - Left, 1 - Mid, 2 - Right
    CompanyWidget[] companyWidgets;
    NewsAutoScrollField newsScroll;
    CanvasGroup companySelectionGroup;

    public delegate void OnCompanySelectedDelegate(string companyName);
    public OnCompanySelectedDelegate onCompanySelectedDelegate;

    // Start is called before the first frame update
    void Start()
    {
        newsScroll = GetComponentInChildren<NewsAutoScrollField>();
        companyWidgets = GetComponentsInChildren<CompanyWidget>();
        companySelectionGroup = GetComponentInChildren<CanvasGroup>();

        companySelectionGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateCompanySelection(Company[] companies)
    {
        for (int i = 0; i < companyWidgets.Length; i++)
        {
            companyWidgets[i].onSelectedDelegate = OnCompanySelected;
            companyWidgets[i].Initialize(companies[i].name, companies[i].type, companies[i].description);
        }
        companySelectionGroup.alpha = 1;
    }

    public float ShowNews(News news)
    {
        return newsScroll.ActivateScroll(news.content, newsScrollCount);
    }

    void OnCompanySelected(string companyName)
    {
        onCompanySelectedDelegate(companyName);

        companySelectionGroup.alpha = 0;
        companySelectionGroup.interactable = false;
    }
}
