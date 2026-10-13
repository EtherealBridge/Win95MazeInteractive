using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugControls : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                ForceCompleteAllObjectives(true);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                CompleteAnObjective();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                PutDeaconNearPlayer();
            }

        }
    }


    void ForceCompleteAllObjectives(bool isComplete)
    {
        Objective[] Objective_List = FindObjectsByType<Objective>(FindObjectsSortMode.InstanceID);
        for (int i = 0; i < Objective_List.Length; i++)
        {
            Objective Obj = Objective_List[i];
            float Time = 0.0F;
            if (!isComplete) Time = 1.0F;
            Obj.TimeToComplete = Time;
        }
    }

    /// <summary>
    /// Complete the next objective in the list
    /// </summary>
    void CompleteAnObjective()
    {
        Objective[] Objective_List = FindObjectsByType<Objective>(FindObjectsSortMode.InstanceID);
        bool isDone = false;
        for (int i = 0; i < Objective_List.Length; i++)
        {
            if (!isDone)
            {
                Objective Obj = Objective_List[i];
                if (Obj.TimeToComplete > 0.0F)
                {
                    Obj.TimeToComplete = 0.0F;
                    isDone = true;
                }
            }
        }
    }

    //Respawns Deacon until he is near the player
    void PutDeaconNearPlayer()
    {
        if(!Deacon.isDeaconSpawned)
        {
            Deacon.Current.Spawn();
        }
        int Iterations = 0;
        float Deacon_Dist = 0.0F;
        while (Iterations < 10000 && !(Deacon_Dist >= 4.0F && Deacon_Dist <= 16.0F))
        {
            Deacon.Current.Entity_Class.SpawnRandomPosition();
            Deacon_Dist = Deacon.Current.Entity_Class.DistanceTo(Entity.Player.World_Position);
            Iterations++;
        }    
    }
}
