using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One object enters world, force it to stay in that position
/// </summary>
public class Stabilize : MonoBehaviour
{
    Vector3 OriginalPos;
    Quaternion Original_Rot;

    // Start is called before the first frame update
    void Start()
    {
        OriginalPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = OriginalPos;
    }
}
