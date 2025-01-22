using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    static public UIManager instance;

    [SerializeField] TileInformationUI tileInformationUI;
    // Start is called before the first frame update
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        GridManager.Instance.OnTileHovered += UpdateTileInfo;
    }

    private void OnDestroy()
    {
        GridManager.Instance.OnTileHovered -= UpdateTileInfo;
    }
    private void UpdateTileInfo(Tile tile)
    {
        if (!tile) return;
        tileInformationUI.UpdateUi(tile);
    }
}
