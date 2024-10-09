using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] Color baseColor;
    [SerializeField] Color oddColor;
    [SerializeField] Color highlightedColor;
    [SerializeField] SpriteRenderer sRenderer;

    Unit unitOnTile;
    bool isOdd = false;

    public void Init(int x, int y)
    {
        isOdd = ((x % 2 == 0) && (y % 2 != 0)) || ((y % 2 == 0) && (x % 2 != 0));
        SetColor(isOdd);
    }
    public void SetColor(bool isOdd)
    {
        if (isOdd) sRenderer.color = oddColor;
        else sRenderer.color = baseColor;
    }

    public void OnMouseEnter()
    {
        sRenderer.color = highlightedColor;
    }

    public void OnMouseExit()
    {
        SetColor(isOdd);
    }

    public void OnMouseDown()
    {
        if(unitOnTile == null) return;
        Debug.Log("There is character !");
    }

    public void SetCharacter(Unit character)
    {
        unitOnTile = character;
    }
}
