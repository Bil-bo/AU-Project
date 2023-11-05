using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;

// DeckHandler Attached to each Player to show and manage their cards
public class DeckHandler : MonoBehaviour
{
    // Prefab for playtesting when loading directly into the battle scene
    public GameObject ifEmptyDeck;
    public GameObject canvas; // Unnecessary but I'm scared to remove
    public GameObject cardPrefab;
    public BattlePlayer Player; 

    // Original Deck - The player's current deck - not to be altered here only to create cards from
    // hand - cards to be shown in the battle scene
    // Created Cards - Instantiated cards that can be drawn from
    // Combined cards - Holds cards that have been combined from: unimplemented - restoring combined cards if a respective combined card is used
    public List<GameObject> originalDeck = new List<GameObject>();
    public List<GameObject> hand = new List<GameObject>();
    public List<GameObject> CreatedCards = new List<GameObject>();
    public Dictionary<Guid, GameObject[]> CombinedCards = new Dictionary<Guid, GameObject[]>();


    // Scared to remove
    private void Start()
    {
        canvas = gameObject;
    }


    // Called when the deck handler is created by a player, otherwise some things aren't set up correctly by start or awake
    public void initialise(BattlePlayer player)
    {
        this.Player = player;
        Button button = GetComponentInChildren<Button>();
        button.onClick.AddListener(Player.FinishTurn);
    }

    // Creates and shows a Hand for the current player, also the end turn button
    public void ShowDeck()
    {

        if (originalDeck.Count <= 0) { InitializeDeck();  }
        shuffleDeck(CreatedCards);

        for (int i = 0; i < Player.maxHand; i++) 
        {
            GameObject proxy = CreatedCards[0];
            hand.Add(proxy);
            CreatedCards.Remove(proxy);
        }

        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        UpdateDeck();


    }

    // TODO: Rename this or above
    // Puts all cards in hand back into createdCards, and hides the end turn button
    public void HideHand()
    {
        int counter = hand.Count;
        for (int i = 0; i < counter; i++)
        {
            GameObject proxy = hand[0];
            CreatedCards.Add(proxy);
            hand.Remove(proxy);
            proxy.SetActive(false);
        }
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }


    // TODO: Change Z to S
    // Creates the deck when the handler is first created
    public void InitializeDeck()
    {
        try
        {
            originalDeck = GameData.Instance.deckToPass;
            if (originalDeck.Count == 0) { originalDeck = CreateDefaultList(originalDeck); }
        }
        catch
        {
            originalDeck = CreateDefaultList(originalDeck);
        }

        foreach (GameObject card in originalDeck)
        {
            GameObject addCard = Instantiate(cardPrefab, cardPrefab.transform.position, Quaternion.identity, this.transform);
            addData(addCard, card);
            CreatedCards.Add(addCard);
            addCard.SetActive(false);
        }
    }

    // For When Loading directly into the battle scene
    private List<GameObject> CreateDefaultList(List<GameObject> deck)
    {
        for (int i = 0; i < 10; i++)
        {
            deck.Add(ifEmptyDeck);
        }
        return deck;
    }

    // Fisher-Yates shuffle
    // First found here https://gist.github.com/jasonmarziani/7b4769673d0b593457609b392536e9f9, maybe
    public static List<GameObject> shuffleDeck(List<GameObject> cards)
    {
        System.Random rand = new System.Random();
        int n = cards.Count;
        GameObject card;

        for (int i = 0; i < n; i++)
        {
            int r = i + (int)(rand.NextDouble() * (n - i - 1));
            card = cards[i];
            cards[i] = cards[r];
            cards[r] = card;

        }
        return cards;
    }


    // For Adding a new Card to the deck
    // Currently mainly used for combining cards but should have functionality to just add cards in battle
    public void AddCard(GameObject cardToAdd, GameObject[] combined = null)
    {

        int position = hand.Count - 1;

        GameObject newCard = Instantiate(cardPrefab, cardPrefab.transform.position, Quaternion.identity, this.transform);
        addData(newCard, cardToAdd);
        hand.Add(newCard);

        // For removing cards that were combined from being pulled
        if (combined != null)
        {
            CombinedCards.Add(newCard.GetComponent<Card>().CardID, combined);
            Debug.Log(combined.Length);

            for (int i = 0; i < combined.Length; i++)
            {
                hand.Remove(combined[i]);
                combined[i].SetActive(false);
            }
        }


        cardToAdd.SetActive(true);
        UpdateDeck();



    }

    // Adds the necessary data to the current card
    private void addData(GameObject card, GameObject cardData)
    {
        card.GetComponent<Card>().Player = Player;
        card.GetComponent<Card>().CardDataHolder = cardData.GetComponent<CardData>();
        card.GetComponent<Card>().CardDataHolder.initialise();

    }

    // Fans the cards out from the middle of the screen outwards
    private void UpdateDeck()
    {
        int position = (hand.Count - 1) * 50;


        foreach (GameObject card in hand)
        {

            RectTransform rectTransform = card.GetComponent<RectTransform>();
            rectTransform.anchorMax.Set(0.5f, 0);
            rectTransform.anchorMax.Set(0.5f, 0);
            rectTransform.anchoredPosition = new Vector2(position, 0);
            card.transform.localScale = Vector3.one;

            position -= 100;
            card.SetActive(true);

        }
    }
}
