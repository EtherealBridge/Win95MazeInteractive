using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Delete after X seconds
/// </summary>
public class KillAfterXSecs : MonoBehaviour
{

    [Header("Options")]


    ///<summary>
    /// Amount of time until entity deleted
    /// </summary>
    public float TimeTilGone;



    // Start is called before the first frame update
    void Start()
    {
        Invoke("Kill", TimeTilGone);
    }

    void Kill()
    {
        Destroy(gameObject);
    }
    
}
