using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherUnit : BasicUnit
{
    private bool IsArcherMode = false;

    public void StartArcherMode()
    {
        IsArcherMode = true;
        // TODO: Change mouse cursor
    }

    override protected void Update()
    {
        base.Update();
        
        if (IsArcherMode)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var playerPlane = new Plane(Vector3.up, this.transform.position);
                    playerPlane.Raycast(ray, out float distance);
                    var localOrigin = ray.GetPoint(distance);
                }
            }
        }
    }
}
