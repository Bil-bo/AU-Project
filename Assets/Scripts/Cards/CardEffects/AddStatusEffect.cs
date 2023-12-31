using System.Collections.Generic;
using UnityEngine;

public class AddStatusEffect : ICardActions
{
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
                CharacterID = target.CharID,
                Target = target,
                Name = Status.Name,
                Counter = Status.Counter,

            };

            EventManager.Broadcast(gameEvent);
            if (!gameEvent.IsMerged.IsTrue)
            {
                GameObject statusHolder = target.transform.Find("StatusHolder(Clone)").gameObject;
                GameObject newStatusEffect = StatusEffectSprites.Instance.CreateStatusVisual(statusHolder.transform);
                statusHolder.GetComponent<StatusSorter>().OrganiseStatuses();
                newStatusEffect.GetComponent<StatusVisualiser>().Effect = Status;
                Status.Target = target;
            }
        }


    }
}
