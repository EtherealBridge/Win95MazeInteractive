using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Code for maze flipper objects
/// </summary>
public class Flipper : MonoBehaviour
{

    public static Flipper[] _All_Flippers;

    /// <summary>
    /// All flippers on the game board
    /// </summary>
    public static Flipper[] All_Flippers
    {
        get
        {
            if(_All_Flippers == null)
            {
                _All_Flippers = FindObjectsByType<Flipper>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
            }

            return _All_Flippers; 
        }
    }

    /// <summary>
    /// Get a random flipper from the list of Flippers
    /// </summary>
    /// <returns></returns>
    public static Flipper GetRandom()
    {
        int FlipperCount = All_Flippers.Length;
        int RandNum = Random.Range(0, FlipperCount);
        return All_Flippers[RandNum];
    }

    /// <summary>
    /// Causes all flippers to restart enabling process. Will idle for 16 seconds while still remaining visible.
    /// </summary>
    public static void FreezeAll()
    {
        foreach(Flipper F in All_Flippers)
        {
            F.Freeze();
        }
    }

    public static void Unload()
    {
        _All_Flippers = null;
    }

    Entity _Entity_Class;

    /// <summary>
    /// This Entity's properties
    /// </summary>
    public Entity Entity_Class
    {
        get
        {
            if(_Entity_Class == null)
            {
                _Entity_Class = GetComponentInParent<Entity>();
            }
            return _Entity_Class;
        }
    }

    Transform _Local_Transform;

    /// <summary>
    /// Flipper local object transform
    /// </summary>
    Transform Local_Transform
    {
        get
        {
            if (_Local_Transform == null)
            {
                _Local_Transform = transform;
            }
            return _Local_Transform;
        }
    }

    /// <summary>
    /// Start disabled and wait 16 seconds to show / be active
    /// </summary>
    public bool isEnabled
    {
        get
        {
            return UpTime >= 16.0F;
        }
    }

    /// <summary>
    /// This flipper's uptime
    /// </summary>
    float UpTime;

    /// <summary>
    /// The distance that the player is away from this flipper
    /// </summary>
    float Player_Distance
    {
        get
        {
            return Entity.Player.DistanceTo(Entity_Class.World_Position);
        }
    }


    [Header("Options")]

    ///<summary>
    /// Rotation animation speed of this Flipper
    ///</summary>
    public float Rotation_Speed;

    Vector3 _Rotation_Direction;

    /// <summary>
    /// Rotation animation direction of the Flipper
    /// </summary>
    public Vector3 Rotation_Direction
    {
        get
        {
            if (_Rotation_Direction == Vector3.zero)
            {
                float Rand_X = Random.Range(0.0F, 365.0F);
                float Rand_Y = Random.Range(0.0F, 365.0F);
                float Rand_Z = Random.Range(0.0F, 365.0F);
                _Rotation_Direction = new Vector3(Rand_X, Rand_Y, Rand_Z);
            }
            return _Rotation_Direction;
        }
    }


    // Update is called once per frame
    void Update()
    {
        //If processing is enabled, then,
        if(isEnabled)
        {
            //Show this flipper when it is first activated.
            if(!gameObject.activeSelf) gameObject.SetActive(true);

            TeleportPlayerIfClose();

            RotateThisFrame();
        }
        
        UpTime += Time.deltaTime;
    }


    /// <summary>
    /// If the player is close enough then teleport them to a new area.
    /// </summary>
    void TeleportPlayerIfClose()
    {
        if(Player_Distance <= 4.0F)
        {
            Flipper Next = FindAnotherFlipper();
            Flipper.FreezeAll();
            Entity.Player.SetPosition(Next.Entity_Class.World_Position);            
        }
    }

    /// <summary>
    /// Finds another Flipper to teleport to.
    /// </summary>
    /// <returns>A Flipper to teleport to.</returns>
    Flipper FindAnotherFlipper()
    {
        Flipper Found_Flipper = this;
        while(Found_Flipper == this)
        {
            Found_Flipper = Flipper.GetRandom();

        }

        return Found_Flipper;
    }
    
    /// <summary>
    /// Deactivate this flipper momentarily
    /// </summary>
    public void Freeze()
    {
        UpTime = 0.0F;
    }

    /// <summary>
    /// Plays the rotation animation this frame for this flipper
    /// </summary>
    void RotateThisFrame()
    {
        Vector3 RotationThisFrame = Rotation_Direction * Time.deltaTime * Rotation_Speed;
        Rotate(RotationThisFrame);
    }

    /// <summary>
    /// Rotate this entity by (Euler_Rotation) units
    /// </summary>
    /// <param name="Euler_Rotation">The units to rotate this entity by THIS FRAME.</param>
    public void Rotate(Vector3 Euler_Rotation)
    {
        Local_Transform.localRotation = Quaternion.Euler(Local_Transform.localRotation.eulerAngles + Euler_Rotation);
    }
}
