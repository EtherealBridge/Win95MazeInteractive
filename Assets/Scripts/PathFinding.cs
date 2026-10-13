using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Pathfinding for entities
/// </summary>
public class PathFinding : MonoBehaviour
{

    Entity _Entity_Class;

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

    /// <summary>
    /// AI Behavior Types
    /// </summary>
    public enum BehaviorType
    {
        Wander,
        GoToPlayer,
        FreeMove
    }


    [Header("Options")]


    ///<summary>
    /// Type of behavior this entity has
    /// </summary>
    public BehaviorType Behavior_Type;

    /// <summary>
    /// Target position for this entity to move to.
    /// </summary>
    public Vector3 Target_Position;


    Entity _Player;

    /// <summary>
    /// The player on the map
    /// </summary>
    Entity Player
    {
        get
        {
            if(_Player == null)
            {
                GameObject Player_Obj = GameObject.FindGameObjectWithTag("Player");
                _Player = Player_Obj.GetComponent<Entity>();
            }
            return _Player;
        }
    }

    /// <summary>
    /// Shortcut for nav mesh agent
    /// </summary>
    NavMeshAgent Nav_Mesh_Agent
    {
        get
        {
            return Entity_Class.Nav_Mesh_Agent;
        }
    }


    /// <summary>
    /// Shortcut for maze handler
    /// </summary>
    CreateMaze Create_Maze
    {
        get
        {
            return Player.Create_Maze;
        }
    }

    /// <summary>
    /// Is path finding initialized?
    /// </summary>
    public bool isInitialized;

    /// <summary>
    /// Initialize pathfinding
    /// </summary>
    public void Init()
    {

        if (Behavior_Type != BehaviorType.FreeMove)
        {
            //Add nav mesh component
            if (Entity_Class.Nav_Mesh_Agent == null)
            {
                Entity_Class.Nav_Mesh_Agent = gameObject.AddComponent<NavMeshAgent>();
            }
            Entity_Class.Nav_Mesh_Agent.speed = 12F;

            //Set this entity's initial target
            SetEntityTarget();


            //If behavior is wandering, then, keep setting new target
            if (Behavior_Type == BehaviorType.Wander)
            {
                InvokeRepeating("SetEntityTarget", 16F, 16F);
            }
        }
    }

    private void Update()
    {
        //Set to go to player
       if(Behavior_Type == BehaviorType.GoToPlayer)
        {
            if (gameObject.activeSelf)
            {
                SetEntityTarget();
            }
        }
    }

    /// <summary>
    /// Set this entity's target based on their behavior
    /// </summary>
    void SetEntityTarget()
    {
        if (Behavior_Type == BehaviorType.Wander)
        {
            Target_Position = Create_Maze.GetRandomEntityPlacementPosition();
        }
        else if (Behavior_Type == BehaviorType.GoToPlayer)
        {
            Target_Position = Player.World_Position;
        }

        //Go to
        if (Nav_Mesh_Agent != null)
        {
            if (Nav_Mesh_Agent.enabled)
            {
                Nav_Mesh_Agent.destination = Target_Position;
                Nav_Mesh_Agent.isStopped = false;
            }
        }
    }
}
