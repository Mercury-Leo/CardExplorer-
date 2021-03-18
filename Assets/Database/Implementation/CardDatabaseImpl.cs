using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Database.API;
using Database.API.Card;
using Database.API.Card.Extra;
using UnityEditor;
using UnityEngine;
using static SupportedFileTypes;

namespace Database.Implementation
{
    public class CardDatabaseImpl : CardDatabase
    {
        private const string RESOURCES_FOLDER = "Resources";
        private const string DEFAULT_ASSET_DIRECTORY = "CardAssets";
        private const string PATH_TO_REMOVE = "Assets/CardGame/Resources/";
        private const string EXPORT_ASSET_PATH = "Assets/CardGame/Resources/TextAssets/";

        private FileReaders fileReaders;
        private List<CardData> _allCardsData;
        private List<CardDataStructure> currentCards;
        private Dictionary<string, Func<string, List<CardDataStructure>>> DataReader;
        private Dictionary<string, Func<string, List<CardDataStructure>, bool>> DataWriter;

        private enum FileModes {Read, Write};

        public override IEnumerable<CardData> AllCards => _allCardsData;

        public override void Init()
        {
            _allCardsData = (CardData.LoadAllCards()).ToList();
        }

        public override CardData GetByName(string name)
        {
            return _allCardsData.Find(card => card.name == name);
        }

        public override List<CardData> GetCardsByCost(int cost)
        {
            return _allCardsData.Where(card => card.Cost == cost).ToList();
        }

        /// <summary>
        /// Turns _allCardsData into a list of CardDataStructre.
        /// </summary>
        /// <returns></returns>
        private List<CardDataStructure> CardDataToCards()
        {
            return _allCardsData.Select(card => {
                return new CardDataStructure(card.name, card.Cost, AssetDatabase.GetAssetPath(card.Art), card.ExtraData.GetType().Name, card.ExtraData);
            }).ToList(); 
        } 

        /// <summary>
        /// Imports the card data from a source file.
        /// Gets a new FileReader and init the FileReader.
        /// If all the cards are read correctly, create/update the Scriptable Objects.
        /// </summary>
        /// <param name="source"></param>
        public override void ImportCardsFromSource(string source)
        {
            fileReaders = new FileReaders();
            DataReader = fileReaders.InitReaderDict();

            currentCards = DataHandler(source, FileModes.Read);

            if (currentCards != null)
                AddCardAssets();
        }

        /// <summary>
        /// Makes a new FileReader(Shoudld have changed that name) and inits the types of available writters.
        /// Fixes source path for the file writters.
        /// Calls DataHandler to get the correct File writter and write the contents into the source file.
        /// </summary>
        /// <param name="source"></param>
        public override void ExportCardsToSource(string source)
        {
            fileReaders = new FileReaders();
            DataWriter = fileReaders.InitWriterrDict();
            source = EXPORT_ASSET_PATH + source;
            DataHandler(source, FileModes.Write);
        }

        /// <summary>
        /// Checks the source string for the file ending, if the source has a valid file ending (From FileTypes enum).
        /// If the source is valid, gets the correct function from the FileModes sent.
        /// Returns a list of the read values if in read mode, returns nothing in write mode.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        private List<CardDataStructure> DataHandler(string source, FileModes mode)
        {
            string fileExtesion = GetFileExtension(source);
            if (Enum.IsDefined(typeof(FileTypes), fileExtesion))
            {
                if (mode is FileModes.Read)
                    return DataReader[fileExtesion](source);
                else if (mode is FileModes.Write)
                    DataWriter[fileExtesion](source, CardDataToCards());
            }
            else
                throw new UnidentifiedCardSourceException(fileExtesion);

            return null;
        }

        /// <summary>
        /// Returns the file extension of a file.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private string GetFileExtension(string source)
        {
            string fileExtesion = Path.GetExtension(source);
            fileExtesion = fileExtesion.ToLower(); //Make sure file extension is lowercase.
            fileExtesion = fileExtesion.Replace(".", "");
            return fileExtesion;
        }

        /// <summary>
        /// Creates or updates the cardsdata.
        /// Goes over _allCardsData by the ScriptableObject name and not the card name. 
        /// Card name isnt updated with UpdateAsset so the SO will have an empty cardName.
        /// </summary>
        private void AddCardAssets()
        {
            foreach (CardDataStructure card in currentCards)
            {
                //Trims the ArtPath to begin after the resoucres folder because Resources.load<sprite> needs to get a path after resources file and without file extensions.
                card.ArtPath = card.ArtPath.Replace(PATH_TO_REMOVE, "").Replace(Path.GetExtension(card.ArtPath), "");
                //Check if a card already exists in the database, in case it does, open it from the resources folder and update it.
                if (_allCardsData.Find(curr_card => curr_card.name.Equals(card.Name)))
                {
                    CardData card_holder = Resources.Load<CardData>($"{DEFAULT_ASSET_DIRECTORY}/{card.Name}");
                    if (card_holder != null)
                        card_holder.UpdateAsset(card.Cost, card.ArtPath, card.ExtraData);
                    else
                        throw new Exception("Failed to update card asset");
                }
                else
                    ImportCardsToResources(card.Name, card.Cost, card.ArtPath, card.ExtraData);
            }
        }

        /// <summary>
        /// Creates a new asset by the card params.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cost"></param>
        /// <param name="artPath"></param>
        /// <param name="extraData"></param>
        private void ImportCardsToResources(string name, int cost, string artPath, ExtraData extraData)
        {
            CardData.BuildNewAsset(name, cost, artPath.ToString(), extraData);
        }
    }
}
