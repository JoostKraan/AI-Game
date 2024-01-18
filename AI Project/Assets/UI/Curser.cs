using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curser : MonoBehaviour
{
    BuildingManager buildingManager;
    public Texture2D customCursor;
    public Texture2D buildModecursor;
    void Start()
    {
        Cursor.SetCursor(customCursor, Vector2.zero, CursorMode.ForceSoftware);
    }

    void update()
    {
        if (buildingManager.buildMode)
        {
            Cursor.SetCursor(buildModecursor,Vector2.zero,CursorMode.ForceSoftware);
        }
        else
        {
            Cursor.SetCursor(customCursor, Vector2.zero, CursorMode.ForceSoftware);
        }
    }

    
   
}
