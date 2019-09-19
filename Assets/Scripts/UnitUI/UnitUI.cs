using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
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
}
