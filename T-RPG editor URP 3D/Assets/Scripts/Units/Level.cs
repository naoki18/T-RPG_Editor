using System;
using UnityEngine;

[Serializable]
public class Level
{
    public event Action<int> OnLevelChange;

    [SerializeField, Min(1)] int base_level = 1;
    [SerializeField, Tooltip("Xp needed from level 1 to 2")] int BaseXp = 10;

    int current_level = 1;
    int xp = 0;

    public void Init()
    {
        current_level = base_level;
        Debug.Log(GetAmountXpToLvlUp());
    }
    
    private int GetAmountXpToLvlUp()
    {
        // TODO : Create growth xp function
        return BaseXp * current_level;
    }

    public void GetXp(int amount)
    {
        xp += amount;
        while(xp >= GetAmountXpToLvlUp())
        {
            xp = xp - GetAmountXpToLvlUp();
            current_level++;
            OnLevelChange.Invoke(current_level);
        }
    }
}
