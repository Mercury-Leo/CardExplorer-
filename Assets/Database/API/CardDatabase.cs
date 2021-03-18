using System.Collections.Generic;
using Database.API.Card;

namespace Database.API
{
    /// <summary>
    /// version 1.2
    /// Unity CCG Card database API. Make sure you implement all calls.
    /// Good Luck!
    /// </summary>
    public abstract class CardDatabase
    {
        /// <summary>
        /// Get all cards in the collection.
        /// </summary>
        public abstract IEnumerable<CardData> AllCards { get; }
        
        /// <summary>
        /// Initialize the database with existing card data assets in the project pre defined folder.
        /// </summary>
        public abstract void Init();
        
        /// <summary>
        /// Load cards from a specific source (i.e. SpellCards.json).
        /// </summary>
        /// <param name="source">Import cards from the specified source, create assets and add them to the database.</param>
        /// <exception cref="UnidentifiedCardSourceException">When the source is unidentified.</exception>
        public abstract void ImportCardsFromSource(string source);
        
        /// <summary>
        /// Export all database cards to a specific source (i.e. SpellCards.json).
        /// </summary>
        /// <param name="source">Import cards from the specified source, create assets and add them to the database.</param>
        /// <exception cref="UnidentifiedCardSourceException">When the source is unidentified.</exception>
        public abstract void ExportCardsToSource(string source);
        
        /// <summary>
        /// Get all cards from the collection with the given cost.
        /// </summary>
        /// <param name="cost">The expected cost of the card.</param>
        /// <returns>A list of cards in the given cost. If none exist the list will return empty</returns>
        public abstract List<CardData> GetCardsByCost(int cost);
        
        /// <summary>
        /// Get a card by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A requested card. Null if none exists.</returns>
        public abstract CardData GetByName(string name);
    }
}