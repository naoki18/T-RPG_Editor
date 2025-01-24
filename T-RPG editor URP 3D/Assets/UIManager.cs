using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    static public UIManager Instance;

    [SerializeField] TileInformationUI tileInformationUI;
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
