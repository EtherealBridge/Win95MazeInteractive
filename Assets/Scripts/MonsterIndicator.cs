using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For indicator of how close monster is to you. Only controls if the indicator is active or inactive
/// </summary>
public class MonsterIndicator : MonoBehaviour
{


    /// <summary>
    /// Container to show or hide
    /// </summary>
    public GameObject Elements_Container;

    /// <summary>
    /// Deacon (monster) object
    /// </summary>
    public Deacon Deacon_Obj;

    /// <summary>
    /// Text component ("You Are Being Hunted" Description)
    /// </summary>
    public UnityEngine.UI.Text Text_Comp;

    /// <summary>
    /// Is Deacon current active?
    /// </summary>
    bool isTrackingDeacon
    { 
        get
        {
            if (Deacon_Obj != null) return Deacon_Obj.gameObject.activeSelf;
            else return false;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (isTrackingDeacon)
        {
            Elements_Container.SetActive(Deacon_Obj.gameObject.activeSelf);
        }
        else
        {
            Elements_Container.SetActive(false);
        }

        //If all objectives done, then, change text.
        if(Deacon_Obj.All_Objectives_Done)
        {
            Text_Comp.text = "Deacon Lost Interest...";
        }
    }
}
