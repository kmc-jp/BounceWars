using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulator : MonoBehaviour
{
    public List<GameObject> prefabs;
    List<UnitInfoTag> instances = new List<UnitInfoTag>();
    public List<Unit> units = new List<Unit>();
    public int isClient = 0;
    public List<Command> commands = new List<Command>();
    private void Awake()
    {

    }
    void SimulateCollision(List<Unit> targets)
    {
        Debug.Log(targets[0].vx);
        List<bool> clean = new List<bool>();

        for (int i = 0; i < targets.Count; i++)
        {
            clean.Add(true);
        }
        for (int i = 0; i < targets.Count; i++)
        {
            targets[i].vx1 = targets[i].vx;
            targets[i].vz1 = targets[i].vz;
        }
        for (int i = 0; i < targets.Count; i++)
        {
            for (int j = i; j < targets.Count; j++)
            {
                if (i == j) continue;
                Unit u1 = targets[i];
                Unit u2 = targets[j];
                float d = UnitDistance1(u1, u2);
                float dx = u2.x1 - u1.x1;
                dx /= d;
                float dz = u2.z1 - u1.z1;
                dz /= d;

                if (0<d&&d < 1)
                {
                    float rvx = u2.vx - u1.vx;
                    float rvz= u2.vz - u1.vz;
                    float sizeVertical = dx * rvx + dz * rvz;
                    u1.vx1 = u1.vx + dx* sizeVertical;
                    u1.vz1 = u1.vz + dz* sizeVertical;
                    u2.vx1 = u2.vx - dx* sizeVertical;
                    u2.vz1 = u2.vz - dz* sizeVertical;
                    //Debug.Log(string.Format("{0}:({1},{2}),({3},{4})({5})",Time.time,i,j,rvx,rvz,sizeVertical));
                    //Debug.Log(string.Format("{0} i={1}:({2},{3})",Time.time,i, u1.vx1, u1.vz1));
                    clean[i] = false;
                    clean[j] = false;
                }
            }
        }
        float E = 0;
        float px = 0;
        float pz = 0;
        for (int i = 0; i < targets.Count; i++)
        {
            targets[i].vx = targets[i].vx1;
            targets[i].vz = targets[i].vz1;
            E += targets[i].vx * targets[i].vx + targets[i].vz * targets[i].vz;
            px += targets[i].vx;
            pz += targets[i].vz;
        }

//        Debug.Log(targets[0].vx);
        //Debug.Log(string.Format("E={0},p=({1},{2})", E, px, pz));
        for (int i = 0; i < targets.Count; i++)
        {
            if (clean[i])
            {
                ApplyIntegral(targets[i]);
            }
            else
            {
                SimulateIntegral(targets[i],Time.deltaTime);
                ApplyIntegral(targets[i]);
            }
        }

        Debug.Log(targets[0].vx);
    }
    float UnitDistance1(Unit u1, Unit u2)
    {
        //Debug.Log(Mathf.Sqrt((u1.x1 - u2.x1) * (u1.x1 - u2.x1) + (u1.z1 - u2.z1) * (u1.z1 - u2.z1)));
        return Mathf.Sqrt((u1.x1 - u2.x1) * (u1.x1 - u2.x1) + (u1.z1 - u2.z1) * (u1.z1 - u2.z1));
    }
    void SimulateIntegral(Unit u, float dt)
    {
        float v = Mathf.Sqrt(u.vx * u.vx + u.vz * u.vz);
        if (v > 0)
        {
            float fx = -u.vx / v;
            float fz = -u.vz / v;
            float fxdt = fx * dt;
            float fzdt = fz * dt;
            float vx1 = u.vx + fxdt;
            float vz1 = u.vz + fzdt;
            if (vx1 * u.vx > 0)
            {
                u.vx = vx1;
                u.vz = vz1;
            }
            else
            {
                u.vx = 0;
                u.vz = 0;
            }
        }
        u.x1 += u.vx * dt;
        u.z1 += u.vz * dt;
    }
    void ApplyIntegral(Unit u)
    {
        u.x = u.x1;
        u.z = u.z1;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (isClient > 0)
        {
            return;
        }
        for (int n = -1; n < 2; n += 2)
        {
            for (int i = 0; i < 10; i++)
            {
                Unit u = new Unit();
                u.x = n * 10;
                u.z = i*1.5f;
                u.x1 = u.x;
                u.z1 = u.z;
                u.uuid = Random.Range(int.MinValue, int.MaxValue);
                if (n == -1)
                {
                    u.owner = 0;
                }
                else
                {
                    u.owner = 1;
                }
                units.Add(u);
                /*
                GameObject g = Instantiate(prefabs[0]);
                UnitInfoTag tag = g.GetComponent<UnitInfoTag>();
                tag.Apply(u);
                instances.Add(tag);*/
            }
        }
        UpdateInstances();
    }

    void UpdateInstances()
    {
        for (int i = 0; i < units.Count; i++)
        {
            bool found = false;
            for (int j = 0; j < instances.Count; j++)
            {
                if (units[i].uuid == instances[j].uuid)
                {
                    instances[j].Apply(units[i]);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                GameObject g = Instantiate(prefabs[0]);
                UnitInfoTag tag = g.GetComponent<UnitInfoTag>();
                tag.Apply(units[i]);
                tag.SetOwned(units[i].owner == isClient);
                instances.Add(tag);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ProcessMyCommand();
        for (int i = 0; i < units.Count; i++)
        {
            SimulateIntegral(units[i], Time.deltaTime);
        }
        SimulateCollision(units);
        UpdateInstances();
    }

    void ProcessMyCommand()
    {
        for (int i = 0; i < commands.Count; i++)
        {
            Command c = commands[i];
            if (c.processed)
            {
                continue;
            }
            Unit u = GetUnit(c.uuid);
            u.vx = c.vx;
            u.vz = c.vz;
            c.processed = true;
        }
        List<Command> remains = new List<Command>();
        for (int i = 0; i < commands.Count; i++)
        {
            if (commands[i].sent) continue;
            remains.Add(commands[i]);
        }
        commands = remains;
    }
    public void SetCommandsSent()
    {
        for (int i = 0; i < commands.Count; i++)
        {
            Command c = commands[i];
            if(c!=null)
            c.sent = true;
        }
    }
    public Unit GetUnit(int uuid)
    {
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].uuid == uuid)
            {
                return units[i];
            }
        }
        return null;
    }
}
