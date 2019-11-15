using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepRotating : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, Time.deltaTime*180, 0);
    }
}
