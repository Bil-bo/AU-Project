using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainOverlayManager : MonoBehaviour, IOnTreasureCollected, IOnPickUpCollected, IOnFinalBossDefeated
{
    [SerializeField]
    private RewardPanel panel;

    [SerializeField]
    private TextMeshProUGUI PickUpText;

    [SerializeField]
    private TextMeshProUGUI WinText;

    private int PickUpscollected = 0;

    private void Start()
    {
        EventManager.AddListener<TreasureCollectedEvent>(OnTreasureCollected);
        EventManager.AddListener<PickupCollectedEvent>(OnPickUpCollected);
        EventManager.AddListener<FinalBossDefeatedEvent>(OnFinalBossDefeated);
        
        if (PlayerPrefs.HasKey("PickUpsCollected"))
        {
            PickUpscollected = PlayerPrefs.GetInt("PickUpsCollected");
            PickUpText.text = "PickUps Collected: " + PickUpscollected;
        }
        else 
        {
            PlayerPrefs.SetInt("PickUpsCollected", 0);
            PickUpText.text = "PickUps Collected: " + PickUpscollected;
        }

    }


    public void OnTreasureCollected(TreasureCollectedEvent eventData) 
    {
        StartCoroutine(ShowRewardsScreen(eventData));
    }

    public void OnPickUpCollected(PickupCollectedEvent eventData)
    {
        PickUpscollected++;
        PlayerPrefs.SetInt("PickUpsCollected", 0);
        PickUpText.text = "PickUps Collected: " + PickUpscollected;
    }

    private IEnumerator ShowRewardsScreen(TreasureCollectedEvent eventData)
    {
        eventData.Collider.GetComponent<PlayerInput>().enabled = false;
       
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        CameraController controller = Camera.main.GetComponent<CameraController>();

        controller.enabled = false;
       
        List<BattlePlayer> players = new List<BattlePlayer>();
        GameData.Instance.BattlePlayers.Keys.ToList().ForEach(key => { players.Add(key.GetComponent<BattlePlayer>()); });

        yield return StartCoroutine(panel.ShowRewards(eventData.Treasure, players));

        eventData.Collider.GetComponent<PlayerInput>().enabled = true;
        
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;

        controller.enabled = true;

    }

    public void OnFinalBossDefeated(FinalBossDefeatedEvent eventData)
    {
        StartCoroutine(Congrats());
    }

    private IEnumerator Congrats()
    {
        WinText.gameObject.SetActive(true);
        yield return new WaitForSeconds(8);
        SceneManager.LoadScene("Menu");
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<TreasureCollectedEvent>(OnTreasureCollected);
        EventManager.RemoveListener<PickupCollectedEvent>(OnPickUpCollected);
        EventManager.RemoveListener<FinalBossDefeatedEvent>(OnFinalBossDefeated);

    }


}
