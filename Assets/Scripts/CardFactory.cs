using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    public static GameObject CreateCard(GameObject card, Transform parent, bool isActive = false)
    {
        GameObject newCard = Instantiate(instance.CardPrefab, parent);
        Instantiate(card, newCard.transform);
        newCard.SetActive(isActive);
        return null;
    }

    public static List<GameObject> CreateCards(List<GameObject> Cards, Transform parent, bool isActive = false)
    {
        List<GameObject> cards = new List<GameObject>();
        foreach (GameObject card in Cards)
        {
            GameObject newCard = Instantiate(instance.CardPrefab, parent);
            Instantiate(card, newCard.transform);
            newCard.SetActive(isActive);
            cards.Add(newCard);
        }
        return cards;
    }

}
