using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// if this entity is not moving, then, switch its texture
/// </summary>
public class SwitchTextureIfStopped : MonoBehaviour
{
    MeshRenderer _Mesh_Renderer;

    /// <summary>
    /// This plane's mesh renderer
    /// </summary>
    MeshRenderer Mesh_Renderer
    {
        get
        {
            if (_Mesh_Renderer == null) _Mesh_Renderer = GetComponent<MeshRenderer>();
            return _Mesh_Renderer;
        }
    }

    Entity _Entity;

    /// <summary>
    /// This entity's superclass
    /// </summary>
    Entity Entity_Class
    {
        get
        {
            if (_Entity == null) _Entity = GetComponentInParent<Entity>();
            return _Entity;
        }
    }

    [Header("Options")]

    ///<summary>
    /// Material to set when this entity is moving
    /// </summary>
    public Material Moving_Material;

    /// <summary>
    /// Material to set when this entity is stopped
    /// </summary>
    public Material Stopped_Material;

    /// <summary>
    /// Entity's previous position
    /// </summary>
    Vector3 LastPos = Vector3.zero;

    /// <summary>
    /// Shortcut for current position
    /// </summary>
    Vector3 CurrentPos
    {
        get
        {
            return Entity_Class.World_Position;
        }
    }

    /// <summary>
    /// Is this entity moving?
    /// </summary>
    bool isMoving
    {
        get
        {
            return CurrentPos != LastPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if(isMoving)
        {
            Mesh_Renderer.material = Moving_Material;
        }
        else
        {
            Mesh_Renderer.material = Stopped_Material;
        }

        LastPos = CurrentPos;
    }
}
