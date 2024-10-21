using TMPro;
using UnityEngine;

public class TileInformationUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TileName;
    [SerializeField] TextMeshProUGUI TileCost;
    void Start()
    {
        Tile tile = GetComponentInParent<Tile>();
        TileName.text = tile.gameObject.name;
        TileCost.text = "Cost : " + tile.GetWalkableValue();
    }
}
