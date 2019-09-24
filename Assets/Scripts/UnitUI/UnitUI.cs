using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
    private List<GameObject> showingEmotions = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // Schin  set TextMesh layer to avoid render layer issue
        transform.Find("WaitTime").GetComponent<MeshRenderer>().sortingLayerName = "UserGUI";
        transform.Find("WaitTime").GetComponent<MeshRenderer>().sortingOrder = 0;
        //rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
     
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
        showingEmotions.ForEach(i => Destroy(i));

        var sprite = (Sprite)Resources.Load("Emotions/" + emotionName, typeof(Sprite));
        var icon = (GameObject) Instantiate(Resources.Load("EmotionIcon"));
        icon.transform.SetParent(this.transform);
        icon.transform.localPosition = new Vector3(1.62f, 0, 0);
        icon.transform.localRotation = Quaternion.identity;
        icon.GetComponent<Image>().sprite = sprite;
        showingEmotions.Add(icon);

        Destroy(icon, length);
    }
}
