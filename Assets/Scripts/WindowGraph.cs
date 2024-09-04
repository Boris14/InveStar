using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class WindowGraph : MonoBehaviour
{
    private static WindowGraph instance;
    [SerializeField] private Sprite dotSprite;

    [SerializeField] private AudioSource src;
    [SerializeField] private AudioClip click;
    [SerializeField] private AudioClip moneySound;
    [SerializeField] private AudioClip ambiance;

    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    private List<GameObject> gameObjectList;
    private GameObject tooltipGameObject;
    private TMP_Text buyButtonText;
    private TMP_Text sellButtonText;

    //Graph memorized values
    private List<int> valueList;
    private IGraphVisual graphVisual;
    private int maxVisibleValueAmount;
    private Func<int, string> getAxisLabelX;
    private Func<float, string> getAxisLabelY;

    private bool useBarChart = true;
    IGraphVisual lineGraphVisual;
    IGraphVisual barChartVisual;

    #region SFX

    private void BtnClick()
    {
        src.clip = click;
        src.Play();
    }

    private void MoneySound()
    {
        src.clip = moneySound;
        src.Play();
    }

    // private void MoneySound()
    // {
    //     src.clip = moneySound;
    //     src.Play();
    //     
    // }

    #endregion

    public delegate void OnButtonClickedDelegate();

    public OnButtonClickedDelegate onBuyButtonClickedDelegate;

    public OnButtonClickedDelegate onSellButtonClickedDelegate;

    private void Awake()
    {
        instance = this;
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("LabelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("LabelTemplateY").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("DashTemplateX").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("DashTemplateY").GetComponent<RectTransform>();
        gameObjectList = new List<GameObject>();

        lineGraphVisual = new LineGraphVisual(graphContainer, dotSprite, Color.green, new Color(1, .2f, .2f, 5f));
        barChartVisual = new BarChartVisual(graphContainer, Color.blue, .8f);
        tooltipGameObject = graphContainer.Find("Tooltip").gameObject;
        // InvokeRepeating("Repeat", 2f, 2f);

        valueList = new List<int>() { 5, 99, 32, -18, 64, 50, 40, 30, 25, 22, 20, 25, 35, 40 };
        // valueList = new List<int>() { 5 };
        ShowGraph(valueList, barChartVisual, -1, (int _i) => "D " + (_i + 1), (float _f) => "$" + Mathf.RoundToInt(_f));

        #region ChartBtn

        transform.Find("BarChartBtn").GetComponent<ButtonUI>().ClickFunc = () =>
        {
            Debug.Log("BarChart");
            SetGraphVisual(barChartVisual);
        };
        transform.Find("BarChartBtn").GetComponent<ButtonUI>().ClickFunc += BtnClick;

        #endregion

        #region LineGrapgBtn

        transform.Find("LineGraphBtn").GetComponent<ButtonUI>().ClickFunc = () =>
        {
            Debug.Log("GraphChart");
            SetGraphVisual(lineGraphVisual);
        };
        transform.Find("LineGraphBtn").GetComponent<ButtonUI>().ClickFunc += BtnClick;

        #endregion

        #region DecreaseBtn

        transform.Find("DecreaseVisibleAmountBtn").GetComponent<ButtonUI>().ClickFunc = () =>
        {
            Debug.Log("Increase");
            DecreaseVisibleAmount();
        };

        transform.Find("DecreaseVisibleAmountBtn").GetComponent<ButtonUI>().ClickFunc += BtnClick;

        #endregion

        #region IncreaseBtn

        transform.Find("IncreaseVisibleAmountBtn").GetComponent<ButtonUI>().ClickFunc = () =>
        {
            Debug.Log("Decrease");
            IncreaseVisibleAmount();
        };
        transform.Find("IncreaseVisibleAmountBtn").GetComponent<ButtonUI>().ClickFunc += BtnClick;

        #endregion

        #region BuyBtn

        Transform buyButtonTransform = transform.Find("BuyBtn");
        buyButtonTransform.GetComponent<ButtonUI>().ClickFunc = OnBuyButtonClicked;
        buyButtonText = buyButtonTransform.GetComponentInChildren<TMP_Text>();

        buyButtonTransform.GetComponent<ButtonUI>().ClickFunc += MoneySound;

        #endregion

        #region SellBtn

        Transform sellButtonTransform = transform.Find("SellBtn");
        sellButtonTransform.GetComponent<ButtonUI>().ClickFunc = OnSellButtonClicked;
        sellButtonText = sellButtonTransform.GetComponentInChildren<TMP_Text>();

        sellButtonTransform.GetComponent<ButtonUI>().ClickFunc += MoneySound;

        #endregion

        // onStockChangeDelegate
        //ShowTooltip("this is tooltip", new Vector2(100, 100));
    }

    private void Update()
    {
        return;
    }

    private void Repeat()
    {
        // if (useBarChart)
        // {
        //     Debug.Log("BAR");
        //     ShowGraph(valueList, barChartVisual, -1, (int _i) => "D" + (_i + 1),
        //         (float _f) => "$" + Mathf.RoundToInt(_f));
        // }
        // else
        // {
        //     Debug.Log("LINE");
        //     ShowGraph(valueList, lineGraphVisual, -1, (int _i) => "D" + (_i + 1),
        //         (float _f) => "$" + Mathf.RoundToInt(_f));
        // }
        //
        // useBarChart = !useBarChart;
        valueList.Clear();
        for (int i = 0; i < 15; i++)
        {
            valueList.Add(UnityEngine.Random.Range(0, 400));
        }

        ShowGraph(valueList, graphVisual, -1, (int _i) => "D" + (_i + 1), (float _f) => "$" + Mathf.RoundToInt(_f));
    }

    public static void ShowTooltip_Static(string tooltipTxt, Vector2 anchoredPosition)
    {
        instance.ShowTooltip(tooltipTxt, anchoredPosition);
    }

    public void SetActionButtonsText(string buyBtnText, string sellBtnText)
    {
        buyButtonText.text = buyBtnText;
        sellButtonText.text = sellBtnText;
    }

    private void ShowTooltip(string tooltipTxt, Vector2 anchoredPosition)
    {
        tooltipGameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

        tooltipGameObject.SetActive(true);
        Text tooltipUITxt = tooltipGameObject.transform.Find("Text").GetComponent<Text>();
        tooltipUITxt.text = tooltipTxt;

        float textPaddingSize = 4f;
        Vector2 backgroundSize =
            new Vector2(tooltipUITxt.preferredWidth + textPaddingSize * 2f,
                tooltipUITxt.preferredHeight + textPaddingSize * 2f);
        tooltipGameObject.transform.Find("Background").GetComponent<RectTransform>().sizeDelta = backgroundSize;

        tooltipGameObject.transform.SetAsLastSibling();
    }

    private void HideTooltip()
    {
        tooltipGameObject.SetActive(false);
    }

    public void AddNextStockValue(int value)
    {
        valueList.Add(value);
        Debug.Log(valueList.Count + " " + maxVisibleValueAmount);
        ShowGraph(this.valueList, this.graphVisual, -1, this.getAxisLabelX,
            this.getAxisLabelY);

        if (maxVisibleValueAmount < valueList.Count)
        {
            valueList.RemoveAt(0);
        }
    }

    //Can change the labels of the graph
    private void SetGetAxisLabelX(Func<int, string> getAxisLabelX)
    {
        ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount, getAxisLabelX, this.getAxisLabelY);
    }

    private void SetGetAxisLabelY(Func<float, string> getAxisLabelY)
    {
        ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount, this.getAxisLabelX, getAxisLabelY);
    }

    private void IncreaseVisibleAmount()
    {
        ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount + 1, this.getAxisLabelX,
            this.getAxisLabelY);
    }

    private void DecreaseVisibleAmount()
    {
        ShowGraph(this.valueList, this.graphVisual, this.maxVisibleValueAmount - 1, this.getAxisLabelX,
            this.getAxisLabelY);
    }

    private void SetGraphVisual(IGraphVisual _graphVisual)
    {
        ShowGraph(this.valueList, _graphVisual, this.maxVisibleValueAmount, this.getAxisLabelX, this.getAxisLabelY);
    }

    void OnBuyButtonClicked()
    {
        onBuyButtonClickedDelegate();
    }

    void OnSellButtonClicked()
    {
        onSellButtonClickedDelegate();
    }

    //if you want to show the all numbers maxVisibleValueAmount should be < 0
    private void ShowGraph(List<int> valueList, IGraphVisual graphVisual, int maxVisibleValueAmount = -1,
        Func<int, string> getAxisLabelX = null,
        Func<float, string> getAxisLabelY = null)
    {
        this.valueList = valueList;
        this.graphVisual = graphVisual;
        this.getAxisLabelX = getAxisLabelX;
        this.getAxisLabelY = getAxisLabelY;

        //Validate maxVisibleAmount
        if (maxVisibleValueAmount <= 0)
        {
            maxVisibleValueAmount = valueList.Count;
        }

        if (maxVisibleValueAmount > valueList.Count)
        {
            maxVisibleValueAmount = valueList.Count;
        }

        // this.maxVisibleValueAmount = maxVisibleValueAmount;

        if (getAxisLabelX == null)
        {
            getAxisLabelX = delegate(int _i) { return _i.ToString(); };
        }

        if (getAxisLabelY == null)
        {
            getAxisLabelY = delegate(float _f) { return Mathf.RoundToInt(_f).ToString(); };
        }

        foreach (var gameObject in gameObjectList)
        {
            Destroy(gameObject);
        }

        gameObjectList.Clear();

        float graphHeight = graphContainer.sizeDelta.y;
        float graphWidth = graphContainer.sizeDelta.x;

        float yMaximum = valueList[0]; //money
        float yMinimum = valueList[0];
        float xSize = graphWidth / (maxVisibleValueAmount + 1); // time

        int xIndex = 0;

        //add dynamic y axis
        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++)
        {
            int value = valueList[i];
            if (value > yMaximum)
            {
                yMaximum = value;
            }

            if (value < yMinimum)
            {
                yMinimum = value;
            }
        }

        float yDiff = yMaximum - yMinimum;
        if (yDiff <= 0)
        {
            yDiff = 5f;
        }

        yMaximum = yMaximum + (yDiff * 0.2f); // adds top buffer
        yMinimum = yMinimum - (yDiff * 0.2f);

        // GameObject lastDotGameObject = null;
        // LineGraphVisual lineGraphVisual = new LineGraphVisual(graphContainer, dotSprite, Color.green, new Color(1,.2f,.2f,5f));
        // BarChartVisual barChartVisual = new BarChartVisual(graphContainer, Color.red, .8f);

        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount - 1, 0); i < valueList.Count; i++)
        {
            float xPosition = xIndex * xSize + xSize;
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;
            // GameObject barGameObject = CreateBar(new Vector2(xPosition,yPosition),xSize * .9f);
            // gameObjectList.AddRange(barChartVisual.AddGraphVisual(new Vector2(xPosition, yPosition), xSize));
            string tooltipTxt = getAxisLabelY(valueList[i]);
            // Debug.Log("Draw: " + i + " " + xPosition + "," + yPosition);
            if (i == Mathf.Max(valueList.Count - maxVisibleValueAmount - 1, 0))
            {
                gameObjectList.AddRange(graphVisual.AddGraphVisual(new Vector2(xPosition, yPosition), xSize, true));
            }
            else
            {
                gameObjectList.AddRange(graphVisual.AddGraphVisual(new Vector2(xPosition, yPosition), xSize, false));
            }

            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer);
            labelX.localScale = new Vector3(1, 1, 1);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -7f);
            labelX.GetComponent<Text>().text = getAxisLabelX(i);
            gameObjectList.Add(labelX.gameObject);

            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer);
            dashX.localScale = new Vector3(1, 1, 1);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2(xPosition, -7f);
            gameObjectList.Add(dashX.gameObject);

            xIndex++;
        }

        int seperatorCount = 10;
        for (int i = 0; i <= seperatorCount; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer);
            labelY.localScale = new Vector3(1, 1, 1);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / seperatorCount;
            labelY.anchoredPosition = new Vector2(-7f, normalizedValue * graphHeight);
            labelY.GetComponent<Text>().text = getAxisLabelY(yMinimum + normalizedValue * (yMaximum - yMinimum));
            gameObjectList.Add(labelY.gameObject);

            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer);
            dashY.localScale = new Vector3(1, 1, 1);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(-4, normalizedValue * graphHeight);
            gameObjectList.Add(dashY.gameObject);
        }
    }

    //homemade function - might have issues
    public static float GetNormalizedVectorAngle(Vector2 vector)
    {
        // Calculate the magnitude of the vector
        float magnitude = vector.magnitude;

        if (magnitude == 0)
            return 0;

        // Calculate the angle in radians using atan2
        float angleRadians = Mathf.Atan2(vector.y, vector.x);

        // Convert the angle from radians to degrees
        float angleDegrees = angleRadians * (180 / Mathf.PI);

        if (vector.y < 0)
            angleDegrees += 180;
        return Mathf.Abs(angleDegrees % 360);
    }


    private interface IGraphVisual
    {
        public List<GameObject> AddGraphVisual(Vector2 graphPosition, float graphPositionWidth, bool isLast);
    }

    private class BarChartVisual : IGraphVisual
    {
        private RectTransform graphContainer;
        private Color barColor;
        private float barWidthMultiplier;
        private Vector2 lastGraphPosition;

        public BarChartVisual(RectTransform graphContainer, Color barColor, float barWidthMultiplier)
        {
            this.graphContainer = graphContainer;
            this.barColor = barColor;
            this.barWidthMultiplier = barWidthMultiplier;
            this.lastGraphPosition = new Vector2(0,0);
        }

        public List<GameObject> AddGraphVisual(Vector2 graphPosition, float graphPositionWidth, bool isLast)
        {
            GameObject barGameObject = CreateBar(graphPosition, graphPositionWidth);
            // barGameObject.AddComponent<ButtonUI>().MouseDownOnceFunc += () =>
            // {
            //     ShowTooltip_Static(tooltipTxt, graphPosition);
            //     // ShowTooltip(tooltipTxt);
            // };
            return new List<GameObject>() { barGameObject };
        }

        private GameObject CreateBar(Vector2 graphPosition, float barWidth)
        {
            GameObject gameObject = new GameObject("bar", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            if (graphPosition.y > lastGraphPosition.y)
            {
                //profit
                gameObject.GetComponent<Image>().color = Color.green;
            }else if (graphPosition.y < lastGraphPosition.y)
            {
                //loss
                gameObject.GetComponent<Image>().color = Color.red;
            }
            else
            {
                gameObject.GetComponent<Image>().color = barColor;
            }
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(graphPosition.x, 0f);
            rectTransform.sizeDelta = new Vector2(barWidth * barWidthMultiplier, graphPosition.y);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(.5f, 0f);

            this.lastGraphPosition = graphPosition;
            return gameObject;
        }
    }


    private class LineGraphVisual : IGraphVisual
    {
        private RectTransform graphContainer;
        private Sprite dotSprite;
        private GameObject lastDotGameObject;
        private Color dotColor;
        private Color dotConnectionColor;

        public LineGraphVisual(RectTransform graphContainer, Sprite dotSprite, Color dotColor, Color dotConnectionColor)
        {
            this.graphContainer = graphContainer;
            this.dotSprite = dotSprite;
            lastDotGameObject = null;
            this.dotColor = dotColor;
            this.dotConnectionColor = dotConnectionColor;
        }

        private GameObject CreateDot(Vector2 anchoredPosition)
        {
            GameObject gameObject = new GameObject("dot", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().sprite = dotSprite;
            gameObject.GetComponent<Image>().color = dotColor;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(11, 11);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            return gameObject;
        }

        private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
        {
            GameObject gameObject = new GameObject("dotConnection", typeof(Image));
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().color = dotConnectionColor;
            Vector2 dir = (dotPositionB - dotPositionA).normalized;
            float distance = Vector2.Distance(dotPositionA, dotPositionB);
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(distance, 3f);
            // Debug.Log("Vector" + dir);
            rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
            rectTransform.localEulerAngles = new Vector3(0, 0, GetNormalizedVectorAngle(dir));
            return gameObject;
        }

        public List<GameObject> AddGraphVisual(Vector2 graphPosition, float graphPositionWidth, bool isLast)
        {
            if (isLast)
            {
                lastDotGameObject = null;
            }

            List<GameObject> gameObjectList = new List<GameObject>();
            GameObject dotGameObject = CreateDot(graphPosition);
            gameObjectList.Add(dotGameObject);
            if (lastDotGameObject != null)
            {
                GameObject dotConnectionGameObject = CreateDotConnection(
                    lastDotGameObject.GetComponent<RectTransform>().anchoredPosition,
                    dotGameObject.GetComponent<RectTransform>().anchoredPosition);
                gameObjectList.Add(dotConnectionGameObject);
            }

            lastDotGameObject = dotGameObject;
            // Debug.Log("AddGraph 1: " + " " + lastDotGameObject.GetComponent<RectTransform>().anchoredPosition);
            // Debug.Log("AddGraph 2: " + " " + dotGameObject.GetComponent<RectTransform>().anchoredPosition);


            return gameObjectList;
        }
    }
}