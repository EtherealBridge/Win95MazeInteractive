using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows player to do an objective
/// </summary>
public class PlayerDoObjective : MonoBehaviour
{

    /// <summary>
    /// Layers for doing tasks
    /// </summary>
    public LayerMask Objective_Mask;

    Entity _Entity_Class;


    ObjectiveProgress _Objective_Progress;

    /// <summary>
    /// Objective progress
    /// </summary>
    ObjectiveProgress Objective_Progress
    {
        get
        {
            if (_Objective_Progress == null) _Objective_Progress = FindObjectOfType<ObjectiveProgress>();
            return _Objective_Progress;
        }
    }


    /// <summary>
    /// This entity's superclass
    /// </summary>
    Entity Entity_Class
    {
        get
        {
            if (_Entity_Class == null) _Entity_Class = GetComponent<Entity>();
            return _Entity_Class;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        //If space bar held down then
        if(Input.GetKey(KeyCode.Space))
        {
            //As player, do task
            DoTask();
        }

    }

    /// <summary>
    /// Do task if one is in front of you
    /// </summary>
    void DoTask()
    {
        //Do raycast
        RaycastHit Hit;
        Ray R = Entity_Class.Entity_Cam.ScreenPointToRay(new Vector3(Screen.width / 2F, Screen.height / 2F, 0F));
        if(Physics.Raycast(R, out Hit, 3.0F, Objective_Mask))
        {
            GameObject Hit_Obj = Hit.transform.gameObject;
            Objective O = Hit_Obj.GetComponentInParent<Objective>();
            if(O != null)
            {
                O.Do();
            }
        }
    }
}
