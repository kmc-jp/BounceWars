using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoBow : MonoBehaviour
{
    Simulator sim;
    BallisticsSimulator ballisticsSimulator;
    // Start is called before the first frame update
    void Start()
    {
        sim = GetComponent<Simulator>();
        ballisticsSimulator = GameObject.Find("BallisticsViewer").GetComponent<BallisticsSimulator>();
    }
    public void ProcessArrow(float dt)
    {
        List<Unit> units = sim.units;
        for (int i = 0; i < units.Count; i++)
        {
            Unit u = units[i];
            if (u.type != UnitType.TYPE_ARROW)
            {
                continue;
            }
            if(u.vx*u.vx+ u.vz * u.vz < 5f)
            {
                u.HP = 0;
            }
        }
        for (int i = 0; i < units.Count; i++)
        {
            Unit u = units[i];
            if (u.type != UnitType.TYPE_ARCHER)
            {
                continue;
            }
            u.projectileReload -= dt;
            if (u.owner != sim.isClient)
            {
                continue;
            }
            if (u.projectileReload < 0)
            {
                Fire(u);
            }
        }
    }
    void GenerateCommand(Unit u, Vector3 vel)
    {
        NewUnitCmd cmd = new NewUnitCmd
        {
            fromUnitId = u.uuid,
            velocity = new Vector3(vel.x, 0, vel.z),
            unitType = 2, // Unit type Arrow
        };
        sim.commands.Add(cmd);
    }
    private bool Fire(Unit from)
    {
        List<Unit> units = sim.units;
        Unit target = null;
        float distance = float.PositiveInfinity;
        float approachData = float.PositiveInfinity;
        for (int i = 0; i < units.Count; i++)
        {
            Unit u = units[i];
            if (u.owner == from.owner||u.owner<0)
            {
                continue;
            }
            if (u.isDead)
            {
                continue;
            }
            UnitInfoTag unitInfo = sim.GetUnitInfoTag(u.uuid);
            if (unitInfo == null)
            {
                continue;
            }
            HideUnit hideUnit = unitInfo.transform.GetComponent<HideUnit>();
            if (hideUnit == null)
            {
                continue;
            }
            if (!hideUnit.spotted)
            {
                continue;
            }

            Vector3 p = new Vector3(from.x, 0, from.z);
            Vector3 q = new Vector3(u.x, 0, u.z);
            float d = Vector3.Distance(p, q);

            List<Vector3> trails = ballisticsSimulator.GetTrails(p, calculateVelocity(q - p));
            bool inRange = false;
            float mina = float.PositiveInfinity;
            for (int n = 0; n < trails.Count-1; n++)
            {
                Vector3 v1 = trails[i] - p;
                Vector3 v2 = trails[i+1] - trails[i];
                float area = Vector3.Magnitude(Vector3.Cross(v1, v2));
                float approach = area / v2.sqrMagnitude;
                Vector3 diff = trails[i] - p;
                if (approach < mina)
                {
                    mina = approach;
                }
                if (approach <= 2)
                {
                    inRange = true;
                    break;
                }
            }
            if (!inRange) continue;
            if (d < distance)
            {
                approachData = mina;
                distance = Vector3.Distance(p, q);
                target = u;
            }
        }
        if (target == null)
        {
            return false;
        }
        //Debug.Log("approach" + approachData);

        Vector3 pp = new Vector3(from.x, 0, from.z);
        Vector3 qq = new Vector3(target.x, 0, target.z);
        GenerateCommand(from, calculateVelocity(qq - pp));
        return true;
    }
    Vector3 calculateVelocity(Vector3 diff)
    {
        return diff.normalized * 10;
    }
}
