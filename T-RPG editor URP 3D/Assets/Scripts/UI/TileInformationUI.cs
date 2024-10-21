using TMPro;
using UnityEngine;

public class TileInformationUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TileName;
    [SerializeField] TextMeshProUGUI TileCost;
    void Start()
    {
        Tile tile = GetComponentInParent<Tile>();
        TileName.text = tile.GetName();
        TileCost.text = "Cost : " + tile.GetWalkableValue();
        this.gameObject.SetActive(false);
    }

    public void UpdateUi()
    {
        Tile tile = GetComponentInParent<Tile>();
        TileName.text = tile.GetName();
        TileCost.text = "Cost : " + tile.GetWalkableValue();
    }
}
