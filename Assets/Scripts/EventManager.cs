using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{

    private static readonly Dictionary<IBroadCastEvent, Dictionary<Type, Action<IEvent>>> s_Events = new();

    static readonly Dictionary<Delegate, Action<IEvent>> s_EventLookups = new();


    public static void AddListener<T>(Action<T> evt, IBroadCastEvent source) where T : IEvent
    {
        if (!s_EventLookups.ContainsKey(evt))
        {
            Action<IEvent> newAction = (e) => evt((T)e);
            s_EventLookups[evt] = newAction;
            if (s_Events.TryGetValue(source,
                out Dictionary<Type, Action<IEvent>> eventDictionary))
            {
                if (eventDictionary.TryGetValue(typeof(T), out Action<IEvent> internalAction))
                    eventDictionary[typeof(T)] = internalAction += newAction;
                else
                    eventDictionary[typeof(T)] = newAction;
            }

            else
            {
                s_Events[source] = new Dictionary<Type, Action<IEvent>>
                {
                    { typeof(T), newAction }
                };
            }
        }
    }

    public static void RemoveListener<T>(Action<T> evt, IBroadCastEvent source) where T : IEvent
    {
        if (s_EventLookups.TryGetValue(evt, out var action))
        {
            if (s_Events.TryGetValue(source, out var eventDictionary))
            {
                if (eventDictionary.TryGetValue(typeof(T), out var tempAction))
                {
                    tempAction -= action;
                    if (tempAction == null)
                    {
                        eventDictionary.Remove(typeof(T));
                        if (eventDictionary.Count == 0)
                        {
                            s_Events.Remove(source);
                        }
                    }
                    else
                        eventDictionary[typeof(T)] = tempAction;
                }
            }

            s_EventLookups.Remove(evt);
        }
    }
    public static void Broadcast(IEvent evt, IBroadCastEvent source)
    {
        if (!s_Events.ContainsKey(source))
        {
            s_Events[source] = new Dictionary<Type, Action<IEvent>>();
        }
        else
        { 
            if (s_Events[source].TryGetValue(evt.GetType(), out var action))
                action.Invoke(evt);
        }
    }

    public static void Clear()
    {
        s_Events.Clear();
        s_EventLookups.Clear();
    }
}

