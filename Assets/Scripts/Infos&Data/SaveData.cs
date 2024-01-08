using UnityEngine;

[System.Serializable]
public class SaveData
{
    public bool[] enemyEnabled;
    public bool[] pickupEnabled;
    public Vector3 playerPosition;
    public int pickups;
    public string difficulty;

}
