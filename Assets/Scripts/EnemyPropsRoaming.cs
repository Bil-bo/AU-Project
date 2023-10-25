using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProperties : MonoBehaviour

{
    public List<int> enemyCount = new List<int>();   

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            enemyCount.Add(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int getItem() { return enemyCount[5]; }
}
