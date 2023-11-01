public class StatusEffect
{
    public string Name { get; set; }
    public int Duration { get; set; }
    public EffectType EffectType { get; set; }
    public float EffectValue { get; set; }
}

public enum EffectType
{
    IncreaseDamage,    // Boost damage output
    ReduceDamage,      // Reduce damage output
    IncreaseDefense,   // Reduce damage taken
    ReduceDefense     // Boost damage taken
}
