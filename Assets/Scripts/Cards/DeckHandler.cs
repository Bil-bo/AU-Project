using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// DeckHandler Attached to each Player to show and manage their cards
public class DeckHandler : MonoBehaviour, IOnAttackChanged, IOnPlayerDeath
{
    // Prefab for playtesting when loading directly into the battle scene
    public GameObject ifEmptyDeck;
    public GameObject canvas; // Unnecessary but I'm scared to remove
    public GameObject cardPrefab;
    public RectTransform CardDimensions;
    public BattlePlayer Player;
    private Button button;

    [SerializeField]
    private TextMeshProUGUI PlayerEnergy;

    private EventSystem eventSystem;

    private GameObject _currentPlayer;
    public GameObject currentPlayer { 
        get { return _currentPlayer; }
        set 
        {
            _currentPlayer = value;
            if (Player != null) { Player.OnEnergyChanged += (e, m) => PlayerEnergy.text = e.ToString() + "/" + m.ToString(); }
            Player = _currentPlayer.GetComponent<BattlePlayer>();
            Player.OnEnergyChanged += (e,m) => PlayerEnergy.text = e.ToString()+"/"+m.ToString();
            button.onClick.AddListener(Player.FinishTurn);
        } 
    }

    // Original Deck - The player's current deck - not to be altered here only to create cards from
    // hand - cards to be shown in the battle scene
    // Created Cards - Instantiated cards that can be drawn from
    // Combined cards - Holds cards that have been combined from: unimplemented - restoring combined cards if a respective combined card is used
    public List<GameObject> hand = new List<GameObject>();
    GraphicRaycaster raycaster;


    public Dictionary<GameObject, List<GameObject>> partyDecks = new Dictionary<GameObject, List<GameObject>>();
    public Dictionary<GameObject, List<GameObject>> InDeck = new Dictionary<GameObject, List<GameObject>>();
    public Dictionary<GameObject, Dictionary<Guid, GameObject[]>> CombinedCards = new Dictionary<GameObject, Dictionary<Guid, GameObject[]>>();


    // Scared to remove
    private void Start()
    {
        canvas = gameObject;
        raycaster = gameObject.GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();
        button = GetComponentInChildren<Button>();
        EventManager.AddListener<AttackChangedEvent>(OnAttackChanged);
        EventManager.AddListener<PlayerDeathEvent>(OnPlayerDeath);
    }

    public void ShowDeck()
    {
        if (!InDeck.ContainsKey(currentPlayer)) { InitialiseDeck(); }

        ShuffleDeck(InDeck[currentPlayer]);


        for (int i = 0; i < Player.MaxHand && i < InDeck[currentPlayer].Count; i++)
        {
            GameObject proxy = InDeck[currentPlayer][i];
            hand.Add(proxy);
            UpdateDamage(proxy.GetComponentInChildren<Card>(), Player.Attack);
            InDeck[currentPlayer].Remove(proxy);
        }

        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        UpdateDeck();


    }

    public void OnAttackChanged(AttackChangedEvent evt)
    {
        if (evt.ChangedPlayer == Player) 
        {
            foreach (GameObject card in hand)
            {
                card.GetComponentInChildren<Card>().DamageModifier = evt.NewAtk;
            }
        }
    }

    private void UpdateDamage(Card card, int damageChange)
    {
        card.DamageModifier = damageChange;

    }


    // TODO: Rename this or above
    // Puts all cards in hand back into createdCards, and hides the end turn button
    public void HideDeck()
    {
        int counter = hand.Count;
        for (int i = 0; i < counter; i++)
        {
            GameObject proxy = hand[0];
            InDeck[currentPlayer].Add(proxy);
            hand.Remove(proxy);
            UpdateDamage(proxy.GetComponentInChildren<Card>(), 0);
            proxy.SetActive(false);
        }
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void InitialiseDeck()
    {
        if (partyDecks[currentPlayer].Count == 0) { partyDecks[currentPlayer] = CreateDefaultList(partyDecks[currentPlayer]); }
        else
        {
            InDeck[currentPlayer] = partyDecks[currentPlayer];
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
    public static List<GameObject> ShuffleDeck(List<GameObject> cards)
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


    public bool TryMergeCards(Card cardOne, Card cardTwo)
    {
        bool merged = false;
        GameObject canMerge = cardOne.CanMerge(cardTwo);
        if (canMerge != null)
        {
            merged = true;
            if (!CombinedCards.ContainsKey(currentPlayer)) { CombinedCards.Add(currentPlayer, new Dictionary<Guid, GameObject[]>()); }
            GameObject newCard = CardFactory.CreateCard(canMerge, this.transform, Player.CharID, true);
            hand.Add(newCard);


            CombinedCards[currentPlayer].Add(newCard.GetComponentInChildren<Card>().CardID, new GameObject[] { cardOne.CardBase.gameObject, cardTwo.CardBase.gameObject});
            hand.Remove(cardOne.CardBase.gameObject);
            hand.Remove(cardTwo.CardBase.gameObject);

            cardOne.CardBase.gameObject.SetActive(false);
            cardTwo.CardBase.gameObject.SetActive(false);
            UpdateDeck();
        }
        return merged;
    }

    private bool TryUnmergeCards (Card Merged)
    {
        bool UnMerged = false;
        GameObject[] mergedCards = new GameObject[2];
        if (CombinedCards.ContainsKey(currentPlayer) && CombinedCards[currentPlayer].TryGetValue(Merged.CardID, out mergedCards))
        {
            UnMerged = true;
            foreach (var card in mergedCards)
            {
                if(!TryUnmergeCards(card.GetComponentInChildren<Card>()))
                {
                    InDeck[currentPlayer].Add(card);
                }
                
            }

            Destroy(Merged.CardBase.gameObject);
        }

        return UnMerged;
           
    }

    public void UseCard(GameObject card, Card data)
    {
        GameObject proxy = card;
        hand.Remove(proxy);

        if (!TryUnmergeCards(data))
        {
            InDeck[currentPlayer].Add(proxy);
            proxy.SetActive(false);
        }

        UpdateDeck();


    }

    // Fans the cards out from the middle of the screen outwards
    private void UpdateDeck()
    {
        int position = Mathf.RoundToInt((hand.Count - 1) * (CardDimensions.rect.width / 2.0f));


        foreach (GameObject card in hand)
        {

            RectTransform rectTransform = card.GetComponent<RectTransform>();
            rectTransform.anchorMax.Set(0.5f, 0);
            rectTransform.anchorMax.Set(0.5f, 0);
            rectTransform.anchoredPosition = new Vector2(position, 0);
            card.transform.localScale = Vector3.one;

            position -= Mathf.RoundToInt(rectTransform.rect.width);
            card.SetActive(true);

        }
    }

    public void AddDeck(GameObject player)
    {
        partyDecks[player] = GameData.Instance.BattlePlayers[player];
        if (CardDimensions == null) { CardDimensions = partyDecks[player][0].GetComponent<RectTransform>(); }

        foreach (GameObject card in partyDecks[player])
        {
            card.transform.SetParent(transform, false);
        }
    }

    public bool SelectCard(Vector2 mousePos, out GameObject CardToReturn)
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = mousePos;
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);
        // Checks if the Raycast hit another card
        foreach (RaycastResult result in results)
        {
            Debug.Log(result.gameObject.name);
            if (result.gameObject.GetComponentInChildren<Card>() != null)
            {
                CardToReturn = result.gameObject;
                return true;
            }
        }
        CardToReturn = null;
        return false;
    }

    public void ResetCards()
    {
        foreach (GameObject player in partyDecks.Keys.ToList())
        {
            GameData.Instance.BattlePlayers[player] = partyDecks[player];
            foreach (GameObject card in partyDecks[player])
            {
                card.SetActive(false);
                card.transform.SetParent(GameData.Instance.transform, false);
            }
        }
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<AttackChangedEvent>(OnAttackChanged);
        EventManager.RemoveListener<PlayerDeathEvent>(OnPlayerDeath);
    }

    public void OnPlayerDeath(PlayerDeathEvent eventData)
    {
        GameObject DeadPlayer = eventData.player.gameObject;

        InDeck.Remove(DeadPlayer);
        CombinedCards.Remove(DeadPlayer);
        if (Player == eventData.player)
        {
            currentPlayer = null;
            hand.Clear();
        }

        foreach (GameObject card in partyDecks[DeadPlayer]) { Destroy(card); }
        partyDecks.Remove(DeadPlayer);
        GameData.Instance.BattlePlayers.Remove(DeadPlayer);

    }
}