using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private float _damage;
    private GameObject unit;
    private Canvas canvas;

    public float time;

    public float Damage
    {
        get => _damage;
        set => _damage = value;
    }

    public GameObject Unit
    {
        get => unit;
        set => unit = value;
    }

    public Canvas Canvas
    {
        get => canvas;
        set => canvas = value;
    }

    public string Text
    {
        get => GetComponent<TextMesh>().text;
        set => GetComponent<TextMesh>().text = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, time);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = new Vector3(1, 0, 0) * Time.deltaTime + transform.position;

        
    }

    private void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    

}
