using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Monster in the maze that kills you
/// </summary>
public class Deacon : MonoBehaviour
{

    [Header("Options")]

    ///<summary>
    /// If Deacon within this distance to player, player either loses life or gets eaten
    /// </summary>
    public float EatDistance;


    public static Deacon _Deacon_Class;

    public static Deacon Current
    {
        get
        {
            if(_Deacon_Class == null)
            {
                _Deacon_Class = FindObjectOfType<Deacon>(true);
            }
            return _Deacon_Class;

        }
    }

    /// <summary>
    /// Unload this item
    /// </summary>
    public static void Unload()
    {
        _Deacon_Class = null;
    }

    Entity _Entity_Class;

    public Entity Entity_Class
    {
        get
        {
            if (_Entity_Class == null) _Entity_Class = GetComponent<Entity>();
            return _Entity_Class;
        }
    }

    public enum WalkingType
    {
        Walking,
        Celebrating,
        Eating0,
        Eating1,
        Eating2,
        Eating3,
        Eating4,
        Eating5,
        Eating6,
        Eating7,
        Eating8,
        Eating9,
        Eating10,
        Eating11
    }

    /// <summary>
    /// Walking materials per walking type
    /// </summary>
    public Material[] Walking_Material;

    /// <summary>
    /// Current material to show
    /// </summary>
    Material Current_Material
    {
        get
        {
            return Walking_Material[(int)Walking_Type];
        }
    }

    /// <summary>
    /// Distance from Deacon to the player
    /// </summary>
    float DistanceToPlayer
    {
        get
        {
            return Vector3.Distance(Entity_Class.World_Position, Entity.Player.World_Position);
        }
    }

    /// <summary>
    /// Is Deacon close enough to eat player?
    /// </summary>
    bool isCloseEnoughToEat
    {
        get
        {
            return DistanceToPlayer < EatDistance;
        }
    }



    /// <summary>
    /// What Deacon is doing at the moment
    /// </summary>
    public WalkingType Walking_Type;


    public bool isEating
    {
        get
        {
            return (int)Walking_Type > 1; 
        }
    }


    /// <summary>
    /// Has Deacon spawned yet? Will be set when Deacon is spawned. Use SpawnDeacon() to spawn Deacon.
    /// </summary>
    bool isSpawned;

    /// <summary>
    /// Returned whether or not Deacon has spawned and is on the game board
    /// </summary>
    public static bool isDeaconSpawned
    { 
        get
        {
            if (Current == null) return false;
            else
            {
                return Current.isSpawned;
            }
        }
    }

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



    MeshRenderer _Mesh_Renderer;

    /// <summary>
    /// The monster's mesh renderer
    /// </summary>
    MeshRenderer Mesh_Renderer
    {
        get
        {
            if (_Mesh_Renderer == null) _Mesh_Renderer = GetComponentInChildren<MeshRenderer>();
            return _Mesh_Renderer;
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
            for (int i = 0; i < Objective_List.Length; i++)
            {
                Objective Obj = Objective_List[i];
                if (Obj.isFinished) Count_Completed++;
            }
            return Count_Completed;
        }
    }

    /// <summary>
    /// Has the player completed all objectives?
    /// </summary>
    public bool All_Objectives_Done
    {
        get
        {
            return NumObjectivesCompleted == 5;
        }
    }

    /// <summary>
    /// Is processing enabled?
    /// </summary>
   bool isEnabled
    {
        get
        {
            return Entity_Class.Nav_Mesh_Agent != null;
        }
    }

    /// <summary>
    /// Death camera
    /// </summary>
    public Camera Death_Cam;


    [Header("Runtime Components")]


    ///<summary>
    /// Particle effect for POOF effect
    /// </summary>
    public GameObject Poof_Particle;



    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("ProgressEating", 0.35F, 0.35F);
    }


    private void Update()
    {
        //If fully loaded, then
        if (isEnabled)
        {
            FreeMoveIfNoObjectives();
            TryEatPlayer();
            SetSpeedBasedOnProgress();
            UpdateImage();
        }
    }


    /// <summary>
    /// Spawn Deacon on the game board
    /// </summary>
    public void Spawn()
    {

        this.gameObject.SetActive(true);
        isSpawned = true;
    }

    /// <summary>
    /// Progress eating animation
    /// </summary>
    void ProgressEating()
    {
        //If walking type is eating or celebrating, then
        if ((int)Walking_Type > (int)WalkingType.Celebrating)
        {

            //Shift state
            int Walking_Int = (int)Walking_Type;
            if((int)Walking_Type < (int)WalkingType.Eating11)
            {
                Walking_Int++;
                Walking_Type = (WalkingType)Walking_Int;
            }
            else if(Walking_Type == WalkingType.Eating11)
            {
                Walking_Type = WalkingType.Celebrating;
            }

            //Do not move
            Entity_Class.Nav_Mesh_Agent.velocity = Vector3.zero;
            Entity_Class.Nav_Mesh_Agent.enabled = false;
        }
    }


    /// <summary>
    /// Deacon tries to eat the player
    /// </summary>
    void TryEatPlayer()
    {

        //If deacon is close enough to player, then
        if(isCloseEnoughToEat && Entity.Player.LivesLeft > 0)
        {
            //Deduct life from player
            Entity.Player.LivesLeft--;

            if(Entity.Player.LivesLeft > 0)
            {
                PlayerLoseLife();
            }
            else
            {
                if(Entity.Player.LivesLeft == 0) EatPlayer();
            }
        }

    }

    /// <summary>
    /// Cause player to lose a life and teleport Deacon to random position
    /// </summary>
    void PlayerLoseLife()
    {
        //Play poof effect
        PlayPoof();

        //Banish Deacon to random position that is far away from the player
        int SpawnIterations = 0;
        float Curr_Dist_Away = 0.0F;
        while (SpawnIterations < 32 && Curr_Dist_Away < 96.0F)
        {
            Entity_Class.SpawnRandomPosition();
            Curr_Dist_Away = Entity_Class.DistanceTo(Entity.Player.World_Position);
            SpawnIterations++;
        }
        

        //Show hurt indicator
        HurtIndicator H = FindObjectOfType<HurtIndicator>();
        H.Flash();


    }

    /// <summary>
    /// Play poof effects
    /// </summary>
    public void PlayPoof()
    {
        //Player poof effect
        GameObject Poof_Obj = Instantiate(Poof_Particle.gameObject, Entity_Class.World_Position, Quaternion.identity);
    }


    void EatPlayer()
    {
        //Kill player
        Entity.Player.Kill();

        //Play Deacon animation for eating player
        Walking_Type = WalkingType.Eating0;

        //Look at Deacon
        Entity.Player.Entity_Cam.enabled = false;
        Death_Cam.gameObject.SetActive(true);
        Death_Cam.transform.LookAt(transform.position + Vector3.up);
        Death_Cam.transform.SetParent(null);

        //Force velocity zero and disable nav mesh
        Move M = GetComponent<Move>();
        M.DeadStop();
        Entity_Class.Nav_Mesh_Agent.enabled = false;

    }

    /// <summary>
    /// Deacon gets faster with more objective progress
    /// </summary>
    void SetSpeedBasedOnProgress()
    {
        switch(NumObjectivesCompleted)
        {
            case 2:
                Entity_Class.Nav_Mesh_Agent.speed = 4F;
                break;
            case 3:
                Entity_Class.Nav_Mesh_Agent.speed = 6F;
                break;
            case 4:
                Entity_Class.Nav_Mesh_Agent.speed = 8F;
                break;
            case 5:
                Entity_Class.Nav_Mesh_Agent.speed = 4F;
                break;
        }   
        
    }

    ///<summary>
    /// Update deacon's image based on state
    /// </summary>
    void UpdateImage()
    {
        Mesh_Renderer.sharedMaterial = Current_Material;
    }

    /// <summary>
    /// If no objectives left then just free move
    /// </summary>
    void FreeMoveIfNoObjectives()
    {
        if(All_Objectives_Done && Entity_Class.Path_Finding.Behavior_Type == PathFinding.BehaviorType.GoToPlayer)
        {
            Entity_Class.Path_Finding.Behavior_Type = PathFinding.BehaviorType.Wander;
            Entity_Class.Path_Finding.Init();
        }
    }
}
