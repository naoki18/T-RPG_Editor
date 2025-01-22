using TMPro;
using UnityEngine;

public class TileInformationUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI TileName;
    [SerializeField] TextMeshProUGUI TileCost;
    //void Start()
    //{
    //    this.gameObject.SetActive(false);
    //}

    public void UpdateUi(Tile tile)
    {
        TileName.text = tile.GetName();
        TileCost.text = "Cost : " + tile.GetWalkableValue();
    }
}
