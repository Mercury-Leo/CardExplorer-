using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Database.API.Card.Extra;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[System.Serializable]
//[XmlRoot(ElementName = "element")]
public class CardDataStructure
{
    //[XmlElement(ElementName = "Name")]
    public string Name; //The unique id and name of the card.

    //[XmlElement(ElementName = "Health")]
    //public int Health; //Card's base health.

    //[XmlElement(ElementName = "AttackDamage")]
    //public int AttackDamage; //Card's base attack damage.

    //[XmlElement(ElementName = "Cost")]
    public int Cost; //Card's cost to use.

    //[XmlElement(ElementName = "ArtPath")]
    public string ArtPath; //Card's art path.

    //[XmlElement(ElementName = "ExtraDataTypeName")]
    public string ExtraDataTypeName;

    //[XmlElement(ElementName = "ExtraData")]
    public ExtraData ExtraData;

    public CardDataStructure(string name, int cost, string artPath, string extraDataTypeName, ExtraData extraData)
    {
        Name = name;
        //Health = health;
        //AttackDamage = attackDamage;
        Cost = cost;
        ArtPath = artPath;
        ExtraDataTypeName = extraDataTypeName;
        ExtraData = extraData;
    }

    public CardDataStructure()
    {

    }

}

[System.Serializable]
//[XmlRoot(ElementName = "root")]
public class Cards
{
    public List<CardDataStructure> cards;
}

/// <summary>
/// Converts the abstract class ExtraData, finds if the data given is MinionExtraData or SpellExtraData and loads it to database.
/// </summary>
public class ExtraConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(ExtraData));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jo = JObject.Load(reader);

        //Checks if data is of SpellExtraData type 
        if (jo.ToString().Contains("Effect") && jo.ToString().Contains("EffectAmount"))
            return new SpellExtraData((SpellEffect)jo["EffectAmount"].Value<int>(), jo["Effect"].Value<int>());
        //Checks if data is of MinionExtraData type
        if (jo.ToString().Contains("Health") && jo.ToString().Contains("AttackDamage"))
            return new MinionExtraData(jo["Health"].Value<int>(), jo["AttackDamage"].Value<int>());  

        return null;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}









