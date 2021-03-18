using Database.API.Card;
using Database.API.Card.Extra;
using Database.Implementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// CardViewer will handle the card view. It will manage updating the scroll view content with cards.
/// Will find cards in the data base by user input (Cost or Name).
/// </summary>
public class CardViewer : MonoBehaviour
{

    private const string IMPORT_ASSET_PATH = "Assets/CardGame/Resources/TextAssets/";
    private const string CARDTEMPLATE_NAME = "CardTemplate";

    [SerializeField] private GameObject SpellCardPrefab;
    [SerializeField] private GameObject MinionCardPrefab;
    [SerializeField] private GameObject CardViewContent;
    [SerializeField] private InputField NameField;
    [SerializeField] private InputField CostField;

    private CardDatabaseImpl cardDatabase;
    private GameObject mainCanvas;
    private GameObject cardScroller;
    private List<GameObject> ActiveCards;
    private enum CardType { typeSpell, typeMinion };

    void Awake()
    {
        Assert.IsNotNull(SpellCardPrefab);
        Assert.IsNotNull(MinionCardPrefab);
        Assert.IsNotNull(CardViewContent);
        cardScroller = GameObject.Find("CardScroll");
        mainCanvas = GameObject.Find("Canvas");
        cardDatabase = new CardDatabaseImpl();
        cardDatabase.Init();
        LoadCardsData();
    }

    void Start()
    {
        cardDatabase.Init();
        ActiveCards = new List<GameObject>();
        InitCardScreen();
    }

    private void InitCardScreen()
    {
        InitCardsPrefabs(cardDatabase.AllCards.ToList());
        AddCardsToView(ActiveCards);
    }

    /// <summary>
    /// Finds all the files in the TextAssets and imports the data to be used.
    /// </summary>
    private void LoadCardsData()
    {
        FileInfo[] files = new DirectoryInfo(IMPORT_ASSET_PATH).GetFiles().Where(name => !name.Name.Contains(".meta")).ToArray();

        foreach(FileInfo file in files)
        {
            if (File.Exists(file.FullName))
                cardDatabase.ImportCardsFromSource(file.Name);
        }
    }

    /// <summary>
    /// Adds a list of cards as a child of the scroll view parent.
    /// </summary>
    private void AddCardsToView(List<GameObject> cardsToAdd)
    {
        foreach(GameObject card in cardsToAdd)
            card.transform.SetParent(CardViewContent.transform); 
    }

    /// <summary>
    /// Initiate prefabs based on the type of card.
    /// </summary>
    /// <param name="cardList"></param>
    private void InitCardsPrefabs(List<CardData> cardList)
    {
        GameObject card_holder = new GameObject();
        foreach (CardData card in cardList)
        {
            if (card.ExtraData is SpellExtraData)
            {
                card_holder = Instantiate(SpellCardPrefab);
                ActiveCards.Add(UpdatePrefabData(card_holder, card, (int)CardType.typeSpell));
            }
            else if (card.ExtraData is MinionExtraData)
            {
                card_holder = Instantiate(MinionCardPrefab);
                ActiveCards.Add(UpdatePrefabData(card_holder, card, (int)CardType.typeMinion));
            }
            else
            {
                throw new Exception($"Unsupported card type: {card.ExtraData.GetType().Name}.");
            }
        }
    }

    /// <summary>
    /// Loads card data based on user input; Returns a list of either speific card by its name or a list of cards that share the same cost.
    /// </summary>
    public void LoadCardsByParam()
    {
        ClearViewList();
        string nameSearch = NameField.text;
        string costSearch = CostField.text;
        List<CardData> cardParamList = new List<CardData>();
        //Name search takes priority over cost search
        if (nameSearch != "")
        {
            cardParamList.Add(cardDatabase.GetByName(nameSearch));
            if(cardParamList.Count == 1)
                InitCardsPrefabs(cardParamList);
        }
        else if(costSearch != "")
        {
            cardParamList = cardDatabase.GetCardsByCost(int.Parse(costSearch));
            if(cardParamList.Count > 0)
                InitCardsPrefabs(cardParamList);
        }
        else
            InitCardsPrefabs(cardDatabase.AllCards.ToList());//If no search was entered, load the cards in memory.

        NameField.text = "";
        CostField.text = "";
        AddCardsToView(ActiveCards);
    }

    /// <summary>
    /// Clears the current cards on the view and resets the ActiveCards list.
    /// </summary>
    private void ClearViewList()
    {
        foreach (Transform cardItem in CardViewContent.transform)
            Destroy(cardItem.gameObject);

        ActiveCards = new List<GameObject>();
    }

    /// <summary>
    /// Updates the prefab to hold the data from cardData.
    /// </summary>
    /// <param name="card"></param>
    /// <param name="cardData"></param>
    /// <param name="cardtype"></param>
    private GameObject UpdatePrefabData(GameObject card, CardData cardData, int cardtype)
    {
        //Finds the card art in the prefab and loads into it the art from the CardData.
        card.transform.Find("CardArt").GetComponent<Image>().sprite = cardData.Art;
        //Finds the CardName in the prefab and loads into it the Card name.
        card.transform.Find(CARDTEMPLATE_NAME).transform.Find("CardName").GetComponent<Text>().text = cardData.name;
        card.transform.Find(CARDTEMPLATE_NAME).transform.Find("Cost").transform.GetChild(0).transform.GetComponent<Text>().text = cardData.Cost.ToString();
        if(cardtype == (int)CardType.typeSpell)
        {
            var spellData = (SpellExtraData)cardData.ExtraData;
            card.transform.Find(CARDTEMPLATE_NAME).transform.Find("Effect").transform.GetChild(0).transform.GetComponent<Text>().text = spellData.EffectAmount.ToString();
            card.transform.Find(CARDTEMPLATE_NAME).transform.Find("SpellEffect").GetComponent<Text>().text = "Spell effect: " + (spellData.Effect).ToString();
        }
        if(cardtype == (int)CardType.typeMinion)
        {
            var minionData = (MinionExtraData)cardData.ExtraData;
            card.transform.Find(CARDTEMPLATE_NAME).transform.Find("Health").transform.GetChild(0).transform.GetComponent<Text>().text = minionData.Health.ToString();
            card.transform.Find(CARDTEMPLATE_NAME).transform.Find("Attack").transform.GetChild(0).transform.GetComponent<Text>().text = minionData.AttackDamage.ToString();
        }
        return card;
    }

}
