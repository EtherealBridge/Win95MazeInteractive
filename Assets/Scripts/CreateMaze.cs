using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMaze : MonoBehaviour
{

    public static CreateMaze _Current;

    public static CreateMaze Current
    {
        get
        {
            if(_Current == null)
            {
                _Current = FindObjectOfType<CreateMaze>();
            }
            return _Current;
        }
    }

    public static void Unload()
    {
        _Current = null;
    }


    [Header("Options")]

    ///<summary>
    /// Size of the maze to create
    /// </summary>
    public Vector2 Maze_Size;

    /// <summary>
    /// Number of maze paths to create
    /// </summary>
    public int Number_Of_Paths;

    /// <summary>
    /// Path minimum size
    /// </summary>
    public int Path_Min_Size;

    /// <summary>
    /// Path maximum size
    /// </summary>
    public int Path_Max_Size;

    /// <summary>
    /// Maze generation seed
    /// </summary>
    public double Seed;

    /// <summary>
    /// Power for simplex noise when generating paths
    /// </summary>
    public double PathPower;

    /// <summary>
    /// Scale for simplex noise when generating paths
    /// </summary>
    public double PathScale;

    /// <summary>
    /// Number of tasks to set
    /// </summary>
    public int Tasks;

    /// <summary>
    /// Amount of OpenGL Special tiles to place
    /// </summary>
    public int OpenGLCount;

    /// <summary>
    /// Number of Unity3D Posters to create
    /// </summary>
    public int UnityPosterCount;

    /// <summary>
    /// Number of LGBT posters to create
    /// </summary>
    public int LGBTPosterCount;

    /// <summary>
    /// Size of each cell
    /// </summary>
    int Cell_Size = 4;

    /// <summary>
    /// Ceiling height for maze
    /// </summary>
    int Ceiling_Height = 3;

    /// <summary>
    /// Array of all walls in the maze based on maze size
    /// </summary>
    Wall[,] All_Walls;

    [Header("Pieces")]

    ///<summary>
    /// Mesh filter for floor
    /// </summary>
    public MeshFilter Level;

    Mesh Level_Mesh;

    /// <summary>
    /// Mesh collider for collision
    /// </summary>
    public MeshCollider Mesh_Collider;


    Entity _Player;

    /// <summary>
    /// The in-game player
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


    BuildNavMesh _Build_Nav_Mesh;

    /// <summary>
    /// Class for building nav mesh
    /// </summary>
    public BuildNavMesh Build_Nav_Mesh
    {
        get
        {
            if (_Build_Nav_Mesh == null) _Build_Nav_Mesh = GetComponent<BuildNavMesh>();
            return _Build_Nav_Mesh;
        }
    }

    /// <summary>
    /// Is the maze initialized?
    /// </summary>
    public bool isInitialized;


    /// <summary>
    /// Initialize maze
    /// </summary>
    public void Initialize()
    {
        All_Walls = new Wall[(int)Maze_Size.x, (int)Maze_Size.y];

        //Set seed based on time
        Seed = UnityEngine.Random.Range(0, int.MaxValue);

        //Set meshes
        Level_Mesh = new Mesh();
        Level.sharedMesh = Level_Mesh;

        //Create paths
        CreatePaths();

        //Place special posters and items
        PlaceSpecialTilesAndObjects();

        //Build new maze
        Build();

        //Build nav mesh
        Build_Nav_Mesh.Build();

        isInitialized = true;

    }

    /// <summary>
    /// Create air paths to form a maze 
    /// </summary>
    void CreatePaths()
    {
        //Get starting position for generation
        Vector2 StartPos = new Vector2(Maze_Size.x / 2f, Maze_Size.y / 2F);

        //Get list of start points of worms. Will be used to link air tiles together to avoid islands.
        List<Vector2> StartPoints = new List<Vector2>();

        //For number of paths,
        for (int i = 0; i < Number_Of_Paths; i++)
        {
            //Seed for generation
            int Path_Seed = (int)Seed + i;

            //Get random worm size based on noise and current seed
            UnityEngine.Random.InitState(Path_Seed);
            int WormSize = UnityEngine.Random.Range(Path_Min_Size, Path_Max_Size);

            ////Get randomized worm starting point based on seed.
            int Worm_Start_X = (int)UnityEngine.Random.Range(0, All_Walls.GetLength(0) - 1);
            int Worm_Start_Y = (int)UnityEngine.Random.Range(0, All_Walls.GetLength(1) - 1);

            //Start position - set as air (-1)
            Vector2 Current_Write_Position = new Vector2(Worm_Start_X, Worm_Start_Y);
            SetIndex(Worm_Start_X, Worm_Start_Y, -1);

            //Last turn direction. Algorithm is prevented from doing exact opposite of last move to avoid wasting worm iterations.
            Vector2 LastTurnDir = Vector2.zero;

            //Add this start point to the list
            StartPoints.Add(Current_Write_Position);

            //For each iteration of worm size, get worm direction
            for (int w = 1; w < WormSize; w++)
            {
                //Get turn direction for worm
                Vector2 TurnDir = SimplexTurnDir(Path_Seed, (int)Current_Write_Position.x, (int)Current_Write_Position.y);

                //Potential turn pos
                Vector2 Potential_Pos = Current_Write_Position + TurnDir;
                bool isOB = (Potential_Pos.x < 0 || Potential_Pos.x >= All_Walls.GetLength(0) ||
                    Potential_Pos.y < 0 || Potential_Pos.y >= All_Walls.GetLength(0));

                //While turn direction is not exact opposite of other turn, keep getting new turn direction
                Vector2 Opposite_Last_Turn = LastTurnDir * new Vector2(-1, -1);
                int WhileIterator = 0;
                while (TurnDir == Opposite_Last_Turn && WhileIterator < 8)
                {
                    TurnDir = SimplexTurnDir(Path_Seed, (int)Current_Write_Position.x * WhileIterator, (int)Current_Write_Position.y * WhileIterator);
                    WhileIterator++;
                }
                WhileIterator = 0;
                while(isOB && WhileIterator < 8)
                {
                    Potential_Pos = Current_Write_Position + TurnDir;
                    isOB = (Potential_Pos.x < 0 || Potential_Pos.x >= All_Walls.GetLength(0) ||
Potential_Pos.y < 0 || Potential_Pos.y >= All_Walls.GetLength(1));
                    TurnDir = SimplexTurnDir(Path_Seed, (int)Current_Write_Position.x * WhileIterator, (int)Current_Write_Position.y * WhileIterator);
                    WhileIterator++;
                }



                //Set new write position, as we passed algorithm testing.
                Current_Write_Position += TurnDir;

                //Write air index
                SetIndex((int)Current_Write_Position.x, (int)Current_Write_Position.y, -1);

                //Record last turn direction for next loop iteration
                LastTurnDir = TurnDir;

            }
        }

        //Clean up islands for each starting worm point. Connect start points to their closest start point neighbor
        CleanupIslands(StartPoints);

    }

    /// <summary>
    /// Place all special tiles and flags
    /// </summary>
    void PlaceSpecialTilesAndObjects()
    {
        //Count of items left
        int OpenGLLeft = OpenGLCount;
        int UnityPosterLeft = UnityPosterCount;
        int LGBTPosterLeft = LGBTPosterCount;

        int Iterator = 128;

        //While open GL posters left to place
        while(OpenGLLeft > 0 && Iterator > 0)
        {
            if(SetRandomWallSpecial(1))
            {
                OpenGLLeft -= 1;
            }

            Iterator -= 1;
        }

        Iterator = 128;

        //While Unity posters left to place
        while (UnityPosterLeft > 0 && Iterator > 0)
        {
            if (SetRandomWallSpecial(2))
            {
                UnityPosterLeft -= 1;
            }

            Iterator -= 1;
        }

        Iterator = 128;

        //While LGBT posters left to place
        while (LGBTPosterLeft > 0 && Iterator > 0)
        {
            if (SetRandomWallSpecial(3))
            {
                LGBTPosterLeft -= 1;
            }

            Iterator -= 1;
        }

        //Place start button
        PlaceStartButton();

        //Place flippers on the map
        PlaceFlippers();

    }

    /// <summary>
    /// Sets wall to special index (posters) if it can. Return if was set or not.
    /// </summary>
    /// <param name="IndexToSet"></param>
    /// <returns></returns>
     bool SetRandomWallSpecial(int IndexToSet)
    {
        //Get x and y to try placement
        int x = UnityEngine.Random.Range(0, (int)Maze_Size.x);
        int y = UnityEngine.Random.Range(0, (int)Maze_Size.y);

        //Get Wall
        Wall W = Get(x, y);

        //If wall is not airt, then do not try to set
        if (W.Index != -1) return false;

        //Check if has wall at least on one side
        Wall WN = Get(x, y + 1);
        Wall WE = Get(x + 1, y);
        Wall WS = Get(x, y - 1);
        Wall WW = Get(x - 1, y);

        //Is wall on least one side?
        bool hasWall = (WN.Index != -1 || WE.Index != -1 || WS.Index != -1 || WW.Index != -1);

        //If has wall, then, replace and deduct counter
        if (hasWall)
        {
            SetIndex(x, y, IndexToSet);

            //if(IndexToSet == 1) Debug.DrawLine(new Vector3(x * Cell_Size, 0, y * Cell_Size), new Vector3(x * Cell_Size, 32, y * Cell_Size), Color.blue, 120F);
            //Debug.Log("PLACED!");
        }

        return hasWall;
    }

    /// <summary>
    /// Get simplex turn direction for a given seed and index in worm iteration. 
    /// Snaps to North (0), East (1), South (2), or West (3)
    /// </summary>
    /// <param name="Seed">Seed for generation</param>
    /// <param name="x">Write direction x</param>
    /// <param name="y">Write direction y</param>
    /// <returns></returns>
    Vector2 SimplexTurnDir(int Seed, int x, int y)
    {
        //0 = North, 1 = East, 2 = South, 3 = West        
        int turndir = (int)PerlinNoise.FCValue(0, x, y, Seed, PathScale, 4, PathPower);

        switch (turndir)
        {
            case 1:
                return new Vector2(1, 0);
            case 2:
                return new Vector2(0, -1);
            case 3:
                return new Vector2(-1, 0);

            default:
                //Worm moves north
                return new Vector2(0, 1);
        }

    }

    /// <summary>
    /// Clean up island worms, as all worms should connect to each other. 
    /// Done by getting closest nearby start point that is not related to this worm. 
    /// </summary>
    /// <param name="StartPoints">Each start point, which will be tested against other start points.</param>
    void CleanupIslands(List<Vector2> StartPoints)
    {
        //If island makes a connection, then, it cannot be connected to.
        List<Vector3> Used_Start_Points = new List<Vector3>();

        //For each start point
        for (int i = 0; i < StartPoints.Count; i++)
        {

            //Current starting point in the list
            Vector2 Curr_Start_Point = StartPoints[i];

            //Winning point when tested against this start point. Also, cache winning distance to avoid that unnecessary overhead.
            Vector2 Winning_Point = Vector2.zero;
            float Winning_Dist = 9999F;

            //Test against list of start points that are not this start point. Grab nearest start point and link to that.
            for (int a = 0; a < StartPoints.Count; a++)
            {
                //Get start point to test against this one. Do not test start points against eachother/
                if (i != a)
                {
                    Vector2 Test_Pt = StartPoints[a];

                    if (!Used_Start_Points.Contains(Test_Pt))
                    {

                        //Get distance from current point to test point
                        float Dist = Vector2.Distance(Curr_Start_Point, Test_Pt);

                        //Set as winning point
                        if (Dist < Winning_Dist)
                        {
                            Winning_Point = Test_Pt;
                            Winning_Dist = Dist;

                            Used_Start_Points.Add(Curr_Start_Point);
                        }
                    }
                }
            }

            //Walk generator to this point if doesn't equal itself
            if (Winning_Point != Curr_Start_Point)
            {
                //Carve out path from this worm's start point to nearest worm start point
                WalkTo(i, Curr_Start_Point, Winning_Point);

                //Vector3 Pt1 = new Vector3(Curr_Start_Point.x * Cell_Size, 0, Curr_Start_Point.y * Cell_Size);
                //Vector3 Pt2 = new Vector3(Winning_Point.x * Cell_Size, 0, Winning_Point.y * Cell_Size);
                //Debug.DrawLine(Pt1, Pt2, Color.red, 120F);
            }

        }
    }

    /// <summary>
    /// Carve out air path from start point to end point
    /// </summary>
    /// <param name="StartPointIteration">Instance index of this worm.</param>
    /// <param name="Start">Starting point to start carving out air spaces.</param>
    /// <param name="End">Ending point to stop carving out air spaces.</param>
    void WalkTo(int StartPointIteration, Vector2 Start, Vector2 End)
    {
        //Seed for generation
        int Path_Seed = (int)Seed + StartPointIteration;

        //Get random worm size based on noise and current seed
        UnityEngine.Random.InitState(Path_Seed);

        //Position to start marching at
        Vector2 Curr_Pos = Start;

        //Iterate until no movements left
        while (Curr_Pos != End)
        {

            //Get x and y left to move. Can only move x or y in one iteration.
            int X_Dir = (int)End.x - (int)Curr_Pos.x;
            int Y_Dir = (int)End.y - (int)Curr_Pos.y;

            //Normalize
            if (X_Dir > 0) X_Dir = 1;
            else if (X_Dir < 0) X_Dir = -1;
            else X_Dir = 0;

            if (Y_Dir > 0) Y_Dir = 1;
            else if (Y_Dir < 0) Y_Dir = -1;
            else Y_Dir = 0;

            //Move direction for this iteration
            Vector2 Move_Dir = Vector2.zero;

            //Get x movmement. If not moving x, then, move Y;
            int X_Move = UnityEngine.Random.Range(0, Mathf.Abs(X_Dir) + 1);
            int Y_Move = 1 - X_Move;

            Move_Dir = new Vector2(X_Move * X_Dir, Y_Move * Y_Dir);

            //March
            Curr_Pos += Move_Dir;

            //Set position as air
            SetIndex((int)Curr_Pos.x, (int)Curr_Pos.y, -1);
        }

    }

    /// <summary>
    /// Build the maze by creating pieces
    /// </summary>
    void Build()
    {
        //Vertices to build with
        List<Vector3> Vertices = new List<Vector3>();

        //UVs
        List<Vector2> UVs = new List<Vector2>();

        //Ceiling UVs, Floor UVs, Wall UVs
        Vector2 cUV0 = new Vector2(.5f, .75f);
        Vector2 cUV1 = new Vector2(.5f, 1f);
        Vector2 cUV2 = new Vector2(.6666667f, 1f);
        Vector2 cUV3 = new Vector2(.6666667f, .75F);

        Vector2 fUV0 = new Vector2(.333333333f, .75F);
        Vector2 fUV1 = new Vector2(.333333333f, 1F);
        Vector2 fUV2 = new Vector2(.5f, 1F);
        Vector2 fUV3 = new Vector2(.5f, .75F);

        //Loop through wall X and Y, set pieces
        for (int x = 0; x < All_Walls.GetLength(0); x++)
        {
            for (int y = 0; y < All_Walls.GetLength(1); y++)
            {
                //If wall is a wall type, then, set
                Wall Curr_Wall = Get(x, y);

                //If index is air, then
                if (Curr_Wall.Index != 0)
                {
                    //Get north, east, south, west vals. Build walls if those are not walls themselves
                    Wall WallN = Get(x, y + 1);
                    Wall WallE = Get(x + 1, y);
                    Wall WallS = Get(x, y - 1);
                    Wall WallW = Get(x - 1, y);

                    //Which walls should we build?
                    bool BuildN = (WallN.Index == 0);
                    bool BuildE = (WallE.Index == 0);
                    bool BuildS = (WallS.Index == 0);
                    bool BuildW = (WallW.Index == 0);

                    //Get start position for x and y vertices
                    float x_start = (x * Cell_Size);
                    float y_start = (y * Cell_Size);
                    float x_end = x_start + Cell_Size;
                    float y_end = y_start + Cell_Size;

                    //Build floor
                    Vertices.Add(new Vector3(x_start, 0, y_start));
                    Vertices.Add(new Vector3(x_start, 0, y_end));
                    Vertices.Add(new Vector3(x_end, 0, y_end));

                    Vertices.Add(new Vector3(x_end, 0, y_end));
                    Vertices.Add(new Vector3(x_end, 0, y_start));
                    Vertices.Add(new Vector3(x_start, 0, y_start));

                    UVs.Add(fUV0);
                    UVs.Add(fUV1);
                    UVs.Add(fUV2);

                    UVs.Add(fUV2);
                    UVs.Add(fUV3);
                    UVs.Add(fUV0);

                    //Build ceiling
                    Vertices.Add(new Vector3(x_start, Ceiling_Height, y_start));
                    Vertices.Add(new Vector3(x_end, Ceiling_Height, y_start));
                    Vertices.Add(new Vector3(x_end, Ceiling_Height, y_end));

                    Vertices.Add(new Vector3(x_end, Ceiling_Height, y_end));
                    Vertices.Add(new Vector3(x_start, Ceiling_Height, y_end));
                    Vertices.Add(new Vector3(x_start, Ceiling_Height, y_start));

                    UVs.Add(cUV0);
                    UVs.Add(cUV1);
                    UVs.Add(cUV2);

                    UVs.Add(cUV2);
                    UVs.Add(cUV3);
                    UVs.Add(cUV0);

                    //Current wall index
                    int WallIndex = Curr_Wall.Index;

                    //Build north wall if should be built
                    if (BuildN)
                    {
                        //Add vertices for north wall
                        Vertices.Add(new Vector3(x_start, 0, y_end));
                        Vertices.Add(new Vector3(x_start, Ceiling_Height, y_end));
                        Vertices.Add(new Vector3(x_end, Ceiling_Height, y_end));

                        Vertices.Add(new Vector3(x_end, Ceiling_Height, y_end));
                        Vertices.Add(new Vector3(x_end, 0, y_end));
                        Vertices.Add(new Vector3(x_start, 0, y_end));

                        UVs = AddWallUVs(UVs, WallIndex);
                        if (WallIndex != -1) WallIndex = -1;

                    }

                    //Build east wall if should be built
                    if (BuildE)
                    {
                        Vertices.Add(new Vector3(x_end, 0, y_end));
                        Vertices.Add(new Vector3(x_end, Ceiling_Height, y_end));
                        Vertices.Add(new Vector3(x_end, Ceiling_Height, y_start));

                        Vertices.Add(new Vector3(x_end, Ceiling_Height, y_start));
                        Vertices.Add(new Vector3(x_end, 0, y_start));
                        Vertices.Add(new Vector3(x_end, 0, y_end));

                        UVs = AddWallUVs(UVs, WallIndex);
                        if (WallIndex != -1) WallIndex = -1;

                    }


                    //Build walls for south if not solid
                    if (BuildS)
                    {
                        Vertices.Add(new Vector3(x_end, 0, y_start));
                        Vertices.Add(new Vector3(x_end, Ceiling_Height, y_start));
                        Vertices.Add(new Vector3(x_start, Ceiling_Height, y_start));

                        Vertices.Add(new Vector3(x_start, Ceiling_Height, y_start));
                        Vertices.Add(new Vector3(x_start, 0, y_start));
                        Vertices.Add(new Vector3(x_end, 0, y_start));

                        UVs = AddWallUVs(UVs, WallIndex);
                        if (WallIndex != -1) WallIndex = -1;

                    }

                    //Build walls for west if not solid
                    if (BuildW)
                    {
                        Vertices.Add(new Vector3(x_start, 0, y_start));
                        Vertices.Add(new Vector3(x_start, Ceiling_Height, y_start));
                        Vertices.Add(new Vector3(x_start, Ceiling_Height, y_end));

                        Vertices.Add(new Vector3(x_start, Ceiling_Height, y_end));
                        Vertices.Add(new Vector3(x_start, 0, y_end));
                        Vertices.Add(new Vector3(x_start, 0, y_start));

                        UVs = AddWallUVs(UVs, WallIndex);
                        if (WallIndex != -1) WallIndex = -1;

                    }

                }
            }
        }

        //Set vertices, UVs, and tris
        Level_Mesh.SetVertices(Vertices);
        Level_Mesh.SetUVs(0, UVs);
        int[] Tris = GetTriangles(Vertices);
        Level_Mesh.SetTriangles(Tris, 0);

        //Set collision
        Mesh_Collider.sharedMesh = Level_Mesh;
    }

    /// <summary>
    /// Type to add. See Wall.cs for indices
    /// </summary>
    /// <param name="Type">Special type to add.</param>
    /// <returns></returns>
    List<Vector2> AddWallUVs(List<Vector2> UVs, int Type)
    {
        //UV types
        Vector2 wUV0 = new Vector2(.5f, .5f);
        Vector2 wUV1 = new Vector2(.5f, .75f);
        Vector2 wUV2 = new Vector2(.666667f, .75f);
        Vector2 wUV3 = new Vector2(.666667f, .5f);

        Vector2 glUV0 = new Vector2(.333333333f, .5f);
        Vector2 glUV1 = new Vector2(.333333333f, .75f);
        Vector2 glUV2 = new Vector2(.5f, .75f);
        Vector2 glUV3 = new Vector2(.5f, .5f);

        Vector2 uUV0 = new Vector2(.833333333f, .75f);
        Vector2 uUV1 = new Vector2(.833333333f, 1f);
        Vector2 uUV2 = new Vector2(1f, 1f);
        Vector2 uUV3 = new Vector2(1f, .75f);

        Vector2 lgbtUV0 = new Vector2(.833333333f, .5f);
        Vector2 lgbtUV1 = new Vector2(.833333333f, .75f);
        Vector2 lgbtUV2 = new Vector2(1f, .75f);
        Vector2 lgbtUV3 = new Vector2(1f, .5f);

        switch (Type)
        {
            case 1:
                UVs.Add(glUV0);
                UVs.Add(glUV1);
                UVs.Add(glUV2);

                UVs.Add(glUV2);
                UVs.Add(glUV3);
                UVs.Add(glUV0);
                break;
            case 2:
                UVs.Add(uUV0);
                UVs.Add(uUV1);
                UVs.Add(uUV2);

                UVs.Add(uUV2);
                UVs.Add(uUV3);
                UVs.Add(uUV0);
                break;
            case 3:
                UVs.Add(lgbtUV0);
                UVs.Add(lgbtUV1);
                UVs.Add(lgbtUV2);

                UVs.Add(lgbtUV2);
                UVs.Add(lgbtUV3);
                UVs.Add(lgbtUV0);
                break;
            default:
                UVs.Add(wUV0);
                UVs.Add(wUV1);
                UVs.Add(wUV2);

                UVs.Add(wUV2);
                UVs.Add(wUV3);
                UVs.Add(wUV0);
                break;

        }

        return UVs;
    }

    /// <summary>
    /// Get triangles for mesh
    /// </summary>
    /// <param name="Verts"></param>
    /// <returns></returns>
    int[] GetTriangles(List<Vector3> Verts)
    {
        int[] RetVal = new int[Verts.Count];

        for (int i = 0; i < Verts.Count; i++)
        {
            RetVal[i] = i;
        }

        return RetVal;
    }


    /// <summary>
    /// Get wall at x,y position
    /// </summary>
    /// <param name="x">Wall x position</param>
    /// <param name="y">Wall y position</param>
    /// <returns></returns>
    public Wall Get(int x, int y)
    {
        if (x >= All_Walls.GetLength(0) || x < 0) return new Wall();
        if (y >= All_Walls.GetLength(1) || y < 0) return new Wall();
        return All_Walls[x, y];
    }

    /// <summary>
    /// Sets wall index. Can only be used before walls are built.
    /// </summary>
    /// <param name="x">Wall x position</param>
    /// <param name="y">Wall y position</param>
    /// <param name="New_Index">New index for the wall</param>
    /// <returns></returns>
    public Wall SetIndex(int x, int y, int New_Index)
    {
        //If index out of range, then, do nothing
        if (x < 0 || y < 0 || x >= All_Walls.GetLength(0) || y >= All_Walls.GetLength(1)) return new Wall();

        Wall W = All_Walls[x, y];
        W.Index = New_Index;
        All_Walls[x, y] = W;
        return W;
    }

    /// <summary>
    /// Get random position in 3D coordinates to place an entity on the map. Entities can be placed anywhere but within walls.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetRandomEntityPlacementPosition()
    {
        //Get random valid air position on map
        Vector2 Random_Air_Pos = GetRandomAirPosition();

        //Convert to 3D coords
        Vector3 World_Coords = TilePostoWorldPos(Random_Air_Pos);

        //Add half of a cell size to put entity in middle of cell
        World_Coords += new Vector3(Cell_Size / 2F, 0f, Cell_Size / 2F);

        //Return world coordinates that entity can safely spawn
        return World_Coords;
    }



    /// <summary>
    /// Grabs a random existing air position on the map
    /// </summary>
    /// <returns></returns>
    Vector2 GetRandomAirPosition()
    {
        bool PositionFound = false;
        Vector2 PlacementPos = new Vector2();
        while(!PositionFound)
        {
            //Get random position on the map
            Vector2 Pos = new Vector2(UnityEngine.Random.Range(0, (int)Maze_Size.x - 1), UnityEngine.Random.Range(0, (int)Maze_Size.y - 1));

            //Is this position not a wall? If so, this is the position to return
            Wall W = Get((int)Pos.x, (int)Pos.y);
            if(W.Index != 0)
            {
                PlacementPos = Pos;
                PositionFound = true;
            }
        }

        return PlacementPos;

    }

    /// <summary>
    /// Return size of each cell on the map.
    /// </summary>
    /// <returns>The current cell size (both length x and width y) for each wall piece.</returns>
    public int GetCellSize()
    {
        return Cell_Size;
    }


    /// <summary>
    /// Convert world class to tile position
    /// </summary>
    /// <param name="WorldPos">World position to convert to tile coordinates</param>
    /// <returns>Tile coordinates from world position</returns>
    public Vector2 WorldPosToTilePos(Vector3 WorldPos)
    {
        return new Vector2((int)(WorldPos.x / Cell_Size), (int)(WorldPos.z / Cell_Size));
    }

    /// <summary>
    /// Converts given tile coordinates to world coordinates
    /// </summary>
    /// <param name="TilePos">Tile position to convert to world position.</param>
    /// <returns>World position of this tile.</returns>
    public Vector3 TilePostoWorldPos(Vector2 TilePos)
    {
        return new Vector3(TilePos.x * Cell_Size, 0, TilePos.y * Cell_Size);
    }

    /// <summary>
    /// Place the start icon
    /// </summary>
    void PlaceStartButton()
    {
        GameObject Start = GameObject.FindGameObjectWithTag("Start");
        Start.transform.position = Player.Entity_Cam_Transform.position;
    }

    /// <summary>
    /// Place flippers on the map
    /// </summary>
    void PlaceFlippers()
    {
        foreach(Flipper F in Flipper.All_Flippers)
        {
            F.Entity_Class.SpawnInCorner();
        }
    }


}
