using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    static public UIManager Instance;

    [SerializeField] TileInformationUI tileInformationUI;
    [SerializeField] GameObject playerChoicesUI;
    // Start is called before the first frame update
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        GameManager.OnGameStateChanged += DisplayPlayerChoices;

    }

    private void DisplayPlayerChoices(GameManager.GameState state)
    {
        if (state != GameManager.GameState.PLAYER_CHOICE)
        {
            playerChoicesUI.SetActive(false);
        }
        else
        {
            playerChoicesUI.SetActive(true);
        }
    }

    public void UpdateTileInfo(Tile tile)
    {
        if (!tile)
        {
            tileInformationUI.enabled = false;
            return;
        }
        tileInformationUI.enabled = true;
        tileInformationUI.UpdateUi(tile);
    }
}
