using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusVisualiser : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro StatusCounter;

    [SerializeField]
    private Renderer StatusRenderer;

    private StatusEffect _Effect;

    public StatusEffect Effect 
    {  
        get { return _Effect; } 
        set 
        {
            if (_Effect != null) { Effect.CounterChange -= e => StatusCounter.text = e.ToString(); }
            _Effect = value;
            StatusRenderer.material = StatusEffectSprites.Instance.GetSprite(value.Name);
            StatusCounter.text = value.Counter.ToString();
            value.CounterChange += e => StatusCounter.text = e.ToString(); ;
            value.Initialise();


        }
    }
    private void OnDestroy()
    {
        Effect?.Remove();
    }
}
