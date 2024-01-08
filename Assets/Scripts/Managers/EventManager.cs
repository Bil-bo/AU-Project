using System;
using System.Collections.Generic;


// based on unity's FPS microgame
// Event manager used as hub for notifying objects of events without creating unecessary dependencies
public static class EventManager
{
    static readonly Dictionary<Type, Action<IEvent>> s_Events = new Dictionary<Type, Action<IEvent>>();

    static readonly Dictionary<Delegate, Action<IEvent>> s_EventLookups =
        new Dictionary<Delegate, Action<IEvent>>();

    public static void AddListener<T>(Action<T> evt) where T : IEvent
    {

        if (!s_EventLookups.ContainsKey(evt))
        {
            Action<IEvent> newAction = (e) => evt((T)e);
            s_EventLookups[evt] = newAction;

            if (s_Events.TryGetValue(typeof(T), out Action<IEvent> internalAction))
                s_Events[typeof(T)] = internalAction += newAction;
            else
                s_Events[typeof(T)] = newAction;
        }

    }

    public static void RemoveListener<T>(Action<T> evt) where T : IEvent
    {
        if (s_EventLookups.TryGetValue(evt, out var action))
        {
            if (s_Events.TryGetValue(typeof(T), out var tempAction))
            {
                tempAction -= action;
                if (tempAction == null)
                    s_Events.Remove(typeof(T));
                else
                    s_Events[typeof(T)] = tempAction;
            }

            s_EventLookups.Remove(evt);
        }
    }

    public static void Broadcast(IEvent evt)
    {
        if (s_Events.TryGetValue(evt.GetType(), out var action))
        {
            action.Invoke(evt);
        }
    }

    public static void Clear()
    {
        s_Events.Clear();
        s_EventLookups.Clear();
    }
}


