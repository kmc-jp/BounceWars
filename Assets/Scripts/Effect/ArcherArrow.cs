using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Represents arrows sent by archers
public class ArcherArrow : BasicUnit
{
    private void Update()
    {
        
    }

    private void Start()
    {
        HP = 1;
        MP = 0;
    }

    private void OnMouseEnter() { }
    private void OnMouseExit() { }
    public new void MarkMoved() { }

}
