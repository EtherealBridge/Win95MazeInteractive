using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartGame : MonoBehaviour
{
    /// <summary>
    /// Text control for game over
    /// </summary>
    public Text GameOverTextControl;

    /// <summary>
    /// Subfolder containing this window
    /// </summary>
    public GameObject Elements_Subfolder;


    public Objective Final_Objective;

    Entity _Player;

    /// <summary>
    /// The player
    /// </summary>
    public Entity Player
    {

        get
        {
            if (_Player == null)
            {
                GameObject Player_Obj = GameObject.FindGameObjectWithTag("Player");
                _Player = Player_Obj.GetComponent<Entity>();
            }

            return _Player;

        }
    }

    // Update is called once per frame
    void Update()
    {
        //Control + R resets
        bool HotkeyHeld = false;
        if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            HotkeyHeld = true;
        }


        //If player is dead, then, show game over window
        if(Player.isDead)
        {
            ShowWindow();
        }


        //If player presses R, then, restart game
        if(Final_Objective.isFinished || Player.isDead || HotkeyHeld)
        {
            if (Input.GetKeyDown(KeyCode.R) && !HelpWindow.Current.isOpen)
             {
                //Clear static vars
                Deacon.Unload();
                CreateMaze.Unload();
                HelpWindow.Unload();
                Flipper.Unload();

                SceneManager.LoadScene("Maze");
            }
        }

    }

    /// <summary>
    /// Set this window as visible
    /// </summary>
    public void ShowWindow()
    {
        Elements_Subfolder.SetActive(true);
    }


    /// <summary>
    /// Set information text
    /// </summary>
    /// <param name="Txt">The text to set for this component</param>
    public void SetInfoText(string Txt)
    {
        GameOverTextControl.text = Txt;
    }
}
