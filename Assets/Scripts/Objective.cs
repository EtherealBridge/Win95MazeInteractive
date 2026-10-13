using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{

    [Header("Options")]
    /// <summary>
    /// Time for this objective to be completed
    /// </summary>
    public float TimeToComplete;


    float OriginalTime;

    /// <summary>
    /// Text to show when objective is being completed
    /// </summary>
    public string Progressing_Text;

    /// <summary>
    /// Goal for this objective
    /// </summary>
    public string Objective_Text;

    /// <summary>
    /// Text to show for finished objective
    /// </summary>
    public string Finished_Text;

    /// <summary>
    /// Indicator for objective
    /// </summary>
    public GameObject Indicator;

    /// <summary>
    /// This objective mesh renderer
    /// </summary>
    public MeshRenderer Obj_Mesh_Renderer;

    /// <summary>
    /// When objective is complete, hide these objects
    /// </summary>
    public MeshRenderer[] Hide_Objects_On_Complete;

    /// <summary>
    /// Effect to play when completed
    /// </summary>
    public ParticleSystem Effect_When_Completed;

    /// <summary>
    /// Amount to shake objective when completed
    /// </summary>
    public float ShakeWhenCompleted;

    /// <summary>
    /// If set, will change material to this when objective complete
    /// </summary>
    public Material Completed_Material;

    /// <summary>
    /// Is this the final objective?
    /// </summary>
    public bool FinalObjective;

    /// <summary>
    /// Score for finishing this task
    /// </summary>
    public float Score;

    /// <summary>
    /// Time for objective to be completed
    /// </summary>
    public float Completion_Time;

    /// <summary>
    /// Total score for this objective
    /// </summary>
    public float Total_Score
    {
        get
        {
            float AdjTime = 300F - Completion_Time;
            AdjTime = Mathf.Clamp(AdjTime, 100.0F, 300F);
            return AdjTime;
        }
    }


    /// <summary>
    /// Percent done this objective is.
    /// </summary>
    public float ProgressPct
    {
        get
        {
            return 1F - (TimeToComplete / OriginalTime);
        }
    }

    Entity _Entity_Class;

    public Entity Entity_Class
    {
        get
        {
            if (_Entity_Class == null) _Entity_Class = GetComponent<Entity>();
            return _Entity_Class;
        }
    }

    ObjectiveProgress _Objective_Progress;

    ObjectiveProgress Objective_Progress
    {
        get
        {
            if (_Objective_Progress == null) _Objective_Progress = FindObjectOfType<ObjectiveProgress>();
            return _Objective_Progress;
        }
    }

    /// <summary>
    /// Is this objective finished?
    /// </summary>
    public bool isFinished
    {
        get
        {
            return TimeToComplete == 0F;
        }
    }




    Transform _Trans;

    /// <summary>
    /// Transform component for objective
    /// </summary>
    Transform Transform_Component
    {
        get
        {
            if (_Trans == null) _Trans = transform;
            return _Trans;
        }
    }

    private void Start()
    {
        OriginalTime = TimeToComplete;
    }

    /// <summary>
    /// Do this objective - decrements time left counter
    /// </summary>
    public void Do()
    {

        TimeToComplete -= Time.deltaTime;

        //Clamp
        TimeToComplete = Mathf.Clamp(TimeToComplete, 0.0F, OriginalTime);

        //Change objective progress GUI to show this objective
        Objective_Progress.ShowProgress(this);

        //If objective is finished, then, remove indicator
        if (isFinished)
        {

            //For each item to hide, hide
            for (int i = 0; i < Hide_Objects_On_Complete.Length; i++)
            {
                MeshRenderer M = Hide_Objects_On_Complete[i];
                M.enabled = false;
            }

            //Set replacement material if set
            if (Completed_Material != null)
            {
                Obj_Mesh_Renderer.material = Completed_Material;
            }

            //Enable particle effect if complete
            if (Effect_When_Completed != null)
            {
                Effect_When_Completed.gameObject.SetActive(true);
            }
        }

    }

    /// <summary>
    /// Updates once per frame
    /// </summary>
    private void Update()
    {
        //If should shake, then
        if(isFinished && ShakeWhenCompleted > 0F)
        {
            Shake();
        }
        

        //Hide indicator if inactive
        if(isFinished && Indicator.activeSelf)
        {
            //Hide indicator
            Indicator.SetActive(false);
        }
    }

    /// <summary>
    /// Causes this entity to shake
    /// </summary>
    void Shake()
    {
        //Get diff. between x and original x
        float RandX = Random.Range(-ShakeWhenCompleted, ShakeWhenCompleted);

        //Get diff. between z and original z
        float RandZ = Random.Range(-ShakeWhenCompleted, ShakeWhenCompleted);

        //Get positions
        float XPos = Entity_Class.Original_Position.x + RandX;
        float ZPos = Entity_Class.Original_Position.z + RandZ;

        //Set pos
        Transform_Component.position = new Vector3(XPos, Entity_Class.Original_Position.y, ZPos);

    }

}
