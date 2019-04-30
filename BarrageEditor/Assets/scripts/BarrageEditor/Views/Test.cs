//Attach this script to your Canvas GameObject.
//Also attach a GraphicsRaycaster component to your canvas by clicking the Add Component button in the Inspector window.
//Also make sure you have an EventSystem in your hierarchy.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Test : MonoBehaviour
{
    public float posX = 640;
    public float posY = 720;

    private RectTransform _rectTf;

    void Start()
    {
        _rectTf = GetComponent<RectTransform>();
    }

    int frame = 0;

    void Update()
    {
        _rectTf.anchoredPosition = new Vector2(posX, posY);
        if ( frame % 60 == 0 )
        {
            Logger.Log(Input.mousePosition);
        }
        frame++;
    }
}