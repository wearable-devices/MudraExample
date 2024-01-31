using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragger : MonoBehaviour
{
    [SerializeField] Rigidbody DraggingObject;
    [SerializeField] Camera cam;
    [SerializeField] InputManager inputManager;
    [SerializeField] LayerMask mask;


    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {

    }


    public void Hold()
    {
        if (Physics.Raycast(cam.ScreenPointToRay(inputManager.MousePos), out hit, 100, mask))
        {
            DraggingObject = hit.rigidbody;
            if (DraggingObject != null)
            {
                DraggingObject.useGravity = false;
            }
        }
    }
    public void Release()
    {
        if (DraggingObject == null) return;

        DraggingObject.useGravity = true;
        DraggingObject = null;
    }
    // Update is called once per frame
    void Update()
    {
        if (DraggingObject != null)
        {
            DraggingObject.velocity = cam.ScreenToWorldPoint(new Vector3(inputManager.MousePos.x, inputManager.MousePos.y, -DraggingObject.transform.position.z)) - DraggingObject.transform.position;
        }
    }
}
