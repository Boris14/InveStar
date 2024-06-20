using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewsAutoScrollField : MonoBehaviour
{
    protected string[] newsContents =
    {
        "PetroPros discovers large oil field in Gulf of Mexico, significantly boosting reserves.",
        "FuelMaster Inc faces fines and cleanup costs after pipeline leak causes environmental damage.",
        "CleanLiving recalls cleaning solution due to safety issues, costing millions and harming brand reputation."
    };
    public TMP_Text newsText;
    public RectTransform rectContainer;

    [SerializeField]
    float scrollSpeed = 1.0f;

    bool isTextContentsDisplayed = false;

    public delegate void OnTextContentsDisplayed();
    public OnTextContentsDisplayed onTextContentsDisplayed;

    // Start is called before the first frame update
    void Start()
    {
        rectContainer = GetComponent<RectTransform>();
        newsText = GetComponent<TMP_Text>();

        newsText.text = newsContents[Random.Range(0, newsContents.Length)];

        rectContainer.sizeDelta = newsText.GetPreferredValues();

        rectContainer.position = new Vector2(rectContainer.sizeDelta.x * 1.5f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(rectContainer == null)
        {
            Debug.LogError("rectContainer == null");
            return;
        }

        Vector2 newNewsPosition = new Vector2(rectContainer.position.x - Time.deltaTime * scrollSpeed, 0.0f);

        // Check if the text is out of the Screen
        if (rectContainer.position.x < -rectContainer.sizeDelta.x * 0.5f)
        {
            newNewsPosition.x = rectContainer.sizeDelta.x * 1.5f;
            isTextContentsDisplayed = false;
        }
        rectContainer.position = newNewsPosition;
    
        if(!isTextContentsDisplayed && newNewsPosition.x < 0)
        {
            isTextContentsDisplayed = true;
            if(onTextContentsDisplayed != null)
            {
                onTextContentsDisplayed();
            }
        }
    }
}
