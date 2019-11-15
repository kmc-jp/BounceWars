using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPlay : MonoBehaviour
{
    public static bool isOffline = true;
    Simulator sim;
    private void Start()
    {
        sim = GetComponent<Simulator>();
        if (sim.isClient == 1)
        {
            isOffline = false;
            enabled = false;
            return;
        }
        if (!isOffline)
        {
            enabled = false;
            return;
        }
    }
    private void Update()
    {
        List<Unit> myUnit = new List<Unit>();
        List<Unit> units = sim.units;
        List<Unit> enemies = new List<Unit>();
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].owner == 0)
            {
                enemies.Add(units[i]);
            }
            if (units[i].owner == 1)
            {
                myUnit.Add(units[i]);
            }
        }
        List<bool> occupied = new List<bool>();

        for (int i = 0; i < units.Count; i++)
        {
            occupied.Add(false);
        }
        for (int i = 0; i < myUnit.Count; i++)
        {

            Unit u = myUnit[i];
            if (u.type != UnitType.TYPE_CHESS) continue;
            Unit target = FindNearest(u.GetPosition(), enemies);
            if (target == null)
            {
                break;
            }
            if (u.GetVelocity().magnitude >0.1f) { continue; }
            enemies.Remove(target);
            Vector3 vel = target.GetPosition() - u.GetPosition();
            vel = vel.normalized * 2;
            UnitMovedCmd c = new UnitMovedCmd();
            c.sent = false;
            c.vx = vel.x;
            c.vz = vel.z;
            c.uuid = u.uuid;
            c.owner = 0;
            sim.commands.Add(c);
        }

        for (int i = 0; i < myUnit.Count; i++)
        {

            Unit u = myUnit[i];
            if (u.type == UnitType.TYPE_CHESS) continue;
            Unit target = FindNearest(u.GetPosition(), enemies);
            if (target == null)
            {
                break;
            }
            enemies.Remove(target);
            Vector3 vel = target.GetPosition() - u.GetPosition();
            vel = vel.normalized * 2;
            UnitMovedCmd c = new UnitMovedCmd();
            c.sent = false;
            c.vx = vel.x;
            c.vz = vel.z;
            c.uuid = u.uuid;
            c.owner =1;
            sim.commands.Add(c);
        }
    }
    Unit FindNearest(Vector3 pos,List<Unit> units)
    {
        float min = float.PositiveInfinity;
        Unit u = null;
        for(int i=0;i< units.Count; i++)
        {
            float d = Vector3.Distance(units[i].GetPosition(), pos);
            if (d < min)
            {
                min = d;
                u = units[i];
            }
        }
        return u;
    }
}
