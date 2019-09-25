using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragUI : MonoBehaviour
{
    private Image circleImg;
    private GameObject arrowImgWrapper;
    private Image arrowImg;

    private void Awake()
    {
        circleImg = transform.Find("Circle").gameObject.GetComponent<Image>();
        arrowImgWrapper = transform.Find("ArrowWrapper").gameObject;
        arrowImg = arrowImgWrapper.transform.Find("Arrow").gameObject.GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ShowDragUI(true);
    }

    // Update is called once per frame
    void Update()
    {
        // Arrow Rotation
    }

    public void ShowDragUI(bool b)
    {
        circleImg.enabled = b;
        arrowImgWrapper.SetActive(b);
    }

    private void UpdateArrowRotation(Vector2 vec)
    {
        arrowImgWrapper.GetComponent<RectTransform>().localRotation = Quaternion.LookRotation(new Vector3(vec.y, 0, vec.x), Vector3.up);
        // Adjust arrow position
        //var x = (arrowImg.rectTransform.sizeDelta.x / 2) * (1 - Mathf.Cos(rad));
        //var y = -(arrowImg.rectTransform.sizeDelta.x / 2) * Mathf.Sin(rad);
        //arrowImg.rectTransform.localPosition = new Vector2(-x, -y);
    }

    public void ArrowLockAt(Vector2 vec)
    {
        print(vec);
        UpdateArrowRotation(new Vector2(-vec.x, -vec.y));
        
    }
}