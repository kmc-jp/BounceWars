
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfoTag : MonoBehaviour
{
    public int uuid;
    public Material red;
    public bool owned;
    public BasicUnit basicUnit;
    public Simulator sim;
    public void CollisionEvent(CollisionInfo info)
    {
        if (basicUnit != null)
            basicUnit.CollisionEvent(info);
    }
    private void Start()
    {
        //GetComponent<MeshRenderer>().sharedMaterial = red;
    }
    public void Apply(Unit u)
    {
        transform.position = new Vector3(u.x, 0, u.z);
        uuid = u.uuid;
        this.gameObject.SetActive(!u.isDead);
        if (basicUnit == null) return;
        basicUnit.unit = sim.GetUnit(uuid);
        basicUnit.isDead = u.isDead;
    }
    public void SetOwned(bool b)
    {
        owned = b;
        if (!owned)
        {
            MeshRenderer mr = GetComponent<MeshRenderer>();
            if (mr != null)
            {
                mr.sharedMaterial = red;
            }
        }
        BasicUnit basicUnit = GetComponent<BasicUnit>();
        if (basicUnit != null)
        {
            // Tinaxd set owner
            basicUnit.Owned = b;
        }
    }

    public void InitializeBasicUnit(Unit u)
    {
        if (basicUnit == null) return;
        basicUnit.HP = u.HP;
        basicUnit.MaxHP = u.HP;
        basicUnit.MP = u.MP;
        basicUnit.MaxMP = u.MP;
        basicUnit.infoTag = this;
    }
}
