using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        START_GAME,
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
    private Grid gameGrid;

    public delegate void OnGameStart();
    public event OnGameStart onGameStart;

    public delegate void OnChangeState(Grid grid);
    public event OnChangeState onPlayerSpawn;
    public event OnChangeState onEnemiesSpawn;
    public event OnChangeState onPlayerTurn;
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
        switch (gameState)
        {
            case GameState.START_GAME:
                onGameStart?.Invoke();
                break;
            case GameState.PLAYER_SPAWN:
                onPlayerSpawn?.Invoke(gameGrid);
                break;
            case GameState.ENEMIES_SPAWN:
                onEnemiesSpawn?.Invoke(gameGrid);
                break;
            case GameState.PLAYER_TURN:
                onPlayerTurn?.Invoke(gameGrid);
                break;
            default:
                break;
        }
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
