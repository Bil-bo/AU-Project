using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// Simple Factory for instantiating cards
// Static to be used anywhere in the code
// Singleton for only on eto exist
[Serializable]
public class CardFactory : MonoBehaviour
{
    public GameObject CardPrefab;

    private static CardFactory instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static GameObject CreateCard(GameObject card, Transform parent, Guid parentId, bool isActive = false)
    {
        GameObject newCard = Instantiate(instance.CardPrefab, parent);
        Instantiate(card, newCard.transform);
        card.GetComponent<Card>().UserID = parentId;
        newCard.SetActive(isActive);
        return newCard;
    }

    public static List<GameObject> CreateCards(List<GameObject> Cards, Transform parent, Guid parentId, bool isActive = false)
    {
        List<GameObject> cards = new List<GameObject>();
        foreach (GameObject card in Cards)
        {
            GameObject newCard = Instantiate(instance.CardPrefab, parent);
            Instantiate(card, newCard.transform);
            card.GetComponent<Card>().UserID = parentId;
            newCard.SetActive(isActive);
            cards.Add(newCard);
        }
        return cards;
    }

    // Card without initial parent
    public static GameObject CreateRewardCard(GameObject card, Transform parent, bool isActive = false)
    {
        GameObject newCard = Instantiate(instance.CardPrefab, parent);
        Instantiate(card, newCard.transform);

        newCard.SetActive(isActive);
        return newCard;
    }

}
