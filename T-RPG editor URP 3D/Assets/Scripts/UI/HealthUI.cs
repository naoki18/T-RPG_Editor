using UnityEngine;

public class HealthUI : MonoBehaviour
{
    public void Start()
    {
        GetComponentInParent<Unit>().OnDamage += ChangeHealth;
    }
    public void ChangeHealth(int currentHp, int maxHp)
    {
        Vector3 scale = Vector3.one;
        scale.x = (float)currentHp / (float)maxHp;
        this.transform.localScale = scale;
    }
}
