using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragUI : MonoBehaviour
{
    private Image circleImg;
    private Image arrowImg;

    private void Awake()
    {
        circleImg = transform.Find("Circle").gameObject.GetComponent<Image>();
        arrowImg = transform.Find("Arrow").gameObject.GetComponent<Image>();
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
        arrowImg.enabled = b;
    }

    private void UpdateArrowRotation(float rad)
    {
        var deg = Mathf.Rad2Deg * rad;
        arrowImg.rectTransform.rotation = Quaternion.Euler(0, 0, deg);
        // Adjust arrow position
        var x = (arrowImg.rectTransform.sizeDelta.x / 2) * (1 - Mathf.Cos(rad));
        var y = -(arrowImg.rectTransform.sizeDelta.x / 2) * Mathf.Sin(rad);
        arrowImg.rectTransform.localPosition = new Vector2(-x, -y);
    }

    public void ArrowLockAt(Vector2 vec)
    {
        var cos = Vector2.Dot(vec, new Vector2(1, 0)) / vec.magnitude;
        UpdateArrowRotation(Mathf.Acos(cos));
    }
}
