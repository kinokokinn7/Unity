using Newtonsoft.Json;
using System;

public class ItemDataConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ItemData);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jsonObject = Newtonsoft.Json.Linq.JObject.Load(reader);
        var type = jsonObject["Type"].ToString();

        ItemData itemData;
        switch (type)
        {
            case "Weapon":
                itemData = new WeaponData();
                break;
            case "LifeRecoveryItem":
                itemData = new LifeRecoveryItemData();
                break;
            case "FoodRecoveryItem":
                itemData = new FoodRecoveryItemData();
                break;
            default:
                itemData = new ItemData();
                break;
        }

        serializer.Populate(jsonObject.CreateReader(), itemData);
        return itemData;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}
