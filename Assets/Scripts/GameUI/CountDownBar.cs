using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

// Original author: Tinaxd
// Attach this script to CountDownBar
public class CountDownBar : MonoBehaviour
{
    private List<CountDownUnitIcon> targets = new List<CountDownUnitIcon>();

    private RectTransform rect;
    private float barWidth;

    public float IconYOffset = 20;
    public float TextYOffset = 10;

    public float CountDownMax = 15;

    public float Threshold = 2.0f;

    private GameObject modelIcon;

    private void Awake()
    {
        modelIcon = ((GameObject)Resources.Load("CountDownIcon"));
    }

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        barWidth = rect.sizeDelta.x;
    }

    private void Update()
    {
        targets.ForEach(i => UpdateIconPosition(i));
        AvoidOverlap();
    }

    private void UpdateIconPosition(CountDownUnitIcon cdui)
    {
        float time = cdui.Unit.WaitTime;

        cdui.Icon.GetComponent<RectTransform>().transform.localPosition = new Vector3(GetIconX(time), cdui.Icon.GetComponent<RectTransform>().transform.localPosition.y, 0);

        if (cdui.Unit.Moved)
        {
            float penalty;
            if (cdui.Unit.LockdownPenalty)
            {
                penalty = cdui.Unit.LockdownPenaltyTime + cdui.Unit.BaseCountDownTime;
                cdui.Unit.LockdownPenalty = false;
            }
            else
            {
                penalty = cdui.Unit.BaseCountDownTime;
            }
            cdui.Icon.GetComponent<CountDownIcon>().ShowPenaltyTime(penalty);
            cdui.Unit.Moved = false;

            DecideIconPosition(cdui);
        }
    }

    private float GetIconX(float time)
    {
        return (float) (barWidth * (time / CountDownMax - 0.5));
    }

    // Add units to the countdown bar
    public void RegisterUnit(BasicUnit unit, string iconPath)
    {
        var texture = (Texture)Resources.Load(iconPath);
        GameObject icon = Instantiate(modelIcon);

        icon.GetComponent<RawImage>().texture = texture;

        var cdui = new CountDownUnitIcon();
        cdui.Unit = unit;
        cdui.Icon = icon;
        targets.Add(cdui);

        icon.GetComponent<RectTransform>().SetParent(this.transform);

        cdui.Icon.GetComponent<CountDownIcon>().TargetUnit = cdui.Unit;

        DecideIconPosition(cdui);
    }

    // Remove units from the countdown bar
    public void RemoveUnit(BasicUnit unit)
    {
        for (int i=targets.Count-1; i>=0; i--)
        {
            if (targets[i].Unit == unit)
            {
                Destroy(targets[i].Icon);
                targets.RemoveAt(i);
            }
        }
    }

    private void SetYPosition(CountDownUnitIcon cdui, int posId)
    {
        int[] candidates = { 1, -1, 2, -2, 3 };
        var y = candidates[posId] * IconYOffset;

        cdui.Icon.transform.localPosition = new Vector3(cdui.Icon.transform.localPosition.x, y, 0);

        Vector2 textOffset = y > 0 ? new Vector2(0, TextYOffset) : new Vector2(0, -TextYOffset);
        cdui.Icon.GetComponent<CountDownIcon>().SetTextPosition(textOffset);
    }

    // TODO
    private void DecideIconPosition(CountDownUnitIcon cdui)
    {
        var posId = Random.Range(0, 2);
        SetYPosition(cdui, posId);
    }

    // TODO
    private void AvoidOverlap()
    {
        var unlocked = targets.Where(t => !t.Unit.Locked).ToArray();
        for (int i=0; i<unlocked.Length; i++)
        {
            SetYPosition(unlocked[i], i);
        }
    }
}

struct CountDownUnitIcon
{
    public BasicUnit Unit;
    public GameObject Icon;
    public int Position;
}