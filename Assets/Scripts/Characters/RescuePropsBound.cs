using UnityEngine;


// For supports instantiated in the battle scene
// Add to party on rescue
public class RescuePropsBound : MonoBehaviour
{
    public string SpawnerID { get; set; }

    [SerializeField]
    public BattleInfo rescuedInfo;

 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerRoaming"))
        {
            GameData.Instance.AddPlayer(rescuedInfo);
            PlayerRescuedEvent p = new PlayerRescuedEvent()
            {
                SpawnerID = SpawnerID,
            };

            EventManager.Broadcast(p);

            gameObject.SetActive(false);
        }
    }


}
