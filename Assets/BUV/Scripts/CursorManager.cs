using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;
    private Vector2 cursorHotspot;

    // Resolve Bug 
    
    /*void Start()
    {
        cursorHotspot = new Vector2(cursorTexture.width/3, cursorTexture.height/3);
        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
    }*/
}
