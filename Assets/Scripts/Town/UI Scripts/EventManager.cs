using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
	private static EventManager instance;
	public static EventManager Instance => instance;

	private static Dictionary<string, Delegate> events = new Dictionary<string, Delegate>();

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
	}

	public static void Subscribe<T>(string eventName, Action<T> listener)
	{
		if(events.ContainsKey(eventName))
		{
			events[eventName] = Delegate.Combine(events[eventName], listener);
		}
		else
		{
			events[eventName] = listener;
		}
	}
	public static void Subscribe(string eventName, Action listener)
	{
		if (events.ContainsKey(eventName))
		{
			events[eventName] = Delegate.Combine(events[eventName], listener);
		}
		else
		{
			events[eventName] = listener;
		}
	}

	public static void Unsubscribe<T>(string eventName, Action<T> listener)
	{
		if(events.ContainsKey(eventName))
		{
			var currentDelegate = Delegate.Remove(events[eventName], listener);
			if(currentDelegate == null)
			{
				events.Remove(eventName);
			}
			else
			{
				events[eventName] = currentDelegate;
			}
		}
	}
	public static void Unsubscribe(string eventName, Action listener)
	{
		if (events.ContainsKey(eventName))
		{
			var currentDelegate = Delegate.Remove(events[eventName], listener);
			if (currentDelegate == null)
			{
				events.Remove(eventName);
			}
			else
			{
				events[eventName] = currentDelegate;
			}
		}
	}

	public static void Trigger<T>(string eventName, T arg)
	{
		if(events.ContainsKey(eventName) && events[eventName] is Action<T> action)
		{
			action.Invoke(arg);
		}
	}
	public static void Trigger(string eventName)
	{
		if (events.ContainsKey(eventName) && events[eventName] is Action action)
		{
			action.Invoke();
		}
	}
}
