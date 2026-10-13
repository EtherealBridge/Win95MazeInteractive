using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelpWindow : MonoBehaviour
{
    public static HelpWindow _Current;

    /// <summary>
    /// Returns the current in-game help window.
    /// </summary>
    public static HelpWindow Current
    {
        get
        {
            if(_Current == null)
            {
                _Current = FindObjectOfType<HelpWindow>();
            }
            return _Current;
        }
    }

    public static void Unload()
    {
        _Current = null;
    }


    /// <summary>
    /// Window for showing / hiding help window
    /// </summary>
    public GameObject Window;

    /// <summary>
    /// Is this dialog window open?
    /// </summary>
    public bool isOpen
    {
        get
        {
            return Window.activeSelf;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F1))
        {
            ToggleWindow();
        }
    }

   /// <summary>
   /// Closes window if open. Opens window if closed.
   /// </summary>
    public void ToggleWindow()
    {
        Window.SetActive(!Window.activeSelf);
    }
}
