using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Android;
using UnityEngine.UI;

public class DeckHandler : MonoBehaviour
{
    public GameObject ifEmptyDeck;
    public GameObject canvas;
    public GameObject cardPrefab;
    public BattlePlayer Player;

    public List<GameObject> originalDeck = new List<GameObject>();
    public List<GameObject> hand = new List<GameObject>();
    public List<GameObject> CreatedCards = new List<GameObject>();
    public Dictionary<Guid, GameObject[]> CombinedCards = new Dictionary<Guid, GameObject[]>();


    private void Start()
    {
        canvas = gameObject;
    }

    public void initialise(BattlePlayer player)
    {
        this.Player = player;
        Button button = GetComponentInChildren<Button>();
        button.onClick.AddListener(Player.FinishTurn);
    }

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

    private List<GameObject> CreateDefaultList(List<GameObject> deck)
    {
        for (int i = 0; i < 10; i++)
        {
            deck.Add(ifEmptyDeck);
        }
        return deck;
    }

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

    public void AddCard(GameObject cardToAdd, GameObject[] combined = null)
    {
        Debug.Log("adding new card");
        int position = hand.Count - 1;

        GameObject newCard = Instantiate(cardPrefab, cardPrefab.transform.position, Quaternion.identity, this.transform);
        addData(newCard, cardToAdd);
        hand.Add(newCard);

        if (combined != null)
        {
            Debug.Log("made it to this stage");
            CombinedCards.Add(newCard.GetComponent<Card>().CardID, combined);
            Debug.Log(combined.Length);

            for (int i = 0; i < combined.Length; i++)
            {
                Debug.Log("Removing From Hand");
                hand.Remove(combined[i]);
                combined[i].SetActive(false);
            }
        }


        cardToAdd.SetActive(true);
        UpdateDeck();



    }

    private void addData(GameObject card, GameObject cardData)
    {
        card.GetComponent<Card>().Player = Player;
        card.GetComponent<Card>().CardDataHolder = cardData.GetComponent<CardData>();
        card.GetComponent<Card>().CardDataHolder.initialise();

    }

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
