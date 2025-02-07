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
        if (tile.GetWalkableValue() == -1)
        {
            TileCost.text = "Occupied"; 
            return;
        }
        TileCost.text = "Cost : " + tile.GetWalkableValue();
    }
}
