using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Represents an entity in the maze
/// </summary>
public class Entity : MonoBehaviour
{

    [Header("Options")]

    /// <summary>
    /// Number of lives left this entity has
    /// </summary>
    public int LivesLeft = 3;


    /// <summary>
    /// Is this entity the player?
    /// </summary>
    public bool isPlayer
    {
        get
        {
            return this == Player;
        }
    }

    static Entity _Player;

    /// <summary>
    /// The player
    /// </summary>
    public static Entity Player
    {

        get
        {
            if (_Player == null)
            {
                GameObject Player_Obj = GameObject.FindGameObjectWithTag("Player");
                _Player = Player_Obj.GetComponent<Entity>();
            }

            return _Player;

        }
    }


    Transform _Trans;

    /// <summary>
    /// This Entity's transform class
    /// </summary>
    public Transform Transform_Class
    {
        get
        {
            if (_Trans == null) _Trans = transform;
            return _Trans;
        }
    }

    Camera _Cam;

    /// <summary>
    /// This entity's camera
    /// </summary>
    public Camera Entity_Cam
    {
        get
        {
            if (_Cam == null) _Cam = GetComponentInChildren<Camera>();
            return _Cam;
        }
    }


    Transform _Cam_Trans;

    /// <summary>
    /// Transform component of this entity's camera
    /// </summary>
    public Transform Entity_Cam_Transform
    {
        get
        {
            if (_Cam_Trans == null) _Cam_Trans = Entity_Cam.transform;
            return _Cam_Trans;
        }
    }


    CreateMaze _Maze_Handler;


    /// <summary>
    /// Maze handler for accessing current maze data
    /// </summary>
    public CreateMaze Create_Maze
    {
        get
        {
            if (_Maze_Handler == null) _Maze_Handler = FindObjectOfType<CreateMaze>();
            return _Maze_Handler;
        }
    }

    /// <summary>
    /// This entity's world position
    /// </summary>
    public Vector3 World_Position;

    /// <summary>
    /// This entity's original position
    /// </summary>
    public Vector3 Original_Position;

    /// <summary>
    /// This entity's tile position
    /// </summary>
    public Vector2 Tile_Position;


    Rigidbody _RB;

    /// <summary>
    /// Get rigid body component for this entity
    /// </summary>
    public Rigidbody Rigid_Body
    {
        get
        {
            if (_RB == null) _RB = GetComponent<Rigidbody>();
            return _RB;
        }
    }

    PathFinding _Path_Finding;

    /// <summary>
    /// This entity's path finding
    /// </summary>
    public PathFinding Path_Finding
    {
        get
        {
            if (_Path_Finding == null) _Path_Finding = GetComponent<PathFinding>();
            return _Path_Finding;
        }
    }

    NavMeshAgent _Nav_Mesh_Agent;

    /// <summary>
    /// This entity's nav mesh agent controller
    /// </summary>
    public NavMeshAgent Nav_Mesh_Agent
    {
        get
        {
            if (_Nav_Mesh_Agent == null) _Nav_Mesh_Agent = GetComponent<NavMeshAgent>();
            return _Nav_Mesh_Agent;
        }
        set
        {
            _Nav_Mesh_Agent = value;
        }
    }

    /// <summary>
    /// Is this the start button?
    /// </summary>
    bool isStartButton
    {
        get
        {
            return gameObject.CompareTag("Start");
        }
    }

    /// <summary>
    /// Is this object a teleporter (Flipper)?
    /// </summary>
    bool isFlipper
    {
        get
        {
            return gameObject.CompareTag("Flipper");
        }
    }


    [Header("Variables")]

    /// <summary>
    /// Is this entity spawned?
    /// </summary>
    public bool isBehaviorEnabled;

    /// <summary>
    /// Is this entity dead?
    /// </summary>
    public bool isDead
    {
        get
        {
            return LivesLeft <= 0;
        }
    }

    float _Time_Alive;

    /// <summary>
    /// Amount of time that this entity is alive
    /// </summary>
    public float Time_Alive
    {
        get
        {
            return _Time_Alive;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        //If is player, then, load the world
        if (!Create_Maze.isInitialized) LoadWorld();

        //If is not start point entity, then, send to random position
        if (!isStartButton && !isFlipper) SpawnRandomPosition();

        //Set original position
        Original_Position = World_Position;

    }

    void Update()
    {
        //Increment time alive
        if (!isDead) _Time_Alive += Time.deltaTime;

        if (Create_Maze.Build_Nav_Mesh.isActiveAndEnabled && !isBehaviorEnabled)
        {
            //Init path finding
            if (Path_Finding != null) Path_Finding.Init();

            isBehaviorEnabled = true;
        }

        RecordPos();
    }

    /// <summary>
    /// Record current position
    /// </summary>
    void RecordPos()
    {
        World_Position = Transform_Class.position;
        Tile_Position = Create_Maze.WorldPosToTilePos(World_Position);
    }

    //If this is the player, then, load the world.
    void LoadWorld()
    {
        //Initialize
        Create_Maze.Initialize();
    }

    /// <summary>
    /// Try to spawn this object in a corner. Will run 1000 sequences and try to find corner to spawn in before giving up and using the last spot.
    /// </summary>
    public void SpawnInCorner()
    {
        //This will spawn an entity in the corner. It will check to find (first) a place with 1 neighbor.
        //If it can't find a placer with one neighbor, it'll check for a place with two neighbors.
        //If it can't find either, it'll just stay with last spawn.
        int Iterations = 0;
        bool isFound = false;
        int NeighborCount = 0;
        for (int i = 1; i <= 2; i++)
        {
            //If correct spot has not been found then
            if (!isFound)
            {
                while (Iterations < 1000 && !isFound)
                {
                    SpawnRandomPosition();

                    NeighborCount = GetNeighborWalls();
                    if (i == NeighborCount)
                    {
                        isFound = true;
                    }
                    Iterations++;
                }
            }
        }
    }

    /// <summary>
    /// Get number of walls neighboring this entity
    /// </summary>
    /// <returns></returns>
    int GetNeighborWalls()
    {
        //Go through each position in 3x3 square, with tile in center, to count neighbors
        int NeighborCount = 0;
        int Tile_X = (int)Tile_Position.x;
        int Tile_Y = (int)Tile_Position.y;
        int Tile_XE = Tile_X + 1;
        int Tile_XW = Tile_X - 1;
        int Tile_YN = Tile_Y + 1;
        int Tile_YS = Tile_Y - 1;

        int NIndex = CreateMaze.Current.Get(Tile_X, Tile_YN).Index;
        int SIndex = CreateMaze.Current.Get(Tile_X, Tile_YS).Index;
        int EIndex = CreateMaze.Current.Get(Tile_XE, Tile_Y).Index;
        int WIndex = CreateMaze.Current.Get(Tile_XW, Tile_Y).Index;

        if (NIndex != 0) NeighborCount++;
        if (SIndex != 0) NeighborCount++;
        if (EIndex != 0) NeighborCount++;
        if (WIndex != 0) NeighborCount++;

        return NeighborCount;

    }



    /// <summary>
    /// Spawn this entity in a random position. Keep trying until valid position found
    /// </summary>
    public void SpawnRandomPosition()
    {
        //Get random air position
        Vector3 RandomAirPos = Create_Maze.GetRandomEntityPlacementPosition();

        //Place entity here
        SetPosition(RandomAirPos);

        //If is player, then
        if (isPlayer)
        {
            //Move start button to this position
            GameObject Start_Button = GameObject.FindGameObjectWithTag("Start");
            Entity Start_Entity = Start_Button.GetComponent<Entity>();
            Start_Entity.SetPosition(Player.World_Position);

        }

    }

    /// <summary>
    /// Set entity to given map position x,y
    /// </summary>
    /// <param name="Map_Pos">Map position in tile coordinates</param>
    public void SetPosition(Vector2 Map_Pos)
    {
        Vector3 World_Pos = new Vector3(Map_Pos.x * Create_Maze.GetCellSize(), 0, Map_Pos.y * Create_Maze.GetCellSize());
        SetPosition(World_Pos);
    }

    /// <summary>
    /// Set entity to given world coordinates on the map
    /// </summary>
    /// <param name="World_Coords">World coordinates to move this entity to.</param>
    public void SetPosition(Vector3 World_Coords)
    {
        Transform_Class.position = World_Coords;
        RecordPos();
    }

    /// <summary>
    /// Set this entity's height
    /// </summary>
    /// <param name="New_Height"></param>
    public void SetHeight(float New_Height)
    {
        RecordPos();
        SetPosition(new Vector3(World_Position.x, New_Height, World_Position.z));
        RecordPos();
    }


    /// <summary>
    /// Kill this entity
    /// </summary>
    public void Kill()
    {
        LivesLeft = 0;
    }

    /// <summary>
    /// Get distance from this Entity to the given point
    /// </summary>
    /// <param name="Point">The point to get distance to.</param>
    /// <returns>The distance to this point.</returns>
    public float DistanceTo(Vector3 Point)
    {
        return Vector3.Distance(World_Position, Point);
    }

}
