using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        GENERATE_GRID,
        PLAYER_SPAWN,
        ENEMIES_SPAWN,
        PLAYER_TURN,
        ENEMIES_TURN
    }
    public static GameManager instance { get; private set; }

    [SerializeField] private GameState baseState;
    private GameState gameState;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }
    void Start()
    {
        ChangeState(baseState);
    }

    public void ChangeState(GameState newState)
    {
        gameState = newState;
        switch(gameState)
        {
            case GameState.GENERATE_GRID:
                GridManager.instance.GenerateGrid();
                break;
            case GameState.PLAYER_SPAWN:
                UnitManager.instance.SpawnAllies();
                break;
            default:
                break;
        }
    }
}
