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
    public float TextYOffset = 5;

    public float CountDownMax = 15;

    public float Threshold = 2.0f;

    private int myUnlockedUnitsPos = 0;

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
        int len = targets.Count();
        for (int i = 0; i < len; i++)
        {
            UpdateIconPosition(targets[i]);

            if (!targets[i].Unit.Locked && !targets[i].Unlocked)
            {
                var nextPos = targets[i].Unit.Owned ? -1 : 4;
                for (int k=0; k<len; k++)
                {
                    if (targets[k].Unit.Owned == targets[i].Unit.Owned && targets[k].Unlocked)
                    {
                        if (nextPos < targets[k].Position)
                            nextPos = targets[k].Position;
                    }
                }
                nextPos++;

                var cdui = targets[i];
                cdui.Position = nextPos;
                SetYPosition(cdui, nextPos);
                cdui.Unlocked = true;
                targets[i] = cdui;
            }
        }
        Debug.Assert(targets.Where(i => !i.Unit.Owned).All(i => i.Position >= 5));
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
            cdui.Unlocked = false;
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
        int[] candidates = { 1, 2, 3, 4, 5, -1, -2, -3, -4, -5 };
        Debug.Log("posID" + posId);
        var y = candidates[posId] * IconYOffset;

        cdui.Icon.transform.localPosition = new Vector3(cdui.Icon.transform.localPosition.x, y, 0);
        //Schin change y>0 condition to (0,0) so that text is not too high
        // Also add canvas to Text of Countdown Icon to make time text always visible
        Vector2 textOffset = y > 0 ? new Vector2(0, TextYOffset) : new Vector2(0, -TextYOffset);
        cdui.Icon.GetComponent<CountDownIcon>().SetTextPosition(textOffset);
    }

    private void DecideIconPosition(CountDownUnitIcon cdui)
    {
        var pos = cdui.Unit.Owned ? 0 : 5;
        cdui.Position = pos;
        SetYPosition(cdui, pos);
        AvoidOverlap();
    }

    private void AvoidOverlap()
    {
        var myUnits = targets.Where(i => i.Unit.Owned).OrderBy(i => i.Unit.WaitTime).ToArray<CountDownUnitIcon>();
        var enemyUnits = targets.Where(i => !i.Unit.Owned && i.Unit.Locked).OrderBy(i => i.Unit.WaitTime).ToArray<CountDownUnitIcon>();

        void solve(CountDownUnitIcon[] array, bool owned)
        {
            int len = array.Length;

            for (int i=0; i<len-1; i++)
            {
                for (int j = i+1; j < len; j++)
                {
                    Debug.Log(array[i].Position);
                    if (array[j].Unit.Locked && array[i].Position == array[j].Position)
                    {
                        float diff = array[j].Unit.WaitTime - array[i].Unit.WaitTime;
                        if (diff < Threshold)
                        {
                            array[j].Position++;
                            SetYPosition(array[j], array[j].Position);
                        }
                    }
                }
            }
        }

        solve(myUnits, true);
        solve(enemyUnits, false);
    }
}

class CountDownUnitIcon
{
    public BasicUnit Unit;
    public GameObject Icon;
    public int Position;
    public bool Unlocked;
}