using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchToEndGoal : MonoBehaviour
{
    [Header("Runtime Components")]

    ///<summary>
    /// The final goal in the game
    /// </summary>
    public EndGoal Final_Goal;


    Objective[] _Objs;

    /// <summary>
    /// Current objectives in the game
    /// </summary>
    Objective[] All_Objectives
    {
        get
        {
            //If list not initialized, then
            if (_Objs == null)
            {
                _Objs = FindObjectsOfType<Objective>();
            }

            return _Objs;
        }
    }

    /// <summary>
    /// Are all normal objectives finished?
    /// </summary>
    public bool allNormalFinished
    {
        get
        {
            bool isFinished = true;
            for(int i = 0; i < All_Objectives.Length; i++)
            {
                Objective Obj = All_Objectives[i];
                if (!Obj.FinalObjective)
                {
                    if (!Obj.isFinished) isFinished = false;
                }
            }
            return isFinished;
        }
    }

    void Start()
    {
        InvokeRepeating("CheckForRevealFinalGoal", 2.0F, 2.0F);
    }



    /// <summary>
    /// Check if final goal should be shown
    /// </summary>
    void CheckForRevealFinalGoal()
    {
        if(allNormalFinished)
        {
            Final_Goal.gameObject.SetActive(true);
        }
    }
}
