using System.Collections.Generic;
using UnityEngine;

public class AddStatusEffect : ICardActions
{
    [SerializeField]
    private GameObject StatusEffectPrefab;

    private BaseBattleCharacter User;
    private List<BaseBattleCharacter> Target;
    private StatusEffect Status;

    public AddStatusEffect(BaseBattleCharacter user, List<BaseBattleCharacter> targets, StatusEffect effect)
    {
        this.User = user;
        this.Target = targets;
        this.Status = effect;
    }


    public void Effect()
    {
        foreach (BaseBattleCharacter target in Target)
        {
            StatusEffectAddedEvent gameEvent = new StatusEffectAddedEvent()
            {
                Name = Status.Name,
                Counter = Status.Counter,

            };

            EventManager.Broadcast(gameEvent, target);
            if (!gameEvent.IsMerged.IsTrue)
            {
                GameObject newStatusEffect = Object.Instantiate(StatusEffectPrefab, target.gameObject.transform);
                newStatusEffect.GetComponent<StatusVisualiser>().Effect = Status;
            }
        }


    }
}
