using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Schin
//Control camera movement with player inputs
//Limit camera boundary
public class CameraMvmt : MonoBehaviour
{
    //public GameObject mainCamera;

    public bool mouseOperationEnabled = false;

    public float panSpeed = 12f;
    public float panBorder = 10f;
    public Vector2 panSpatialLimit = new Vector2(20f, 20f);

    public float scrollSpeed = 15f;
    public float scrollMaxY = 12f;
    public float scrollMinY = 4f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;

        // camera movement
        if (Input.GetKey("w") || (Input.mousePosition.y >= Screen.height - panBorder && mouseOperationEnabled))
        {
            Debug.Log("aaa");
            pos.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("s") || (Input.mousePosition.y <= panBorder && mouseOperationEnabled))
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("a") || (Input.mousePosition.x <= panBorder && mouseOperationEnabled))
        {
            pos.x -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey("d") || (Input.mousePosition.x >= Screen.width - panBorder && mouseOperationEnabled))
        {
            pos.x += panSpeed * Time.deltaTime;
        }

        //camera zooming
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= mouseScroll * scrollSpeed * 100f * Time.deltaTime;


        //camera update
        pos.x = Mathf.Clamp(pos.x, -panSpatialLimit.x, panSpatialLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panSpatialLimit.y, panSpatialLimit.y);

        pos.y = Mathf.Clamp(pos.y, scrollMinY, scrollMaxY);
        
        transform.position = pos;
    }
}
