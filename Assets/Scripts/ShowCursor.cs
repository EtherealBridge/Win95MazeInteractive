using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shows or hides Unity cursor each frame
/// </summary>
public class ShowCursor : MonoBehaviour
{
   


    // Update is called once per frame
    void Update()
    {
        //Default to cursor not being visible
        bool ShowCursor = false;

        if(HelpWindow.Current.isOpen)
        {
            ShowCursor = true;
        }

        //If cursor is visible, then, allow it to move around. Otherwise keep it locked.
        CursorLockMode Curr_Cursor_Mode = CursorLockMode.Locked;
        if (ShowCursor)
        {
            Curr_Cursor_Mode = CursorLockMode.Confined;
        }
        Cursor.visible = ShowCursor;
        Cursor.lockState = Curr_Cursor_Mode;

        
    }
}
