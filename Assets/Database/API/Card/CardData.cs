using System;
using Database.API.Card.Extra;
using UnityEditor;
using UnityEngine;

namespace Database.API.Card
{
    [CreateAssetMenu(fileName = "NewCardData", menuName = "Card")]
    public sealed class CardData : ScriptableObject
    {
        private const string ARGUMENT_NULL_ERROR_MESSAGE = "Card Data build argument is null!";
        private const string DEFAULT_ASSET_RESOURCES_DIRECTORY = "CardAssets";
        private const string CARD_DATA_RESOURCES_PATH = "Assets/CardGame/Resources";

        [Tooltip("The card's unique name")]
        [SerializeField] 
        private string m_CardName;
        /// <summary>
        /// The card's unique name
        /// </summary>
        public string CardName => m_CardName;
        
        [Tooltip("The card's cost")]
        [SerializeField]
        private int m_Cost;
        /// <summary>
        /// The card's cost
        /// </summary>
        public int Cost => m_Cost;
        
        [Tooltip("The card's sprite")]
        [SerializeField]
        private Sprite m_Art;
        /// <summary>
        /// The card's sprite
        /// </summary>
        public Sprite Art => m_Art;
        
        [SerializeField]
        [HideInInspector]
        private string m_ExtraDataTypeName;
        /// <summary>
        /// The extra data type name.
        /// </summary>
        public string ExtraDataTypeName
        {
            get => m_ExtraDataTypeName;
            private set => m_ExtraDataTypeName = value;
        }
        
        [Tooltip("The card's extra data")]
        [SerializeReference]
        private ExtraData m_ExtraData = null;
        
        /// <summary>
        /// The card's extra data
        /// </summary>
        public ExtraData ExtraData => m_ExtraData;
        
        /// <summary>
        /// Load all card assets to memory from default resources directory: <see cref="DEFAULT_ASSET_RESOURCES_DIRECTORY"/>.
        /// </summary>
        /// <returns>An array of card data</returns>
        public static CardData[] LoadAllCards()
        {
            return Resources.LoadAll<CardData>(path: DEFAULT_ASSET_RESOURCES_DIRECTORY);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Build a new Card Data asset that will be saved in the default card data directory.
        /// </summary>
        /// <param name="name"><see cref="CardName"/></param>
        /// <param name="cost"><see cref="Cost"/></param>
        /// <param name="artPath"><see cref="Cost"/></param>
        /// <param name="extraData"><see cref="ExtraData"/></param>
        /// <exception cref="ArgumentNullException">If any of the arguments is null</exception>
        /// <returns>A brand new asset ref for card data</returns>
        public static CardData BuildNewAsset(string name, int cost, string artPath, ExtraData extraData)
        {
            Validate(name, cost, artPath, extraData);
            
            CardData cardData = BuildEmptyAsset(name);
            
            SetData(name, cost, artPath, extraData, cardData);
            
            return cardData;
        }
        
        /// <summary>
        /// Update an assets data.
        /// </summary>
        public void UpdateAsset(int cost, string artPath, ExtraData extraData)
        {
            Validate(m_CardName, cost, artPath, extraData);
            UpdateData(cost, artPath, extraData);
        }
        
        private static void Validate(string name, int cost, string artPath, ExtraData extraData)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name), ARGUMENT_NULL_ERROR_MESSAGE);
            }
            
            if (artPath == null)
            {
                throw new ArgumentNullException(nameof(artPath), ARGUMENT_NULL_ERROR_MESSAGE);
            }

            if (extraData == null)
            {
                throw new ArgumentNullException(nameof(extraData), ARGUMENT_NULL_ERROR_MESSAGE);
            }
            
            extraData.Validate();
        }
        
        private static void SetData(string name, int cost, string artPath, ExtraData extraData, CardData cardData)
        {
            cardData.m_CardName = name;
            cardData.m_Cost = cost;
            cardData.SetExtraData(extraData);
            cardData.m_Art = Resources.Load<Sprite>(artPath);
            
            AssetDatabase.SaveAssets();
        }
        
        private void UpdateData(int cost, string artPath, ExtraData extraData)
        {
            m_Cost = cost;
            m_ExtraData = extraData;
            m_Art = Resources.Load<Sprite>(artPath);
            
            AssetDatabase.SaveAssets();
        }
        
        private static CardData BuildEmptyAsset(string name)
        {
            CardData asset = CreateInstance<CardData>();
            
            AssetDatabase.CreateAsset(asset, $"{CARD_DATA_RESOURCES_PATH}/{DEFAULT_ASSET_RESOURCES_DIRECTORY}/{name}.asset");
            
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            
            return asset;
        }
        
        internal void SetExtraData(ExtraData extraData)
        {
            m_ExtraData = extraData;
            ExtraDataTypeName = m_ExtraData == null ? string.Empty : extraData.GetType().Name;
                        
            AssetDatabase.SaveAssets();
        }
#endif
    }
}