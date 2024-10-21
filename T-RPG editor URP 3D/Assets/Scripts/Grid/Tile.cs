using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] short walkableValue = 0;

    MeshRenderer mRenderer;
    Unit unitOnTile;

    Color baseColor;

    [SerializeField] TileInformationUI informationUi;

    public static Tile CreateTile(ScriptableTile data)
    {
        Tile tile = Instantiate(GridManager.Instance.GetTilePf(), Vector3.zero, Quaternion.identity);
        tile.mRenderer = tile.GetComponent<MeshRenderer>();
        tile.unitOnTile = null;
        tile.walkableValue = data.walkableValue;
        tile.mRenderer.material = data.material;
        tile.baseColor = tile.mRenderer.material.color;
        return tile;
    }

    public void SetCharacter(Unit character)
    {
        unitOnTile = character;
    }

    public short GetWalkableValue()
    {
        if (unitOnTile != null) return -1;
        return walkableValue;
    }

    /// <summary>
    /// Send value between 1 and 0.
    /// Value over 1 will be clamped to 1.
    /// Value under 0 will be clamped to 0.
    /// </summary>
    /// <param name="highlightPercent"></param>
    public void Highlight(float highlightPercent)
    {
        highlightPercent = Mathf.Clamp01(highlightPercent);
        Color color = mRenderer.material.color;
        color.r = Mathf.Lerp(color.r, 1, highlightPercent);
        color.g = Mathf.Lerp(color.g, 1, highlightPercent);
        color.b = Mathf.Lerp(color.b, 1, highlightPercent);

        mRenderer.material.color = color;
    }

    public void RemoveHighlight()
    {
        mRenderer.material.color = baseColor;
    }
    public bool IsValid()
    {
        if (walkableValue == -1 || unitOnTile != null) return false;
        return true;
    }

    public Unit GetCharacter()
    {
        return unitOnTile;
    }

    public void ShowInformation()
    {
        informationUi.UpdateUi();
        informationUi.gameObject.SetActive(true);
    }

    public void HideInformation()
    {
        informationUi.gameObject.SetActive(false);
    }
}
