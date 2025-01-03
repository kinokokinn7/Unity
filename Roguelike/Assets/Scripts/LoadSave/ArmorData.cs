[System.Serializable]
public class ArmorData : ItemData
{
    public int Defence;
    public bool IsEquipped;


    public ArmorData() : base()
    {
    }

    public ArmorData(Armor Armor) : base(Armor)
    {
        Defence = Armor.Defence.GetCurrentValue();
        IsEquipped = Armor.IsEquipped;
    }

    public override Item ToItem()
    {
        var Armor = base.ToItem() as Armor;
        Armor.Defence.SetCurrentValue(Defence);
        Armor.IsEquipped = IsEquipped;
        return Armor;
    }
}