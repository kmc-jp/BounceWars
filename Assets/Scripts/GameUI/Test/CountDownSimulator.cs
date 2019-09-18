using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDownSimulator : MonoBehaviour
{
    public void SimulateMoved()
    {
        GetComponent<CountDownIcon>().TargetUnit.MarkMoved();
    }
}
