using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsUITopButton : MonoBehaviour
{

    public BasicUnit Target;

    public Transform TargetTransform
    {
        get => Target.transform;
    }
    

    private RectTransform myTransform;
    public Vector2 Offset = new Vector2(0, 0);

    public float SubButtonRadius;
    public float SubButtonOffsetRadian;
    public float SubButtonInterval;

    public List<GameObject> SubButtons;
    private List<GameObject> subButtonInstances = new List<GameObject>();

    public void NotifyActive(bool active)
    {
        if (!active)
        {
            SetSubButtonActive(false);
        }
    }

    void Start()
    {
        myTransform = GetComponent<RectTransform>();
    }

    void LateUpdate()
    {
        myTransform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, TargetTransform.position) + Offset;
    }

    public void SetSubButtonActive(bool active)
    {
        if (active)
        {
            GetComponentInParent<ButtonsUI>().CloseSubButtons();
            var basePoint = new Vector2(0, SubButtonRadius);

            for (int i=0; i<SubButtons.Count; i++)
            {
                var rad = SubButtonOffsetRadian + i * SubButtonInterval;
                var subX = basePoint.x * Mathf.Cos(rad) - basePoint.y * Mathf.Sin(rad);
                var subY = basePoint.x * Mathf.Sin(rad) + basePoint.y * Mathf.Cos(rad);

                var subButton = Instantiate(SubButtons[i]);
                subButton.GetComponent<RectTransform>().SetParent(myTransform);
                subButton.GetComponent<RectTransform>().localPosition = new Vector3(subX, subY, 0);

                subButton.GetComponent<ButtonsUISubButtonBase>().TopButton = this;

                subButtonInstances.Add(subButton);
            }
        }
        else
        {
            subButtonInstances.ForEach(i => Destroy(i));
        }
    }
}