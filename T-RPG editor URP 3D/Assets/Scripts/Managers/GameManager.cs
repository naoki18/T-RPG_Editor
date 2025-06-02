using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        START_GAME,
        PLAYER_SPAWN,
        ENEMIES_SPAWN,
        PLAYER_TURN,
        PLAYER_CHOICE,
        PLAYER_MOVE_CHARACTER,
        PLAYER_ATTACK,
        ENEMIES_TURN,
        NULL
    }
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameState baseState;
    private GameState gameState;
    private Grid gameGrid;
    private Stack<GameState> previousStates = new Stack<GameState>();

    public static event Action<GameState> OnGameStateChanged;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Instance.gameState = GameState.NULL;
        }
        else Destroy(this);
    }

    void Start()
    {
        ChangeState(baseState);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            BackToPreviousState();
        }
    }

    public void ChangeState(GameState newState, bool savePreviousState = false)
    {
        if(savePreviousState) previousStates.Push(gameState);
        // TODO : Ajouter des delegates pour laisser chaque manager faire ce qu'il doit faire lors d'un nouveau state
        gameState = newState;
        OnGameStateChanged?.Invoke(newState);
    }

    public void ChangeState(string newState)
    {
        string[] strings = newState.Split('&');
        bool savePreviousState = false;
        if(strings.Length > 1 && string.Equals(strings[1], "True", StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log("Save previous");
            savePreviousState = true;
        }
        Debug.Log($"new State => {strings[0]}");
        ChangeState((GameState)Enum.Parse(typeof(GameState), strings[0]), savePreviousState);
    }

    public void BackToPreviousState()
    {
        if (previousStates.Count == 0) return;
        ChangeState(previousStates.Pop());
    }
    public void ClearPreviousStates()
    {
        previousStates.Clear();
    }
    public GameState GetState()
    {
        return gameState;
    }
    public void SetGrid(Grid grid)
    { 
        gameGrid = grid; 
    }
}
