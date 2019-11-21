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
    void LateUpdate()
    {
    }

    public void ShowDragUI(bool b)
    {
        b = false;//tmporary
        circleImg.enabled = b;
        arrowImgWrapper.SetActive(b);
    }

    private void UpdateArrow(Vector3 vec)
    {
        arrowImgWrapper.transform.rotation = Quaternion.LookRotation(vec, new Vector3(0, 1, 0));
        // Schin change the position of Arrow
        float rad = vec.magnitude < 2f ? 0f : vec.magnitude-2f;
        vec = vec.normalized * rad;
        arrowImgWrapper.transform.localPosition = new Vector3(vec.x, vec.z,0);
    }

    public void UpdateDragUI(Vector3 vec)
    {
        //print(vec);
        float rad = vec.magnitude < 2f ? 2f : vec.magnitude;
        UpdateArrow(new Vector3(-vec.x, vec.y, -vec.z));
        circleImg.rectTransform.localScale = new Vector2(rad, rad);
    }
}