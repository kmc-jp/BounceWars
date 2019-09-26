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
    }

    public void ShowDragUI(bool b)
    {
        circleImg.enabled = b;
        arrowImgWrapper.SetActive(b);
    }

    private void UpdateArrowRotation(Vector3 vec)
    {
        arrowImgWrapper.transform.rotation = Quaternion.LookRotation(vec, new Vector3(0, 1, 0));
    }

    public void ArrowDrag(Vector3 vec)
    {
        //print(vec);
        UpdateArrowRotation(new Vector3(-vec.x, vec.y, -vec.z));
    }
}