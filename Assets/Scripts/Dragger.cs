using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mudra.Unity;
public class Dragger : MonoBehaviour
{
    [SerializeField] Rigidbody DraggingObject;
    [SerializeField] Camera cam;
    [SerializeField] InputManager inputManager;
    [SerializeField] LayerMask mask;


    RaycastHit hit;
    bool holding;
    // Start is called before the first frame update
    void Start()
    {

    }


    public void Hold()
    {

        if (Physics.Raycast(cam.ScreenPointToRay(InputManager.mousePos), out hit, 100, mask))
        {
            DraggingObject = hit.rigidbody;
            if (DraggingObject != null)
            {
                DraggingObject.useGravity = false;
            }
            holding = true;
        }

    }
    public void Release()
    {
        if (DraggingObject == null) return;

        DraggingObject.useGravity = true;
        DraggingObject = null;
        holding = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (inputManager.pressure > 0.5 && !holding)
        {
            Hold();
        }
        if (holding && inputManager.pressure < 0.5f)
        {
            Release();
        }
        if (DraggingObject != null)
        {
            DraggingObject.velocity = cam.ScreenToWorldPoint(new Vector3(InputManager.mousePos.x, InputManager.mousePos.y, -DraggingObject.transform.position.z)) - DraggingObject.transform.position;

        }
    }
}
