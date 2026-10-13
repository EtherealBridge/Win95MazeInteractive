using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hide this gameobject if player is dead
/// </summary>
public class HideIfPlayerDead : MonoBehaviour
{
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
        //If player is dead, then
        if(Player.isDead)
        {
            gameObject.SetActive(false);
        }
    }

}
