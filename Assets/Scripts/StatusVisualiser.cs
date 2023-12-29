using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusVisualiser : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro StatusCounter;

    private Renderer StatusRenderer;

    private StatusEffect _Effect;

    public StatusEffect Effect 
    {  
        get { return _Effect; } 
        set 
        {
            if (_Effect != null) { Effect.CounterChange -= UpdateText; }
            _Effect = value;
            StatusRenderer.material = StatusEffectSprites.Instance.GetSprite(value.Name);
            StatusCounter.text = value.Counter.ToString();
            value.CounterChange += UpdateText;


        }
    }


    void Start()
    {
        StatusRenderer = GetComponentInChildren<Renderer>();
    }

    void UpdateText(int value)
    {
        StatusCounter.text += value.ToString();
    } 
}
