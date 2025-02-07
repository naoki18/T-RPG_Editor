using System;
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
        ENEMIES_TURN
    }
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameState baseState;
    private GameState gameState;
    private Grid gameGrid;

    public static event Action<GameState> OnGameStateChanged;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
    void Start()
    {
        ChangeState(baseState);
    }

    public void ChangeState(GameState newState)
    {
        // TODO : Ajouter des delegates pour laisser chaque manager faire ce qu'il doit faire lors d'un nouveau state
        gameState = newState;

        OnGameStateChanged?.Invoke(newState);
    }

    public void ChangeState(string newState)
    {
        ChangeState((GameState)Enum.Parse(typeof(GameState), newState));
    }
    public GameState GetState()
    {
        return gameState;
    }

    public void SetGrid(Grid grid)
    { 
        gameGrid = grid; 
    }

    public void Test()
    {
        Debug.Log("test");
    }
}
