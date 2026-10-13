using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Make Deacon appear after objectives started or time limit reached
/// </summary>
public class DeaconAppearAfterXObjs : MonoBehaviour
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

    [Header("Options")]

    ///<summary>
    /// Appear after this many seconds
    /// </summary>
    public float AppearAfterTime;

    /// <summary>
    /// Make monster appear after X number of objectives completed
    /// </summary>
    public int AppearAfterObjectivesCompleted;


    [Header("Runtime Components")]

    ///<summary>
    /// Monster to be added to the maze
    /// </summary>
    public Deacon Deacon_Entity;


    Objective[] _Objective_List;


    /// <summary>
    /// List of objectives
    /// </summary>
    Objective[] Objective_List
    {
        get
        {
            if (_Objective_List == null) _Objective_List = FindObjectsOfType<Objective>();
            return _Objective_List;
        }
    }

    /// <summary>
    /// Number of objectives completed
    /// </summary>
    public int NumObjectivesCompleted
    {
        get
        {
            int Count_Completed = 0;
            for(int i = 0; i < Objective_List.Length; i++)
            {
                Objective Obj = Objective_List[i];
                if (Obj.isFinished) Count_Completed++;
            }
            return Count_Completed;
        }
    }


    // Update is called once per frame
    void Update()
    {

        //Has Deacon spawned?
        if (!Deacon.isDeaconSpawned)
        {

            //If player has exceeded appear after time, then, make Deacon appear
            if (Player.Time_Alive > AppearAfterTime)
            {
                Deacon.Current.Spawn();               
            }

            //if threshold of objectives completed, then, show monster
            if (NumObjectivesCompleted >= 2)
            {
                Deacon.Current.Spawn();
            }
        }

    }





}
