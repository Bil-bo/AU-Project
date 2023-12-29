using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{

    public static ActionManager Instance;

    private readonly LinkedList<ICardActions> ActionQueue = new LinkedList<ICardActions>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
    }

    private void Update()
    {
        while (ActionQueue.Count > 0) 
        {
            ICardActions currentAction = ActionQueue.First.Value;
            ActionQueue.RemoveFirst();
            currentAction.Effect();
        }
    }

    public void AddToTop(ICardActions action)
    {
        ActionQueue.AddFirst(action);
    }

    public void AddToBottom(ICardActions action) 
    {
        ActionQueue.AddLast(action);
    }


}
