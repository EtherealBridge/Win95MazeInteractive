using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shows this UI element if player is within range of an objective
/// </summary>
public class ShowIfInObjectRange : MonoBehaviour
{
    /// <summary>
    /// Window to show or hide depending on if player is in range or not.
    /// </summary>
    public GameObject Window;

    Objective[] _All_Objectives;

    /// <summary>
    /// All objectives to complete in the map
    /// </summary>
    public Objective[] All_Objectives
    {
        get
        {
            if(_All_Objectives == null)
            {
                _All_Objectives = new Objective[0];
            }
            if (_All_Objectives.Length == 0)
            {
                _All_Objectives = FindObjectsByType<Objective>(FindObjectsSortMode.InstanceID);
            }
            return _All_Objectives;
        }
    }


    public float RefreshRate = 0.75F;
    float TimeSinceRefresh = 0.0F;

    // Update is called once per frame
    void Update()
    {
        if (TimeSinceRefresh >= RefreshRate)
        {
            //Determine if an objective is in range
            bool isInRange = false;
            for (int i = 0; i < All_Objectives.Length; i++)
            {
                Objective Curr_Obj = All_Objectives[i];
                if (!Curr_Obj.isFinished)
                {
                    float Dist = Vector3.Distance(Entity.Player.World_Position, Curr_Obj.transform.position);
                    if (Dist < 4.0F)
                    {
                        isInRange = true;
                    }
                }
            }

            //If an objective is in range, then, show dialog. Otherwise hide.
            Window.SetActive(isInRange);

            TimeSinceRefresh = 0.0F;
        }
        else
        {
            TimeSinceRefresh += Time.deltaTime;
        }



    }
}
