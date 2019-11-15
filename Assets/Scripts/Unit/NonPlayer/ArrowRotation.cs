using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowRotation : MonoBehaviour
{
    private BasicUnit bu;

    private void Awake()
    {
        bu = GetComponent<BasicUnit>();
    }

    private void LateUpdate()
    {
        Vector3 vel = new Vector3(bu.unit.vx, 0, bu.unit.vz);
        Vector3 relToCam = transform.position - Camera.main.transform.position;
        transform.LookAt(transform.position + vel, relToCam);
    }
}
