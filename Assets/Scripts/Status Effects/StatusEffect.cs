using System;
using System.Threading;
using Unity.VisualScripting;

public abstract class StatusEffect
{
    public int timer { get; set; }
    public string name;

    public StatusEffect(int timer)
    {
        this.timer = timer;
    }

    public virtual int countDown (BaseBattleCharacter affected)
    {
        timer -= 1;
        return timer;
    }

    public virtual void Combine(StatusEffect extra)
    {
        timer += extra.timer;

    }

}

public class Poison : StatusEffect
{
    public Poison(int timer) : base(timer) 
    {
        name = "Poison";
    }

    public override int countDown(BaseBattleCharacter affected)
    {
        affected.TakeDamage(timer);
        timer -= 1;
        return timer;

    }
}

public class Block : StatusEffect
{
    public Block(int timer) : base(timer)
    {
        name= "Block";
    }
}



public class StatusFactory
{
    public static StatusFactory Instance = new StatusFactory();

    public T createStatus<T>(int timer) where T : StatusEffect
    {
        return Activator.CreateInstance(typeof(T), timer) as T;
    }
}
