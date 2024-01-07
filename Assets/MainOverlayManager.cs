using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainOverlayManager : MonoBehaviour, IOnTreasureCollected
{
    [SerializeField]
    private RewardPanel panel;

    private void Start()
    {
        EventManager.AddListener<TreasureCollectedEvent>(OnTreasureCollected);
        
    }


    public void OnTreasureCollected(TreasureCollectedEvent eventData) 
    {
        StartCoroutine(ShowRewardsScreen(eventData));
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

    private void OnDestroy()
    {
        EventManager.RemoveListener<TreasureCollectedEvent>(OnTreasureCollected);
    }
}
