public class Weapon
{
    private int attack;
    private int range;
    private int width;
    public static Weapon InstantiateWeapon(ScriptableWeapon data)
    {
        Weapon newWeapon = new()
        {
            attack = data.attack,
            range = data.range
        };
        return newWeapon;
    }

    public void Attack(Unit unit)
    {
        unit.Damage(attack);
    }
}
