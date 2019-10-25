using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowUnit : BasicUnit
{
    // Do not show mouse hover menu
    private void OnMouseEnter()
    {

    }

    private void OnMouseExit()
    {

    }

    protected override void Awake()
    {

    }

    protected override void Start()
    {

    }

    protected override  void Update()
    {

    }

    public override float HP
    {
        get => this.hp;
        set 
        {
            this.hp = value;
            this.unit.HP = value;
        }
    }

    public override float MP
    {
        get => this.mp;
        set
        {
            this.mp = value;
            this.unit.MP = value;
        }
    }

    public override float MaxHP
    {
        get => this.HP;
        set
        {

        }
    }

    public override float MaxMP
    {
        get => this.MP;
        set
        {

        }
    }
}
