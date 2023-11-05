using System;
using System.Threading;
using Unity.VisualScripting;

// For Holding StatusEffects that can effect the game characters
// Also no monobehaviour: no need for inspector and doesn't have to be attached to a GameObject
public abstract class StatusEffect
{
    // Timer for how long an affect lasts for
    // Name for ease of storage
    public int timer { get; set; }
    public string name;

    public StatusEffect(int timer)
    {
        this.timer = timer;
    }

    // Counts down - Timer value returned for ease of storage arguments
    public virtual int countDown (BaseBattleCharacter affected)
    {
        timer -= 1;
        return timer;
    }

    // Combine adds two of the same statusEffect together, so that they can stack
    public virtual void Combine(StatusEffect extra)
    {
        timer += extra.timer;

    }

}

// Simple Status Effect that deals damage every turn 
public class Poison : StatusEffect
{
    public Poison(int timer) : base(timer) 
    {
        name = "Poison";
    }

    // Deals damage relative to the timer each turn. Designed to get stronger the more poison you can stack
    // Based off of Slay the Spire / WildFrost Poison
    public override int countDown(BaseBattleCharacter affected)
    {
        affected.TakeDamage(timer);
        timer -= 1;
        return timer;

    }
}

// Simple - Take reduced damage for a turn then go back to normal
public class Block : StatusEffect
{
    public Block(int timer) : base(timer)
    {
        name = "Block";
    }
}


// Factory (maybe but probably not) for more Dynamic creation of status effects
public class StatusFactory
{
    public static StatusFactory Instance = new StatusFactory();

    public T createStatus<T>(int timer) where T : StatusEffect
    {
        return Activator.CreateInstance(typeof(T), timer) as T;
    }
}
