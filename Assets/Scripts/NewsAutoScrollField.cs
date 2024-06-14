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
    protected TMP_Text newsText;
    protected RectTransform rectContainer;

    [SerializeField]
    int wrappedDistance = 6;
    [SerializeField]
    float scrollSpeed = 1.0f;
    float scrollTimeLeft = 0.0f;
    int lettersLeft = 0;
    string currentNewsContent;

    // Start is called before the first frame update
    void Start()
    {
        rectContainer = GetComponent<RectTransform>();
        newsText = GetComponent<TMP_Text>();
        currentNewsContent = newsContents[Random.Range(0, newsContents.Length)];
        newsText.text = currentNewsContent;
        scrollTimeLeft = 1 / scrollSpeed;

        // Force the TextMeshPro to update its layout
        newsText.ForceMeshUpdate();

        // Get the rendered bounds of the text
        var textBounds = newsText.textBounds;

        // Adjust the RectTransform size to match the text bounds
        rectContainer.sizeDelta = new Vector2(textBounds.size.x, textBounds.size.y);
    }

    // Update is called once per frame
    void Update()
    {
        scrollTimeLeft -= Time.deltaTime;
        if(scrollTimeLeft <= 0.0f)
        {
            char removedLetter = newsText.text[0];
            newsText.text = newsText.text.Remove(0, 1);
            if(lettersLeft <= 0)
            {
                for(int i = 0; i < wrappedDistance; ++i)
                {
                    newsText.text += " ";
                }
                lettersLeft = currentNewsContent.Length;
            }
            newsText.text += removedLetter;

            scrollTimeLeft = 1.0f / scrollSpeed;
        }
    }
}
