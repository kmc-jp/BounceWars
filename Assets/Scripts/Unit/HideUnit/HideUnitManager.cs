using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideUnitManager : MonoBehaviour
{
    public Simulator sim;
    public float spotRange;
    // Update is called once per frame
    void Update()
    {
        List<UnitInfoTag> allies = new List<UnitInfoTag>();
        for(int i = 0; i < sim.instances.Count; i++)
        {
            if (sim.instances[i].owned)
            {
                allies.Add(sim.instances[i]);
            }
        }
        for (int i = 0; i < sim.instances.Count; i++)
        {
            if (!sim.instances[i].owned)
            {
                HideUnit hideUnit = sim.instances[i].GetComponent<HideUnit>();
                bool spotted=false;
                for(int j = 0; j < allies.Count; j++)
                {
                    Vector3 pos = allies[j].transform.position;
                    pos.y = 0;
                    Vector3 pos1 = hideUnit.transform.position;
                    pos1.y = 0;
                    if (Vector3.Distance(pos, pos1) < spotRange)
                    {
                        spotted = true;
                    }
                }
                hideUnit.spotted = spotted;
            }
        }
    }
}
