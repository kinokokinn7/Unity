[System.Serializable]
public class WeaponData : ItemData
{
    public int Attack;
    public bool IsEquipped;


    public WeaponData() : base()
    {
    }

    public WeaponData(Weapon weapon) : base(weapon)
    {
        Attack = weapon.Attack.GetCurrentValue();
        IsEquipped = weapon.IsEquipped;
    }

    public override Item ToItem()
    {
        var weapon = base.ToItem() as Weapon;
        weapon.Attack.SetCurrentValue(Attack);
        weapon.IsEquipped = IsEquipped;
        return weapon;
    }
}