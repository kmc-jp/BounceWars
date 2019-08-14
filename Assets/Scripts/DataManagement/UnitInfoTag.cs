using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfoTag : MonoBehaviour
{
    public int uuid;
    public Material red;
    public bool owned;
    private void Start()
    {
        //GetComponent<MeshRenderer>().sharedMaterial = red;
    }
    public void Apply(Unit u)
    {
        transform.position = new Vector3(u.x, 0, u.z);
        uuid = u.uuid;
    }
    public void SetOwned(bool b)
    {
        owned = b;
        if (!owned)
        {
            GetComponent<MeshRenderer>().sharedMaterial = red;
        }
    }
}
