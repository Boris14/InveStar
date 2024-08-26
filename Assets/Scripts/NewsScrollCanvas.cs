using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewsScrollCanvas : MonoBehaviour
{
    NewsAutoScrollField newsScroll;

    protected string[] newsContents =
    {
        "PetroPros discovers large oil field in Gulf of Mexico, significantly boosting reserves.",
        "FuelMaster Inc faces fines and cleanup costs after pipeline leak causes environmental damage.",
        "CleanLiving recalls cleaning solution due to safety issues, costing millions and harming brand reputation."
    };

    // Start is called before the first frame update
    void Start()
    {
        newsScroll = GetComponentInChildren<NewsAutoScrollField>();
        newsScroll.ActivateScroll(newsContents[Random.Range(0, newsContents.Length)], 3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
