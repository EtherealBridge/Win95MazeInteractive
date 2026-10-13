using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Deacon's walk animation. Causes moving entity to bounce up and down when walking.`
/// </summary>
public class DeaconWalkAnim : MonoBehaviour
{

    Entity _Entity_Class;

    Entity Entity_Class
    {
        get
        {
            if (_Entity_Class == null) _Entity_Class = GetComponent<Entity>();
            return _Entity_Class;
        }
    }


    /// <summary>
    /// Jumping height for entity
    /// </summary>
    float JumpHeight = 1F;

    /// <summary>
    /// Jump multiplier
    /// </summary>
    float JumpMulti = 3F;

    float Actual_Height = 0F;

    /// <summary>
    /// Which way to increase / decrease height
    /// </summary>
    float Multi_Amt = 1F;


    Deacon _Deacon_Class;

    /// <summary>
    /// Deacon class
    /// </summary>
    Deacon Deacon_Class
    {
        get
        {
            if (_Deacon_Class == null) _Deacon_Class = GetComponent<Deacon>();
            return _Deacon_Class;
        }
    }

    /// <summary>
    /// Position this frame for this entity
    /// </summary>
    Vector3 Position_This_Frame
    {
        get
        {
            return Entity_Class.World_Position;
        }
    }


    bool Position_Changed_This_Frame
    {
        get
        {
            return Position_This_Frame != Position_Last_Frame;
        }
    }


    Vector3 Position_Last_Frame;

    // Update is called once per frame
    void Update()
    {

        //Change amount of jumping
        float ChgAmt = JumpHeight * Time.deltaTime * JumpMulti;

        if ((Position_Changed_This_Frame && Deacon_Class.Walking_Type == Deacon.WalkingType.Walking) || Deacon_Class.Walking_Type == Deacon.WalkingType.Celebrating)
        {
            //Amount to change actual height by
            Actual_Height += ChgAmt * Multi_Amt;

            if (Actual_Height <= 0.0F)
            {
                Multi_Amt = 1F;
                Actual_Height = 0F;
            }
            if (Actual_Height >= JumpHeight)
            {
                Multi_Amt = -1F;
                Actual_Height = JumpHeight;
            }
        }
        else
        {
            Multi_Amt = -1F;
            Actual_Height += ChgAmt * Multi_Amt;

            if (Actual_Height <= 0F) Actual_Height = 0F;
        }

        Position_Last_Frame = Position_This_Frame;

        //Set height position
        Entity_Class.SetHeight(Actual_Height);
    }
}
