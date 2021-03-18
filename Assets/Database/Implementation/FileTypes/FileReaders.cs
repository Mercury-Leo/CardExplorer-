using Database.API.Card;
using Database.API.Card.Extra;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using static SupportedFileTypes;
/// <summary>
/// FileReaders will have all the different file readers and writers and hold them inside a dict SupportedFileReaders also stores another dict of the SupportedFileWriters.
/// </summary>
namespace Database.Implementation
{
    public class FileReaders
    {
        //Path to the asset folder
        private static string ASSET_PATH = "Assets/CardGame/Resources/TextAssets/";
        private const string DEFAULT_ASSET_DIRECTORY = "CardAssets";
        private static string file_contents;

        //The Dicts will hold a file ending to a specific function so it may be used in the Database without telling it what type of source is given.
        private Dictionary<string, Func<string, List<CardDataStructure>>> SupportedFileReaders = new Dictionary<string, Func<string, List<CardDataStructure>>>();
        private Dictionary<string, Func<string, List<CardDataStructure>, bool>> SupportedFileWriters = new Dictionary<string, Func<string, List<CardDataStructure>, bool>>();

        //Will initialize the dict with a key value pair of file type with its file reader. Add a new file reader here in case of extending supported files.
        public Dictionary<string, Func<string, List<CardDataStructure>>> InitReaderDict()
        {
            SupportedFileReaders.Add(FileTypes.json.ToString(), JsonReader);
            SupportedFileReaders.Add(FileTypes.xml.ToString(), XMLReader);

            return SupportedFileReaders;
        }

        /// <summary>
        /// If another file reader/writer needs to be added, need to add it to the dict and to the enum FileTypes
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Func<string, List<CardDataStructure>, bool>> InitWriterrDict()
        {
            SupportedFileWriters.Add(FileTypes.json.ToString(), JsonWriter);
            SupportedFileWriters.Add(FileTypes.xml.ToString(), XmlWriter);

            return SupportedFileWriters;
        }


        /// <summary>
        /// Will read the data of a json file from source and return the data.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static List<CardDataStructure> JsonReader(string source)
        {
            source = AddPath(source);
            file_contents = File.ReadAllText(source);
            file_contents = AddJsonObject(file_contents, "cards");
            return CardsToList(JsonConvert.DeserializeObject<Cards>(file_contents, new ExtraConverter()));
        }

        /// <summary>
        /// Turns a Cards to a readable list of CardDataStructure.
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        private static List<CardDataStructure> CardsToList(Cards cards)
        {
            //List<CardDataStructure> temp_cards = new List<CardDataStructure>();
            return cards.cards.Select(card =>
            {
                return new CardDataStructure(card.Name, card.Cost, card.ArtPath, card.ExtraDataTypeName, card.ExtraData);
            }).ToList();
        }

        private static bool JsonWriter(string source, List<CardDataStructure> cardData)
        {
            JsonCall(source, cardData);
            return true;
        }

        private static async void JsonCall(string source, List<CardDataStructure> cardData)
        {
            using (StreamWriter file = File.CreateText(source))
            {
                List<CardDataStructure> cardsListToExport = cardData;
                await file.WriteLineAsync(JsonConvert.SerializeObject(cardsListToExport, Newtonsoft.Json.Formatting.Indented));
            }
        }

        /// <summary>
        /// Writes a list of CardDataStrcutre to an XML file.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="cardData"></param>
        /// <returns></returns>
        private static bool XmlWriter(string source, List<CardDataStructure> cardData)
        {
            CardNodes cardNodes = new CardNodes();
            cardNodes.cardsData = cardData.Select(card =>
            {
                return new XmlDataStructure(card.ArtPath, 0, card.Cost, new XmlExtra(card.ExtraData), card.ExtraDataTypeName,0, card.Name);
            }).ToList();            
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CardNodes));
            using (StreamWriter stream = File.CreateText(source))
            {
                xmlSerializer.Serialize(stream, cardNodes);
            }

            return false;
        }


        /// <summary>
        /// Will read the data of a xml file from source and return a list of CardDataStructure.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static List<CardDataStructure> XMLReader(string source)
        {
            //Adds a path the source needs.
            source = AddPath(source);
            var serializer = new XmlSerializer(typeof(CardNodes));
            var xmlReader = new XmlTextReader(source);
            CardNodes xmlData = (CardNodes)serializer.Deserialize(xmlReader);
            var tempHolder = new List<XmlDataStructure>(xmlData.cardsData.Cast<XmlDataStructure>());
            return tempHolder.Select(card =>
            {
                if (card.ExtraDataTypeName == "SpellExtraData") //Checks what type of card we have in order to create the correct ExtraData.
                {
                    SpellExtraData spellExtra = new SpellExtraData((SpellEffect)card.ExtraData.Effect, card.ExtraData.EffectAmount);
                    return new CardDataStructure(card.Name, card.Cost, card.ArtPath, card.ExtraDataTypeName, spellExtra);
                }
                else if (card.ExtraDataTypeName == "MinionExtraData")
                {
                    MinionExtraData minionExtra = new MinionExtraData(card.ExtraData.Health, card.ExtraData.AttackDamage);
                    return new CardDataStructure(card.Name, card.Cost, card.ArtPath, card.ExtraDataTypeName, minionExtra);
                }
                else
                    return null; //Returns null if its an unsupported Card type.
            }).ToList();
        }

        /// <summary>
        /// Adds the asset path to the source.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static string AddPath(string source)
        {
            return ASSET_PATH + source;
        }

        /// <summary>
        /// Adds a json object on contents for the json deserializer 
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="obj_name"></param>
        /// <returns></returns>
        private static string AddJsonObject(string contents, string obj_name)
        {
            return "{\"" + obj_name + "\":" + contents + "}";
        }

    }

}
