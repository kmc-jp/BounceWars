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
        ShowDragUI(false);
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

    private void UpdateArrowRotation(Vector3 vec)
    {
        arrowImgWrapper.transform.rotation = Quaternion.FromToRotation(new Vector3(0, 1, 0), vec);
        // Adjust arrow position
        //var x = (arrowImg.rectTransform.sizeDelta.x / 2) * (1 - Mathf.Cos(rad));
        //var y = -(arrowImg.rectTransform.sizeDelta.x / 2) * Mathf.Sin(rad);
        //arrowImg.rectTransform.localPosition = new Vector2(-x, -y);
    }

    public void ArrowDrag(Vector3 vec)
    {
        //print(vec);
        UpdateArrowRotation(new Vector3(-vec.x, vec.y, -vec.z));
        
    }
}