using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewsAutoScrollField : MonoBehaviour
{
    [SerializeField]
    float scrollSpeed = 1.0f;
    [SerializeField]
    int spaceBetweenSentences = 10;

    TMP_Text newsText;
    RectTransform rectContainer;

    bool isScrollActive = false;

    // Returns the showcase duration of the content for the given scrollsCount
    public float ActivateScroll(string content, int scrollsCount)
    {
        newsText.text = "BREAKING NEWS: ";
        while(scrollsCount > 0)
        {
            newsText.text += content;
            if(scrollsCount > 1)
            {
                newsText.text += new string(' ', spaceBetweenSentences);
            }
            --scrollsCount;
        }

        rectContainer.anchoredPosition = new Vector2(0.0f, 0.0f);
        rectContainer.sizeDelta = newsText.GetPreferredValues();
        isScrollActive = true;

        float timeToScroll = (Mathf.Abs(rectContainer.sizeDelta.x + Screen.width) - rectContainer.anchoredPosition.x) / scrollSpeed;
        return timeToScroll;
    }

    void DeactivateScroll()
    {
        newsText.text = "";
        isScrollActive = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        rectContainer = GetComponent<RectTransform>();
        newsText = GetComponent<TMP_Text>();

        DeactivateScroll();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isScrollActive) 
        {
            return;
        }
        if(rectContainer == null)
        {
            Debug.LogError("rectContainer == null");
            return;
        }

        Vector2 newNewsPosition = new Vector2(rectContainer.anchoredPosition.x - Time.deltaTime * scrollSpeed, 50.0f);

        // Check if the text is out of the Screen
        if (rectContainer.anchoredPosition.x < -(rectContainer.sizeDelta.x + Screen.width))
        {
            DeactivateScroll();
        }
        rectContainer.anchoredPosition = newNewsPosition;
    }
}
