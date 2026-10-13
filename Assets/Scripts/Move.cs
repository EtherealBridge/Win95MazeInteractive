using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [Header("Options")]

    ///<summary>
    /// This entity's speed per second
    /// </summary>
    public float Speed;

    /// <summary>
    /// Speed this entity loses per second when not trying to move
    /// </summary>
    public float SpeedLossPerSec;

    /// <summary>
    /// Speed per frame (instead of per second)
    /// </summary>
    float SpeedPerFrame
    {
        get
        {
            return Speed * Time.fixedDeltaTime;
        }
    }

    Entity _Entity_Class;

    /// <summary>
    /// This entity's parent class
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
    /// Shortcut for maze data
    /// </summary>
    CreateMaze Create_Maze
    {
        get
        {
            return Entity_Class.Create_Maze;
        }
    }


    [Header("Variables")]

    /// <summary>
    /// This entity's velocity this frame
    /// </summary>
    public Vector2 Velocity_This_Cycle = Vector2.zero;

    public bool Input_MoveFoward;
    public bool Input_MoveBackwards;
    public bool Input_MoveLeft;
    public bool Input_MoveRight;

    /// <summary>
    /// Amount player is looking sideways
    /// </summary>
    public float LookSidewaysAmt
    {
        get
        {
            return Input.GetAxis("Mouse X");
        }
    }

    /// <summary>
    /// Amount player is looking up / down
    /// </summary>
    public float LookUpDownAmt
    {
        get
        {
            return Input.GetAxis("Mouse Y");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //If this entity is the player, then, move by input
        if (Entity_Class.isPlayer && !Entity_Class.isDead)
        {
            MoveByInput();

            //Player can look around with mouse
            MouseLook();
        }

        //If player dead, then, no input
        if(Entity_Class.isDead)
        {
            //No input allowed
            Input_MoveFoward = false;
            Input_MoveBackwards = false;
            Input_MoveLeft = false;
            Input_MoveRight = false;

            //Stop immediately
            Velocity_This_Cycle = Vector2.zero;
        }
    }

    /// <summary>
    /// Updates once every physics update
    /// </summary>
    private void FixedUpdate()
    {
        //If not moving, bleed speed
        if (!Input_MoveFoward && Velocity_This_Cycle.x > 0F) BleedFwdSpeed();
        if (!Input_MoveBackwards && Velocity_This_Cycle.x < 0F) BleedBackwardSpeed();
        if (!Input_MoveLeft && Velocity_This_Cycle.y > 0F) BleedLeftSpeed();
        if (!Input_MoveRight && Velocity_This_Cycle.y < 0F) BleedRightSpeed();

        //Set velocity based on movement input or AI desired movement
        SetVelocity();

        //Clamp speed this cycle and snap to 0 if close enough
        ClampSpeed();

        //If entity is moving, then
        if (Velocity_This_Cycle != Vector2.zero)
        {
            //Update velocity for entity this frame
            UpdatePositionThisCycle();
        }
        else
        {
            //Do not allow extra movement
            Entity_Class.Rigid_Body.velocity = Vector3.zero;
        }
}

    /// <summary>
    /// Move by user input
    /// </summary>
    void MoveByInput()
    {
        //Move forward
        Input_MoveFoward = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow));

        //Move backward
        Input_MoveBackwards = (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow));

        //Left
        Input_MoveLeft = (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow));

        //Right
        Input_MoveRight = (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow));


    }

    
    /// <summary>
    /// Set entity velocity this cycle.
    /// </summary>
    void SetVelocity()
    {

        //Forward / Back
        if(Input_MoveFoward)
        {
            Velocity_This_Cycle.x += (Speed * Time.fixedDeltaTime);
        }
        else if (Input_MoveBackwards)
        {
            Velocity_This_Cycle.x -= (Speed * Time.fixedDeltaTime);
        }

        //Side to Side
        if (Input_MoveLeft)
        {
            Velocity_This_Cycle.y += (Speed * Time.fixedDeltaTime);
        }
        else if (Input_MoveRight)
        {
            Velocity_This_Cycle.y -= (Speed * Time.fixedDeltaTime);
        }
    }

    /// <summary>  
    /// Update this entity's position this frame
    /// </summary>
    void UpdatePositionThisCycle()
    {

        //If is player and is swimming, then forward is camera forward, not entity forward
        Vector3 fwd = Velocity_This_Cycle.x * Entity_Class.Transform_Class.forward.normalized;

        Vector3 side = (Velocity_This_Cycle.y * -Entity_Class.Transform_Class.right.normalized);
        Vector3 Move = (fwd + side);

        Move.y = 0F;

        //Set Velocities
        Entity_Class.Rigid_Body.velocity = Move;


    }

    /// <summary>
    /// Lose speed if no movement for moving forward
    /// </summary>
    void BleedFwdSpeed()
    {
        bool isPos = Velocity_This_Cycle.x > 0.0F;
        Velocity_This_Cycle.x -= (SpeedLossPerSec * Time.fixedDeltaTime);
        bool isPosNow = Velocity_This_Cycle.x > 0.0F;
        if ((isPos && !isPosNow) || (isPosNow && !isPos))
        {
            Velocity_This_Cycle.x = 0.0F;
        }
    }

    /// <summary>
    /// Lose speed if no movement for moving back
    /// </summary>
    void BleedBackwardSpeed()
    {
        bool isPos = Velocity_This_Cycle.x > 0.0F;
        Velocity_This_Cycle.x += (SpeedLossPerSec * Time.fixedDeltaTime);
        bool isPosNow = Velocity_This_Cycle.x > 0.0F;
        if ((isPos && !isPosNow) || (isPosNow && !isPos))
        {
            Velocity_This_Cycle.x = 0.0F;
        }
    }


    /// <summary>
    /// Lose speed if no movement for moving left
    /// </summary>
    void BleedLeftSpeed()
    {
        bool isPos = Velocity_This_Cycle.y > 0.0F;
        Velocity_This_Cycle.y -= (SpeedLossPerSec * Time.fixedDeltaTime);
        bool isPosNow = Velocity_This_Cycle.y > 0.0F;
        if ((isPos && !isPosNow) || (isPosNow && !isPos))
        {
            Velocity_This_Cycle.y = 0.0F;
        }
    }


    /// <summary>
    /// Lose speed if no movement for moving right
    /// </summary>
    void BleedRightSpeed()
    {
        bool isPos = Velocity_This_Cycle.y > 0.0F;
        Velocity_This_Cycle.y += (SpeedLossPerSec * Time.fixedDeltaTime);
        bool isPosNow = Velocity_This_Cycle.y > 0.0F;
        if ((isPos && !isPosNow) || (isPosNow && !isPos))
        {
            Velocity_This_Cycle.y = 0.0F;
        }
    }

    /// <summary>
    /// Do not allow to go over speed limit. If close to not moving, clamp to 0
    /// </summary>
    void ClampSpeed()
    {
        //Do not allow to go over speed limit in any direction
        Velocity_This_Cycle.x = Mathf.Clamp(Velocity_This_Cycle.x, -Speed, Speed);
        Velocity_This_Cycle.y = Mathf.Clamp(Velocity_This_Cycle.y, -Speed, Speed);

        //if close to 0, then snap
        if (Velocity_This_Cycle.x < SpeedPerFrame && Velocity_This_Cycle.x > -SpeedPerFrame) Velocity_This_Cycle.x = 0F;
        if (Velocity_This_Cycle.y < SpeedPerFrame && Velocity_This_Cycle.y > -SpeedPerFrame) Velocity_This_Cycle.y = 0F;
    }


    /// <summary>
    /// Mouse look - moves body left-right and camera up-down
    /// </summary>
    void MouseLook()
    {
        //Get current look rotation
        Vector3 Look_Rot = GetLookRotation();

        //Rotate with mouse movement
        RotPlayer(Look_Rot);

    }

    /// <summary>
    /// Gets the look rotation for the camera
    /// </summary>
    /// <returns></returns>
    Vector3 GetLookRotation()
    {
        float LeftRight = LookSidewaysAmt * 4F;
        float UpDown = -LookUpDownAmt * 4F;
        Vector3 Rot = new Vector3(UpDown, LeftRight, 0F);
        return Rot;
    }


            /// <summary>
        /// Rotates the player
        /// </summary>
        /// <param name="v">Amount to rotate</param>
        void RotPlayer(Vector2 v)
        {
                //Rotate Body
                RotateBody(v.y);

                //Rotate camera
                RotateCamera(Entity_Class.Entity_Cam_Transform, v.x);

        }

    /// <summary>
    /// Rotates head and body
    /// </summary>
    /// <param name="CamHolder"></param>
    /// <param name="Vertical"></param>
    /// <param name="Horizontal"></param>
    public void Rotate(Transform CamHolder, float Vertical, float Horizontal)
    {
        //Rotate head
        RotateCamera(CamHolder, Vertical);

        //Rotate body
        RotateBody(Horizontal);
    }


    /// <summary>
    /// Rotates this entity's camera (head). Positive = up, negative = down
    /// </summary>
    /// <param name="Cam">Camera to rotate</param>
    /// <param name="UpDown">Amount to rotate this frame up and down</param>
    /// <param name="SideToSide">Amount to rotate side-to-side - not required</param>
    public void RotateCamera(Transform CamHolder, float UpDown, float SideToSide = 0F)
    {
        //Get current camera rotation
        Vector3 CameraRot = CamHolder.localRotation.eulerAngles;

        //Rotate Camera
        float CurrUpDown = CameraRot.x;

        //Get new camera up-down position
        float NewUpDown = CurrUpDown + UpDown;

        //Get side-to-side (if applicable)
        float NewSideToSide = CameraRot.y;
        if (SideToSide != 0F)
        {
            NewSideToSide += SideToSide;
        }

        //Apply rotation
        Vector3 CamRot = new Vector3(NewUpDown, NewSideToSide, 0F);

        CamRot.y = 0F;

        //Set actual location
        CamHolder.transform.localRotation = Quaternion.Euler(CamRot);

    }


    /// <summary>
    /// Amount to rotate. Negative = left, Positive = right
    /// </summary>
    /// <param name="Amt"></param>
    public void RotateBody(float Amt)
    {
        //Rotate player
        Entity_Class.Transform_Class.Rotate(0F, Amt, 0F);
    }

    /// <summary>
    /// Force this entity to come to a dead stop
    /// </summary>
    public void DeadStop()
    {
        Entity_Class.Rigid_Body.velocity = Vector3.zero;
        Velocity_This_Cycle = Vector2.zero;
    }

}
