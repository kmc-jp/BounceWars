using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallisticsSimulator : MonoBehaviour
{
    public LineRenderer lr;
    // Start is called before the first frame update
    void Start()
    {
        lr.SetWidth(0.1f, 0.1f);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdateTrails(Vector3 position, Vector3 velocity)
    {
        List<Vector3> trails = GetTrails(position, velocity);
        lr.positionCount=(trails.Count);
        lr.SetPositions(trails.ToArray());
    }
    public List<Vector3> GetTrails(Vector3 position,Vector3 velocity)
    {
        List<Vector3> trails = new List<Vector3>();
        float dt = 1 / 30f;
        Unit unit = new Unit();
        unit.x = position.x;
        unit.z = position.z;
        unit.x1 = position.x;
        unit.z1 = position.z;
        unit.vx = velocity.x;
        unit.vz = velocity.z;
        unit.vx1 = velocity.x;
        unit.vz1 = velocity.z;
        for (float t = 0; t < 10f; t += dt)
        {
            unit.vx1 = unit.vx;
            unit.vz1 = unit.vz;
            PhysicsSimulator.SimulateIntegral(unit, dt, MapBehaviour.instance);
            if (!PhysicsSimulator.CollideMap(unit, MapBehaviour.instance))
            {
                PhysicsSimulator.SimulateIntegral(unit, dt, MapBehaviour.instance);
                unit.vx = unit.vx1;
                unit.vz = unit.vz1;
            }
            PhysicsSimulator.ApplyIntegral(unit);
            trails.Add(new Vector3(unit.x, 0, unit.z));
            if (t > 3)
            {
                dt = 0.1f;
            }
            if (unit.vx == 0 && unit.vz == 0)
            {
                break;
            }
        }
        return trails;
    }
}
