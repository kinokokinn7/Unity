[System.Serializable]
public class WeaponData : ItemData
{
    public int Attack;

    public WeaponData(Weapon weapon) : base(weapon)
    {
        Attack = weapon.Attack.GetCurrentValue();
    }

    public override Item ToItem()
    {
        var weapon = base.ToItem() as Weapon;
        weapon.Attack.SetCurrentValue(Attack);
        return weapon;
    }
}