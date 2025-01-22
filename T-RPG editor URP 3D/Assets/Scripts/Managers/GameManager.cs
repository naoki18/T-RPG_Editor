using System;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        GENERATE_GRID,
        PLAYER_SPAWN,
        ENEMIES_SPAWN,
        PLAYER_TURN,
        PLAYER_MOVE_CHARACTER,
        PLAYER_ATTACK,
        ENEMIES_TURN
    }
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameState baseState;
    private GameState gameState;

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
        switch(gameState)
        {
            case GameState.GENERATE_GRID:
                GridManager.Instance.GenerateGrid();
                break;
            case GameState.PLAYER_SPAWN:
                UnitManager.instance.SpawnAllies();
                break;
            case GameState.ENEMIES_SPAWN:
                UnitManager.instance.SpawnEnemies();
                break;
            case GameState.PLAYER_TURN:
                GridManager.Instance.ClearReachablePos();
                break;
            default:
                break;
        }
    }

    public GameState GetState()
    {
        return gameState;
    }
}
