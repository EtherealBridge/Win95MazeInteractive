using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Indicator for player being hurt
/// </summary>
public class HurtIndicator : MonoBehaviour
{

    [Header("Loss Per Second")]

    ///<summary>
    /// Loss per second
    /// </summary>
    public float LossPerSec;

    RawImage _Raw_Image;

    /// <summary>
    /// The hurt image
    /// </summary>
    RawImage Raw_Image
    {
        get
        {
            if (_Raw_Image == null) _Raw_Image = GetComponent<RawImage>();
            return _Raw_Image;
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
    /// Current transparency
    /// </summary>
    float Transparency;


    // Update is called once per frame
    void Update()
    {
        //Lose transparency
            Transparency -= (LossPerSec * Time.deltaTime);
            Transparency = Mathf.Clamp(Transparency, 0.0F, 1.0F);

        //Update material
        Color Raw_Img_Color = Raw_Image.color;
        Raw_Img_Color.a = Transparency;
        Raw_Image.color = Raw_Img_Color;
    }


    /// <summary>
    /// Cause effect
    /// </summary>
    public void Flash()
    {
        Transparency = 1f;
    }
}
