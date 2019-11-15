using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowRotation : MonoBehaviour
{
    private Unit unit;

    private void Start()
    {
        Simulator sim = GetComponent<UnitInfoTag>().sim;
        unit = sim.GetUnit(GetComponent<UnitInfoTag>().uuid);
    }

    private void LateUpdate()
    {
        if (unit == null)
            return;
        Vector3 vel = new Vector3(unit.vx, 0, unit.vz);
        Vector3 relToCam = transform.position - Camera.main.transform.position;
        transform.LookAt(transform.position + vel, relToCam);
    }
}
