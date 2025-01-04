[System.Serializable]
public class ArmorData : ItemData
{
    public int Defence;
    public bool IsEquipped;


    public ArmorData() : base()
    {
    }

    public ArmorData(Armor armor) : base(armor)
    {
        Defence = armor.Defence.GetCurrentValue();
        IsEquipped = armor.IsEquipped;
    }

    public override Item ToItem()
    {
        var armor = base.ToItem() as Armor;
        armor.Defence.SetCurrentValue(Defence);
        armor.IsEquipped = IsEquipped;
        return armor;
    }
}