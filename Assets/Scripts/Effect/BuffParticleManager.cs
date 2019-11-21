
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffParticleManager : MonoBehaviour
{
    //disable play onawake
    public List<GameObject> prefabs;
    List<ParticlePair> instances;
    
    // Start is called before the first frame update
    void Start()
    {
        instances = new List<ParticlePair>();
    }
    void AddParticle(int id)
    {
        ParticlePair pp = new ParticlePair((GameObject)(Instantiate(prefabs[id],transform.position, Quaternion.Euler(36, 0, 0), transform)), id);
        instances.Add(pp);
    }
    public void UpdateParticles(int mask)
    {
        List<ParticlePair> remains = new List<ParticlePair>();
        for(int i = 0; i < instances.Count; i++)
        {
            if(((mask >> instances[i].id) & 1) == 1)
            {
                remains.Add(instances[i]);
            }
        }
        instances = remains;
        for (int i = 0; i < 10; i++)
        {
            if (((mask >> i) & 1) == 1)
            {
                if (!ExistParticle(i))
                {
                    AddParticle(i);
                }
            }
        }
    }
    void RemoveParticle(int index)
    {
        Destroy(instances[index].gameObject);
        instances.RemoveAt(index);
    }
    bool ExistParticle(int id)
    {
        for(int i = 0; i < instances.Count; i++)
        {
            if (instances[i].id == id) return true;
        }
        return false;
    }
    private void Update()
    {
        for(int i = 0; i < instances.Count; i++)
        {
            GameObject g = instances[i].gameObject;
            g.transform.position=transform.position+ Quaternion.Euler(36, 0, 0) * Vector3.down;
        }
    }
}
[System.Serializable]
public class ParticlePair
{
    public GameObject gameObject;
    public int id;
    public ParticlePair(GameObject gameObject, int id)
    {
        this.gameObject = gameObject;
        this.id = id;
    }
}