using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveProgress : MonoBehaviour
{

    /// <summary>
    /// Holder for objective progress
    /// </summary>
    public Transform Main_Window;

    /// <summary>
    /// For the objective status bar
    /// </summary>
    public RawImage Objective_Status_Bar;

    RectTransform _Objective_Status_Bar_Trans;

    /// <summary>
    /// Objective status bar transform
    /// </summary>
    RectTransform Objective_Status_Bar_Trans
    {
        get
        {
            if (_Objective_Status_Bar_Trans == null) _Objective_Status_Bar_Trans = Objective_Status_Bar.gameObject.GetComponent<RectTransform>();
            return _Objective_Status_Bar_Trans;
        }
    }

    /// <summary>
    /// Text to show as object is progressing
    /// </summary>
    public Text Objective_Progressing_Text;


    float HideTime = 3F;
    public float TimeLeftForGUI = 0.0F;


    private void Update()
    {
        //Hide GUI after 3 seconds
        if (TimeLeftForGUI <= 0.0F)
        {
            TimeLeftForGUI = 0F;
            Main_Window.gameObject.SetActive(false);
        }
        else
        {
            TimeLeftForGUI -= Time.deltaTime;

            //Show content
            Main_Window.gameObject.SetActive(true);
        } 

    }

    /// <summary>
    /// Show progress for the given objective
    /// </summary>
    /// <param name="Obj">Objective to show progress for.</param>
    public void ShowProgress(Objective Obj)
    {
        TimeLeftForGUI = HideTime;

        //Update status bar to reflect % finished
        Objective_Status_Bar_Trans.anchorMax = new Vector2(Obj.ProgressPct, 0.5F);
        SetRight(Objective_Status_Bar_Trans, 0.0F);

        //If objective is not finished, then, show progressing message. Otherwise, show finished message.
        if(!Obj.isFinished)
        {
            SetText(Obj.Progressing_Text);
        }
        else
        {
            SetText(Obj.Finished_Text);
        }
    }

    //public static void SetLeft(RectTransform rt, float left)
    //{
    //    rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    //}

    void SetRight(RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    /// <summary>
    /// Set the progression text
    /// </summary>
    /// <param name="Txt">Progression text to set</param>
    void SetText(string Txt)
    {
        Objective_Progressing_Text.text = Txt;
    }

    //public static void SetTop(RectTransform rt, float top)
    //{
    //    rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    //}

    //public static void SetBottom(RectTransform rt, float bottom)
    //{
    //    rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    //}

}
