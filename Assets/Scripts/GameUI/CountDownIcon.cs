using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Original author: Tinaxd
// Attach this script to CountDownIcon.prefab
public class CountDownIcon : MonoBehaviour
{
    public BasicUnit TargetUnit;

    private Text textObj;

    //public Vector2 TextOffset = new Vector2(0, 20);

    public readonly Color NormalTextColor = new Color(0x00, 0x1e, 0xff);
    public readonly Color PenaltyTextColor = new Color(0xff, 0, 0);

    public void ShowTimeText(bool show)
    {
        if (!ShowingPenaltyTime)
        {
            NormalText();
            textObj.enabled = show;
        }
    }

    private bool ShowingPenaltyTime = false;

    public void ShowPenaltyTime(float penalty)
    {
        PenaltyText();
        textObj.enabled = true;
        textObj.text = string.Format("+{0:0.#}s", penalty);
        Invoke("_HideText", 1);
    }

    private void _HideText()
    {
        textObj.enabled = false;
        ShowingPenaltyTime = false;
    }

    public void SetTextPosition(Vector2 pos)
    {
        textObj.GetComponent<RectTransform>().localPosition = pos;
    }

    private void Awake()
    {
        textObj = transform.Find("Text").gameObject.GetComponent<Text>();
    }

    private void Start()
    {
        ShowTimeText(false);
    }

    private void Update()
    {
        if (!ShowingPenaltyTime)
            textObj.text = string.Format("{0:0.#}s", TargetUnit.WaitTime);
    }

    private void NormalText()
    {
        textObj.color = NormalTextColor;
    }

    private void PenaltyText()
    {
        textObj.color = PenaltyTextColor;
        ShowingPenaltyTime = true;
    }
}
