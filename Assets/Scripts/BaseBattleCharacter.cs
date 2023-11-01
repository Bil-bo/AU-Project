using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBattleCharacter : MonoBehaviour
{

    private int maxHealth;
    private int currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public abstract void Attack();

    public abstract void Defend();
}
