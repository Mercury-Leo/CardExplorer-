using Database.API.Card.Extra;
using System.Collections.Generic;
using System.Xml.Serialization;
//TODO: Turns XmlExtra into two classes that inherit from it so the xml output wouldnt hold both types of card data together.
[XmlRoot(ElementName = "ExtraData")]
public class XmlExtra
{

    [XmlElement(ElementName = "Effect")]
    public int Effect;

    [XmlElement(ElementName = "EffectAmount")]
    public int EffectAmount;

    [XmlElement(ElementName = "AttackDamage")]
    public int AttackDamage;

    [XmlElement(ElementName = "Health")]
    public int Health;

    public XmlExtra(ExtraData extraData)
    {
        if (extraData is SpellExtraData)
        {
            Effect = (int)((SpellExtraData)extraData).Effect;
            EffectAmount = ((SpellExtraData)extraData).EffectAmount;
        }
        if (extraData is MinionExtraData)
        {
            AttackDamage = ((MinionExtraData)extraData).AttackDamage;
            Health = ((MinionExtraData)extraData).Health;
        }
    }

    public XmlExtra()
    {

    }

}

/// <summary>
/// The xml structure used in serialization and deserialization.
/// </summary>
[XmlRoot(ElementName = "element")]
public class XmlDataStructure
{

    [XmlElement(ElementName = "ArtPath")]
    public string ArtPath;

    [XmlElement(ElementName = "AttackDamage")]
    public int AttackDamage;

    [XmlElement(ElementName = "Cost")]
    public int Cost;

    [XmlElement(ElementName = "ExtraData")]
    public XmlExtra ExtraData;

    [XmlElement(ElementName = "ExtraDataTypeName")]
    public string ExtraDataTypeName;

    [XmlElement(ElementName = "Health")]
    public int Health;

    [XmlElement(ElementName = "Name")]
    public string Name;

    public XmlDataStructure()
    {

    }

    public XmlDataStructure(string artPath, int attackDamage, int cost, XmlExtra extraData, string extraDataTypeName, int health, string name)
    {
        ArtPath = artPath;
        AttackDamage = attackDamage;
        Cost = cost;
        ExtraData = extraData;
        ExtraDataTypeName = extraDataTypeName;
        Health = health;
        Name = name;
    }
}

[XmlRoot(ElementName = "root")]
public class CardNodes
{
    [XmlElement(ElementName = "element")]
    public List<XmlDataStructure> cardsData;
}
