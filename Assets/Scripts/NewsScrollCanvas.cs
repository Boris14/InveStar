using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewsScrollCanvas : MonoBehaviour
{
    Canvas canvas;
    [SerializeField]
    GameObject newsScroll;
    GameObject newsGameObject = null;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();
        //newsScroll.GetComponent<NewsAutoScrollField>().onTextContentsDisplayed = OnNewsContentsDisplayed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnNewsContentsDisplayed()
    {
        newsGameObject = Instantiate(newsScroll);
        newsGameObject.transform.SetParent(canvas.transform);
        newsScroll.GetComponent<NewsAutoScrollField>().onTextContentsDisplayed -= OnNewsContentsDisplayed;
    }
}
