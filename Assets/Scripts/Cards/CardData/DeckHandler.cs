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
    public GameObject cardPrefab; // Deprecated
    public RectTransform CardDimensions; // For deciding where cards go
    public BattlePlayer Player; // The script of the current player
    private Button button;

    [SerializeField]
    private TextMeshProUGUI PlayerEnergy;

    private EventSystem eventSystem; // For raycasting

    private GameObject _currentPlayer;

    // Property to set the GameObject and the BattlePlayer at the same time
    public GameObject currentPlayer { 
        get { return _currentPlayer; }
        set 
        {
            if (Player != null)
            {
                button.onClick.RemoveListener(Player.FinishTurn);
                Player.OnEnergyChanged = (e, m) => PlayerEnergy.text = e.ToString() + "/" + m.ToString(); 
            }
            _currentPlayer = value;

            if (value != null)
            {
                Player = _currentPlayer.GetComponent<BattlePlayer>();
                Player.OnEnergyChanged += (e, m) => PlayerEnergy.text = e.ToString() + "/" + m.ToString();
                button.onClick.AddListener(Player.FinishTurn);
            }
        } 
    }

    // Party Deck - The player's current deck - not to be altered here only to create cards from
    // hand - cards to be shown in the battle scene
    // InDecK - Instantiated cards that can be drawn from
    // Combined cards - Holds cards that have been combined from
    public List<GameObject> hand = new List<GameObject>();
    GraphicRaycaster raycaster;


    public Dictionary<GameObject, List<GameObject>> partyDecks = new Dictionary<GameObject, List<GameObject>>();
    public Dictionary<GameObject, List<GameObject>> InDeck = new Dictionary<GameObject, List<GameObject>>();
    public Dictionary<GameObject, Dictionary<Guid, GameObject[]>> CombinedCards = new Dictionary<GameObject, Dictionary<Guid, GameObject[]>>();

    // Setup
    private void Start()
    {
        canvas = gameObject;
        raycaster = gameObject.GetComponent<GraphicRaycaster>();
        eventSystem = GetComponent<EventSystem>();
        button = GetComponentInChildren<Button>();
        EventManager.AddListener<AttackChangedEvent>(OnAttackChanged);
        EventManager.AddListener<PlayerDeathEvent>(OnPlayerDeath);
    }

    // Show the current players deck
    public void ShowDeck()
    {
        if (!InDeck.ContainsKey(currentPlayer)) { InitialiseDeck(); }

        ShuffleDeck(InDeck[currentPlayer]);

        // Show as many cards as player will allow
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

    // Update the cards in the hand on dynamic change
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

    // update the modifier at turn start
    private void UpdateDamage(Card card, int damageChange)
    {
        card.DamageModifier = damageChange;

    }


    // Puts all cards in hand back into createdCards
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

    // Sets up the initial deck
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

    // Attempts to merge two cards together, stores them away if possible
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

    // Recursively splits cards back up into their components
    // A merged card will always split back into its most unmergd form
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

    // Hides a used card
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
            rectTransform.localScale = Vector3.one;
            rectTransform.anchorMin = new Vector2(0.5f, 0);
            rectTransform.anchorMax = new Vector2(0.5f, 0);
            rectTransform.pivot = new Vector2(0.5f, 0);
            rectTransform.anchoredPosition = new Vector3(position, 0, 0);


            position -= Mathf.RoundToInt(rectTransform.rect.width);
            card.SetActive(true);

        }
    }

    // Adds a players hand to the deck
    // Also gets dimensions of first player added
    public void AddDeck(GameObject player)
    {
        partyDecks[player] = GameData.Instance.BattlePlayers[player];
        if (CardDimensions == null) { CardDimensions = partyDecks[player][0].GetComponent<RectTransform>(); }

        foreach (GameObject card in partyDecks[player])
        {
            card.transform.SetParent(transform, false);
        }
    }

    // Graphics raycasting, finding a card on the UI
    public bool SelectCard(Vector2 mousePos, out GameObject CardToReturn)
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = mousePos;
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);
        // Checks if the Raycast hit another card
        foreach (RaycastResult result in results)
        {

            if (result.gameObject.GetComponentInChildren<Card>() != null)
            {
                CardToReturn = result.gameObject;
                return true;
            }
        }
        CardToReturn = null;
        return false;
    }

    // Moves all cards back so they don't get destroyed
    public void ResetCards()
    {
        foreach (GameObject player in partyDecks.Keys.ToList())
        {
            foreach (GameObject card in partyDecks[player])
            {
                card.SetActive(false);
                card.transform.SetParent(GameData.Instance.transform, false);
            }
        }
    }


    // Listener handling

    private void OnDestroy()
    {
        EventManager.RemoveListener<AttackChangedEvent>(OnAttackChanged);
        EventManager.RemoveListener<PlayerDeathEvent>(OnPlayerDeath);
    }

    // Deletes player hand
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

        foreach (GameObject card in InDeck[DeadPlayer]) { card.SetActive(false); }
        partyDecks.Remove(DeadPlayer);
        GameData.Instance.BattlePlayers.Remove(DeadPlayer);

    }
}