using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
    public List<EmotionTuple> showingEmotions = new List<EmotionTuple>();

    // Tinaxd Drag UI
    public DragUI DragUI;
    private GameObject emotionPanel;

    // Start is called before the first frame update
    void Start()
    {
        // Schin  set TextMesh layer to avoid render layer issue
        transform.Find("WaitTime").GetComponent<MeshRenderer>().sortingLayerName = "UserGUI";
        transform.Find("WaitTime").GetComponent<MeshRenderer>().sortingOrder = 0;
        //rectTransform = GetComponent<RectTransform>();

        // Tinaxd Drag UI
        DragUI = transform.parent.Find("DragUI").GetComponent<DragUI>();
        emotionPanel = transform.Find("EmotionPanel").gameObject;
        emotionPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEmotion();
    }

    private void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    public float HP
    {
        get => transform.Find("HPSlider").GetComponent<Slider>().value;
        set
        {
            transform.Find("HPSlider").GetComponent<Slider>().value = value;
            transform.Find("HPSlider").transform.Find("HPText").GetComponent<Text>().text = value.ToString();
        }
    }

    public float MP
    {
        get => transform.Find("MPSlider").GetComponent<Slider>().value;
        set
        {
            transform.Find("MPSlider").GetComponent<Slider>().value = value;
            transform.Find("MPSlider").transform.Find("MPText").GetComponent<Text>().text = value.ToString();
        }
    }

    public float HPMax
    {
        get => transform.Find("HPSlider").GetComponent<Slider>().maxValue;
        set => transform.Find("HPSlider").GetComponent<Slider>().maxValue = value;
    }

    public float MPMax
    {
        get => transform.Find("MPSlider").GetComponent<Slider>().maxValue;
        set => transform.Find("MPSlider").GetComponent<Slider>().maxValue = value;
    }

    public string WaitTimeText
    {
        set => transform.Find("WaitTime").GetComponent<TextMesh>().text = value;
    }

    public Color WaitTimeColor
    {
        set => transform.Find("WaitTime").GetComponent<TextMesh>().color = value;
        get => transform.Find("WaitTime").GetComponent<TextMesh>().color;
    }

    public bool WaitTimeEnabled
    {
        set => transform.Find("WaitTime").gameObject.SetActive(value);
        get => transform.Find("WaitTime").gameObject.activeSelf;
    }

    // Show Emotion Icon Tinaxd
    public void ShowEmotion(string emotionName, float length)
    {
        /*
        showingEmotions.ForEach(i =>  Destroy(i));

        var sprite = (Sprite)Resources.Load("Emotions/" + emotionName, typeof(Sprite));
        var icon = (GameObject) Instantiate(Resources.Load("EmotionIcon"));
        icon.name = emotionName;
        icon.transform.SetParent(this.transform);
        icon.transform.localPosition = new Vector3(1.62f, 0, 0);
        icon.transform.localRotation = Quaternion.identity;
        icon.GetComponent<Image>().sprite = sprite;
        showingEmotions.Add(icon);

        Destroy(icon, length);
        */

        // Schin major change to Emotion system
        // The displaying emotion is the last one added to list.
        // When adding a new emotion, it replaces any previous same icon no matter how long.
        ExpireEmotion(emotionName);
        Guid newID = Guid.NewGuid();
        showingEmotions.Add(new EmotionTuple(emotionName, newID));
        StartCoroutine(ExpireEmotionByID(newID, length));
    }
    public void ExpireEmotion(string emotionName)//schin
    {
        foreach (EmotionTuple et in showingEmotions.ToArray())
        {
            if (et.eType == emotionName)
                showingEmotions.Remove(et);
        }
    }
    IEnumerator ExpireEmotionByID(Guid id, float delay)//schin
    {
        yield return new WaitForSeconds(delay);
        foreach (EmotionTuple et in showingEmotions.ToArray())
        {
            if (et.id == id)
                showingEmotions.Remove(et);
        }
    }
    public bool HasEmotion(string emotionName)//schin
    {
        // null input for checking ANY emotion
        if (emotionName == null & showingEmotions.Count > 0)
        {
            Debug.Log(showingEmotions.Count);
            return true;

        }
        foreach(EmotionTuple et in showingEmotions.ToArray())
        {
            if (et.eType == emotionName)
                return true;
        }
        return false;
    }
    private void UpdateEmotion()//schin
    {
        if (showingEmotions.Count == 0)
        {
            emotionPanel.transform.Find("EmotionSprite").GetComponent<SpriteRenderer>().sprite = null;
            emotionPanel.SetActive(false);
        }
        else
        {
            Sprite sprite = (Sprite)Resources.Load("Emotions/" + showingEmotions[showingEmotions.Count - 1].eType, typeof(Sprite));
            emotionPanel.transform.Find("EmotionSprite").GetComponent<SpriteRenderer>().sprite = sprite;
            emotionPanel.SetActive(true);
        }

    }
    public class EmotionTuple//schin
    {
        public EmotionTuple(string eType, Guid id)
        {
            this.eType = eType;
            this.id = id;
        }
        //type defined by EmotionType
        public string eType;
        //Guid when generated
        public Guid id;
    }
}

public sealed class EmotionType//schin
{
    public static readonly string THUMBS_UP = "thumbsUp";
    public static readonly string CD_READY = "cdReady";
}
