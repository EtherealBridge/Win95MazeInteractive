using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Builds the navigation mesh for entities on the map
/// </summary>
public class BuildNavMesh : MonoBehaviour
{


    Unity.AI.Navigation.NavMeshSurface _Nav_Mesh_Surface;

    /// <summary>
    /// The nav mesh surface for the maze
    /// </summary>
    Unity.AI.Navigation.NavMeshSurface Nav_Mesh_Surface
    {
        get
        {
            if (_Nav_Mesh_Surface == null) _Nav_Mesh_Surface = GetComponent< Unity.AI.Navigation.NavMeshSurface> ();
            return _Nav_Mesh_Surface;
        }
    }

   /// <summary>
   /// Build the nav mesh for the maze
   /// </summary>
    public void Build()
    {
        Nav_Mesh_Surface.BuildNavMesh();
    }
}
