using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] InputManager inputs;
   
    // Update is called once per frame
    void Update()
    {
        transform.rotation = inputs.quaternion;
    }
}
