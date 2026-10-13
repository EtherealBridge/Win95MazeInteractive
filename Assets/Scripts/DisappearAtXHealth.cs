using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Causes this item to disappear if player has X health
/// </summary>
public class DisappearAtXHealth : MonoBehaviour
{

    /// <summary>
    /// Hide this item if below this number of lives
    /// </summary>
    public int HideBelowLives;

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

    /// <summary>
    /// is this item visible?
    /// </summary>
    bool isVisible
    {
        get
        {
            return Player.LivesLeft >= HideBelowLives;
        }
    }



    // Update is called once per frame
    void Update()
    {
        gameObject.SetActive(isVisible);
    }
}
