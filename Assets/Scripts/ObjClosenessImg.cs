using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjClosenessImg : MonoBehaviour
{
    /// <summary>
    /// Parent listbox
    /// </summary>
    public ListBox Parent_Listbox;

    /// <summary>
    /// This item
    /// </summary>
    public GameObject ItemComponent;

    /// <summary>
    /// Closeness image for list control
    /// </summary>
    public Image Closeness_Img;

    /// <summary>
    /// Text content in dropdown
    /// </summary>
    public Text Text_Content;


    /// <summary>
    /// If set, wull measure closeness to Deacon instead
    /// </summary>
    public Deacon Deacon_Obj;


    /// <summary>
    /// Is Deacon being tracked instead of objective?
    /// </summary>
    bool isTrackingDeacon
    {
        get
        {
            return Deacon_Obj != null;
        }
    }


    /// <summary>
    /// Type of object closeness images
    /// </summary>
    public enum ImgCloseness
    {
        Closest,
        VeryClose,
        Close,
        SlightlyClose,
        Far,
        VeryFar,
        Furthest,
        Completed
    }

    /// <summary>
    /// Image closeness images by material
    /// </summary>
    public Material[] Img_Closeness_Materials;

    /// <summary>
    /// Very close distance
    /// </summary>
    float VeryCloseDistance = 20F;

    /// <summary>
    /// Distance for when you're close to an objective
    /// </summary>
    float CloseDistance = 30F;

    /// <summary>
    /// Slightly close diatance
    /// </summary>
    float SlightlyCloseDistance = 50F;

    /// <summary>
    /// Distance for when you're far
    /// </summary>
    float FarDistance = 70F;

    /// <summary>
    /// Distance at when objective is very far
    /// </summary>
    float VeryFarDistance = 100f;

    /// <summary>
    /// Very far distance
    /// </summary>
    float FurthestDistance = 140F;

    /// <summary>
    /// Is this image visible? To be visible, must have text associated with it
    /// </summary>
    public bool isVisible
    {
        get
        {
            return !string.IsNullOrEmpty(Text_Content.text);
        }
    }

    Entity _Player;

    /// <summary>
    /// The player
    /// </summary>
    public Entity Player
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



    /// <summary>
    /// The amount of distance to the player
    /// </summary>
    float DistanceToPlayer
    {
        get
        {
            if(isTrackingDeacon) return Vector3.Distance(Deacon_Obj.Entity_Class.World_Position, Player.World_Position);
            else return Vector3.Distance(This_Objective.Entity_Class.World_Position, Player.World_Position);
        }
    }

    /// <summary>
    /// Current image to show for this control
    /// </summary>
    ImgCloseness Current_Image_Type
    {
        get
        {

            //If instance is not tracking Deacon, then, see if object completed
            if (!isTrackingDeacon)
            {
                if (isCompleted) return ImgCloseness.Completed;
            }

            float Dist = DistanceToPlayer;
            if (Dist > FurthestDistance)
            {
                return ImgCloseness.Furthest;
            }
            else if (Dist > VeryFarDistance)
            {
                return ImgCloseness.VeryFar;
            }
            else if (Dist > FarDistance)
            {
                return ImgCloseness.Far;
            }
            else if (Dist > SlightlyCloseDistance)
            {
                return ImgCloseness.SlightlyClose;
            }
            else if (Dist > CloseDistance)
            {
                return ImgCloseness.Close;
            }
            else if (Dist > VeryCloseDistance)
            {
                return ImgCloseness.VeryClose;
            }
            else
            {
                return ImgCloseness.Closest;
            }
            
        }
    }

    /// <summary>
    /// This item's current material
    /// </summary>
    public Material Current_Material
    {
        get
        {
            return Img_Closeness_Materials[(int)Current_Image_Type];
        }
    }

    int _My_Index = -1;

    //Get my index in listbox
    int My_Index
    {
        get
        {
            if(_My_Index == -1)
            {
                for(int i = 0; i < Parent_Listbox.options.Length; i++)
                {
                    GameObject Curr_Item = Parent_Listbox.options[i];
                    if(Curr_Item == ItemComponent)
                    {
                        _My_Index = i;
                    }
                }
            }

            return _My_Index;
        }
    }

    /// <summary>
    /// The objective assigned to this item?
    /// </summary>
    Objective This_Objective
    {
        get
        {
            if (My_Index < Parent_Listbox.Count())
            {
                return (Objective)Parent_Listbox.GetValueAtIndex(My_Index);
            }
            else
            {
                return null;
            }
        }
    }
    
    /// <summary>
    /// Has this objective been completed?
    /// </summary>
    bool isCompleted
    {
        get
        {
            return This_Objective.isFinished;
        }
    }


    // Update is called once per frame
    void Update()
    {
        UpdateImg();
    }


    void UpdateImg()
    {
        //If should not show, then disable. Otherwise, show.
        if(isVisible)
        {
            Closeness_Img.enabled = true;

            //Update image
            Closeness_Img.material = Current_Material;
        }
        else
        {
            Closeness_Img.enabled = false;
        }
    }


   }
