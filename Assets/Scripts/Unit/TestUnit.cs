﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUnit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(new Vector3(-50, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
