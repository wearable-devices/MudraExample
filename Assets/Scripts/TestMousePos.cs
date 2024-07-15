using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mudra.Unity;
 
public class TestMousePos : MonoBehaviour
{
    [SerializeField] RectTransform Cursor;
    [SerializeField] Canvas canvas;
   // [SerializeField] InputManager inputs;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      //  Debug.Log(Cursor.position);
       // Debug.Log(InputManager.mousePos);

        Cursor.position = InputManager.mousePos ;
    }
}
