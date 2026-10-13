using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskList : MonoBehaviour
{


    [Header("Runtime Components")]


    ///<summary>
    /// Control for listing in-game tasks
    /// </summary>
    public ListBox Task_List;


    Objective[] _Objs;

    /// <summary>
    /// Current objectives in the game
    /// </summary>
    Objective[] All_Objectives
    {
        get
        {
            //If list not initialized, then
            if(_Objs == null)
            {
                _Objs = FindObjectsOfType<Objective>();
            }

            return _Objs;
        }
    }


    // Update is called once per frame
    void Update()
    {
        //If task list is empty, then, all current objectives
        if(Task_List.Count() == 0)
        {
            AddAllObjsToList();
        }
        else
        {
            //Show completed objectives as green
            ShowCompleteObjectivesAsGreen();
        }
    }

    /// <summary>
    /// Add items to list
    /// </summary>
    void AddAllObjsToList()
    {
        //Clear list
        Task_List.Clear();

        //Add items
        for(int i = 0; i < All_Objectives.Length; i++)
        {
            Objective Obj = All_Objectives[i];

            //Add
            Task_List.AddOption(Obj.Objective_Text, Obj);
        }
    }

    /// <summary>
    /// Show objectives as green
    /// </summary>
    void ShowCompleteObjectivesAsGreen()
    {
        //For each task that is complete, show as green
        for(int i = 0; i < Task_List.Count(); i++)
        {
            Objective Obj = (Objective)Task_List.GetValueAtIndex(i);
            if(Obj.isFinished)
            {
                SetComplete(i);
            }
        }
    }

    
    /// <summary>
    /// Set task ID complete
    /// </summary>
    /// <param name="TaskID">Index of task to complete</param>
    void SetComplete(int TaskID)
    {
        //Get option by task index
        GameObject Obj = Task_List.GetOption(TaskID);
        Image Img = Obj.GetComponent<Image>();
        Img.enabled = true;


    }

    
}
