using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    /// <summary>
    /// Player's time score
    /// </summary>
    public float Time_Score
    {
        get
        {
            return Mathf.Clamp(10000F - (10 *  Player.Time_Alive), 250F, 1000F);
        }
    }

    /// <summary>
    /// Final objective score
    /// </summary>
    public float Objective_Score
    {
        get
        {
            float FinalScore = 0F;
            for(int i = 0; i < Objectives.Length; i++)
            {
                Objective Obj = Objectives[i];
                if(Obj.isFinished)
                {
                    FinalScore += Obj.Total_Score;

                }
            }

            return FinalScore;
        }
    }

    /// <summary>
    /// Penalty for dying if player died
    /// </summary>
    public float Death_Penalty
    {
        get
        {
            if (!Player.isDead) return 0F;
            else
            {
                return Mathf.Clamp((10000F - (Player.Time_Alive * 10F)), 250F, 10000F);
            }
        }
    }

    /// <summary>
    /// Total player score
    /// </summary>
    public float TotalScore
    {
        get
        {
            return (Objective_Score + Time_Score) - Death_Penalty;
        }
    }

    Objective[] _Obj;

    /// <summary>
    /// Get objectives
    /// </summary>
    Objective[] Objectives
    {
        get
        {
            if(_Obj == null) _Obj = FindObjectsOfType<Objective>();
            return _Obj;
        }
    }


    /// <summary>
    /// The score text control
    /// </summary>
    public Text ScoreText;


    Entity _Player;

    /// <summary>
    /// The player
    /// </summary>
    Entity Player
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
    /// Ending goal component
    /// </summary>
    public EndGoal End_Goal;


    public string Score_Text
    {
        get
        {
            return string.Format("Score: " + TotalScore.ToString(""));
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        UpdateScoreControl();  
    }

    /// <summary>
    /// Tally the player's score
    /// </summary>
    void UpdateScoreControl()
    {
        ScoreText.text = Score_Text;
    }
}
