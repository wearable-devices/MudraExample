using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mudra.Unity;
 
public class TestMousePos : MonoBehaviour
{
    [SerializeField] RectTransform Cursor;
    [SerializeField] Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, PluginPlatform.mousePos,canvas.worldCamera,out pos);
        Cursor.position = canvas.transform.TransformPoint(pos);
    }
}
