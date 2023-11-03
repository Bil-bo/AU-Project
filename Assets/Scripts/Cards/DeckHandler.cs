using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class DeckHandler : MonoBehaviour
{
    public GameObject ifEmptyDeck;
    public GameObject canvas;
    public GameObject cardPrefab;
    public List<GameObject> originalDeck = new List<GameObject>();
    public List<GameObject> hand = new List<GameObject>();
    public List<GameObject> CreatedCards = new List<GameObject>();

    private void Start()
    {
        canvas = gameObject;
    }

    public void ShowDeck(BattlePlayer player)
    {
        Debug.Log("Now ONto here");
        if (originalDeck.Count <= 0) { InitializeDeck(player);  }
        shuffleDeck(CreatedCards);
        IEnumerable<GameObject> cut = CreatedCards.Take(3);
        hand = cut.ToList();
        int position = (hand.Count - 1) * 50;
        Debug.Log(position);

        foreach (GameObject card in hand) 
        {
            Debug.Log(position);
            RectTransform rectTransform = card.GetComponent<RectTransform>();
            rectTransform.anchorMax.Set(0.5f, 0);
            rectTransform.anchorMax.Set(0.5f, 0);
            rectTransform.anchoredPosition = new Vector2(position, 0);
            card.transform.localScale = Vector3.one;
            
            position -= 100;
            card.SetActive(true);

        }

    }

    public void InitializeDeck(BattlePlayer player)
    {
        Debug.Log("Made it here");
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

            addCard.GetComponent<Card>().Player = player;
            addCard.GetComponent<Card>().CardDataHolder = card.GetComponent<CardData>();
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
        foreach (GameObject card in hand)
        {
            card.SetActive(false);
        }
    }

}
