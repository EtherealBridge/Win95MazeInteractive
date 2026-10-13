using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGoal : MonoBehaviour
{

    ///<summary>
    /// Control for listing in-game tasks
    /// </summary>
    public ListBox Task_List;

    /// <summary>
    /// Distance player needs to be to this item to win
    /// </summary>
    public float Win_Distance;


    Entity _Entity_Class;

    Entity Entity_Class
    {
        get
        {
            if (_Entity_Class == null) _Entity_Class = GetComponent<Entity>();
            return _Entity_Class;
        }
    }

    Objective _This_Objective;

    /// <summary>
    /// This objective
    /// </summary>
    public Objective This_Objective
    {
        get
        {
            if (_This_Objective == null) _This_Objective = GetComponent<Objective>();
            return _This_Objective;
        }
    }    

    /// <summary>
    /// Get Deacon character
    /// </summary>
    Deacon Deacon_Char
    {
        get
        {
            return FindObjectOfType<Deacon>();
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
    /// Component for resarting game
    /// </summary>
    RestartGame Restart_Game
    {
        get
        {
            return FindObjectOfType<RestartGame>();
        }
    }


    bool hasDestroyedDeacon;


    // Start is called before the first frame update
    void Start()
    {
        ReplaceGoals();

    }


    private void Update()
    {
        CheckIfWin();
    }

    /// <summary>
    /// Replace goals with goal to exit maze
    /// </summary>
    void ReplaceGoals()
    {
        //Remove cureent goals
        Task_List.Clear();

        //Blank out completion indicators
        for(int i = 0; i < Task_List.options.Length; i++)
        {
            GameObject Obj = Task_List.options[i];
            Image I = Obj.GetComponent<Image>();
            I.enabled = false;

        }

        //Add option for objective
        Task_List.AddOption(This_Objective.Progressing_Text, This_Objective);


    }


    /// <summary>
    /// Check if player has won
    /// </summary>
    void CheckIfWin()
    {
        //Get distance to player
        float DistToPlayer = Vector3.Distance(Entity_Class.World_Position, Player.World_Position);

        //If player is within win distance, then, win
        if(DistToPlayer < Win_Distance || This_Objective.isFinished)
        {
            This_Objective.TimeToComplete = 0;
            Restart_Game.ShowWindow();
            Restart_Game.SetInfoText("Congratulations! You Won! Press R to Play Again!");
            if (!hasDestroyedDeacon)
            {
                hasDestroyedDeacon = true;
                if(Deacon_Char != null) Deacon_Char.gameObject.SetActive(false);
            }
            
        }
    }

}
