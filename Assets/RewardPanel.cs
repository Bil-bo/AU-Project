using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RewardPanel : MonoBehaviour
{

    [SerializeField]
    private List<Toggle> CardButtons = new List<Toggle>();

    [SerializeField]
    private GameObject SelectPlayerText;

    [SerializeField]
    private List<Button> PlayerButton = new List<Button>();

    private List<GameObject> CardRewards = new List<GameObject>();

    private List<GameObject> SelectedCards = new();

    private int ActivePlayers = 0;

    private int RewardsToChoose;

    public IEnumerator ShowRewards(List<GameObject> rewards, List<BattlePlayer> players)
    {
        gameObject.SetActive(true);
        SelectPlayerText.SetActive(false);
        foreach (Button b in PlayerButton) { b.gameObject.SetActive(false); }
        foreach (Toggle t in CardButtons) { t.gameObject.SetActive(false); }


        for (int i = 0; i < rewards.Count; i++)
        {
            GameObject card = CardFactory.CreateRewardCard(rewards[i], CardButtons[i].transform, true);
            Debug.Log(card.name);    
            CardRewards.Add(card);
            RectTransform rectTransform = card.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one / 2f;
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector3.zero;

            CardButtons[i].gameObject.SetActive(true);
            CardButtons[i].targetGraphic = card.GetComponent<Image>();
            CardButtons[i].onValueChanged.AddListener((isOn) => SelectButton(isOn, card));
        }

        foreach (BattlePlayer player in players)
        {
            Debug.Log(player.Name);
            PlayerButton[ActivePlayers].transform.Find("PT").GetComponent<TextMeshProUGUI>().text = player.Name;
            PlayerButton[ActivePlayers].onClick.AddListener(() => AddToDeck(player.gameObject));
            ActivePlayers++;
        }
        yield return StartCoroutine(SelectRewards());

    }

    private void SelectButton(bool isOn, GameObject card)
    {
        if (isOn && !SelectedCards.Contains(card))
        {
            if (SelectedCards.Count == 0) { ShowPlayerSelect(true); }
            SelectedCards.Add(card); 
        } 

        else if (!isOn && SelectedCards.Contains(card)) 
        {
            SelectedCards.Remove(card);
            if (SelectedCards.Count == 0) { ShowPlayerSelect(false); }
        }
    }


    private IEnumerator SelectRewards()
    {
        RewardsToChoose = CardRewards.Count;
        while (RewardsToChoose > 0)
        {
            yield return null;
        }
    }

    private void ShowPlayerSelect(bool active) 
    {
        for (int i = 0; i < ActivePlayers; i++)
        {
            PlayerButton[i].gameObject.SetActive(active);
        }       
    }

    private void AddToDeck(GameObject player) 
    {
        GameData.Instance.BattlePlayers[player].AddRange(SelectedCards);
        RewardsToChoose -= SelectedCards.Count;
        foreach (Toggle t in CardButtons) 
        {
            if (t.isOn) { t.gameObject.SetActive(false); }
        }
    }


}
