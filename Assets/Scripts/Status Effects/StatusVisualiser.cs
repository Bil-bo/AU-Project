using TMPro;
using UnityEngine;


// Game object that holds the status effect behaviour
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
            if (_Effect != null) { Effect.CounterChange -= e => UpdateText(e); }
            _Effect = value;
            StatusRenderer.material = StatusEffectSprites.Instance.GetSprite(value.Name);
            StatusCounter.text = value.Counter.ToString();
            value.CounterChange += e => UpdateText(e);
            value.Initialise();
        }
    }

    // Delete everything once the counter hits 0
    private void UpdateText(int c)
    {
        if (c == 0)
        {
            Destroy(gameObject);
        }
        else
        {
            StatusCounter.text = c.ToString();  
        }

    }
    private void OnDestroy()
    {
        Effect?.Remove();
    }
}
