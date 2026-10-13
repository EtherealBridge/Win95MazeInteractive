using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysFaceCamera : MonoBehaviour
{

    [Header("Options")]


    ///<summary>
    /// Distance at which this mesh can be viewed.
    /// </summary>
    public float Visible_Distance;


    Entity _Entity_Class;

    /// <summary>
    /// This Entity's superclass
    /// </summary>
    Entity Entity_Class
    {
        get
        {
            if (_Entity_Class == null) _Entity_Class = GetComponentInParent<Entity>();
            return _Entity_Class;
        }
    }

    Transform _Trans;


    Transform Transform_Component
    {
        get
        {
            if(_Trans == null)
            {
                _Trans = transform;
            }

            return _Trans;
        }
    }

    Camera _Death_Cam;

    /// <summary>
    /// Death camera
    /// </summary>
    Camera Death_Cam
    {
        get
        {
            if (_Death_Cam == null)
            {
                GameObject Death_Cam_Obj = GameObject.FindGameObjectWithTag("DeathCam");
                if (Death_Cam_Obj != null) _Death_Cam = Death_Cam_Obj.GetComponent<Camera>();
            }
            return _Death_Cam;
        }
    }

    /// <summary>
    /// Player shortcut
    /// </summary>
    Entity Player
    {
        get
        {
            return Entity.Player;
        }
    }

    /// <summary>
    /// This entity's distance from the player
    /// </summary>
    float DistanceToPlayer
    {
        get
        {
            return Vector3.Distance(Entity_Class.World_Position, Player.World_Position);
        }
    }

    /// <summary>
    /// Can the player see this entity?
    /// </summary>
    bool isVisibleToPlayer
    {
        get
        {
            return DistanceToPlayer < Visible_Distance;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if(isVisibleToPlayer)
        {
            //Turn this entity towards player this frame
            FacePlayer();
        }    
    
    }



    /// <summary>
    /// Turn this entity towards player
    /// </summary>
    void FacePlayer()
    {

        Transform_Component.LookAt(Player.Entity_Cam_Transform, Vector3.up);
        Vector3 Curr_Rot = Transform_Component.rotation.eulerAngles;
        Curr_Rot.x = 0;
        Curr_Rot.z = 0;
        Transform_Component.rotation = Quaternion.Euler(Curr_Rot);

    }
}
