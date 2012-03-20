﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;

using System.Linq;

using CryEngine.Extensions;

namespace CryEngine
{
	public class UI
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		extern internal static int _RegisterEvent(string eventsystem, int direction, UIEventDescription desc);
		[MethodImpl(MethodImplOptions.InternalCall)]
		extern internal static bool _RegisterToEventSystem(string eventsystem, int type);
		[MethodImpl(MethodImplOptions.InternalCall)]
		extern internal static void _UnregisterFromEventSystem(string eventsystem, int type);
		[MethodImpl(MethodImplOptions.InternalCall)]
		extern internal static void _SendEvent(string eventsystem, int Event, object[] args);
		[MethodImpl(MethodImplOptions.InternalCall)]
		extern internal static void _SendNamedEvent(string eventsystem, string Event, object[] args);

		public static void OnEvent(string EventSystem, string EventName, int EventID, object[] args)
		{
			UIEventArgs e = new UIEventArgs(EventSystem, EventName, EventID, args);
			Debug.LogAlways("Event: {0}.{1} = {2}", EventSystem, EventName, e.EventID);
			int i, c;
			Object o;
			c = e.Args.Length;
			for(i = 0; i < c; i++)
			{
				o = e.Args[i];
				Debug.LogAlways("Arg {0}/{1}: {2} {3}", i + 1, c, o.GetType().Name, o);
			}
			//SendEvent("MySystemEvent", "TestEvent2", new object[2] { EventName, EventID });
			if(Events != null)
				Events(null, e);
		}

		public static void OnInit()
		{
		}

		public static void OnShutdown()
		{
		}

		public static void OnReload()
		{
		}

		public static void OnReset()
		{
		}

		public static void OnUpdate(float delta)
		{
		}

		/// <summary>
		/// Registers an event to the system
		/// </summary>
		/// <param name="eventsystem">The name of the eventsystem this is registered to</param>
		/// <param name="direction">Event direction (system -> UI or UI -> system)</param>
		/// <param name="desc">Descriptor of the event</param>
		/// <returns>-1 if registration failed, event ID otherwise</returns>
		internal static int RegisterEvent(string eventsystem, UIEventDirection direction, UIEventDescription desc)
		{
			return _RegisterEvent(eventsystem, (int)direction, desc);
		}

		internal static bool RegisterToEventSystem(string eventsystem, UIEventDirection direction)
		{
			return _RegisterToEventSystem(eventsystem, (int)direction);
		}

		internal static void UnregisterFromEventSystem(string eventsystem, UIEventDirection direction)
		{
			_UnregisterFromEventSystem(eventsystem, (int)direction);
		}


		internal static void SendEvent(string eventsystem, int Event, object[] args)
		{
			_SendEvent(eventsystem, Event, args);
		}
		internal static void SendEvent(string eventsystem, string Event, object[] args)
		{
			_SendNamedEvent(eventsystem, Event, args);
		}

		internal delegate void UIEventDelegate(object sender, UIEventArgs e);

		/// <summary>
		/// UI.Events += MyUIEventDelegateMethod;
		/// </summary>
		internal static event UIEventDelegate Events;

		internal static void TestInit()
		{
			Debug.LogAlways("Test init called!");
			bool b;
			b = RegisterToEventSystem("MenuEvents", UIEventDirection.ToSystem);
			Debug.LogAlways("RegisterToEventSystem(\"MenuEvents\") == {0}", b);
			UIEventDescription desc = new UIEventDescription("TestEvent", "TestEventDName", "TestEventDescription");
			desc.Params = new Object[2];
			desc.Params[0] = new UIParameterDescription("Param1", "Param1DName", "Param1Desc", UIParameterType.String);
			desc.Params[1] = new UIParameterDescription("Param2", "Param2DName", "Param2Desc", UIParameterType.Int);
			int i = RegisterEvent("MyEvent", UIEventDirection.ToSystem, desc);
			Debug.LogAlways("RegisterEvent == {0}", i);
			i = RegisterEvent("MyEvent2", UIEventDirection.ToSystem, desc);
			Debug.LogAlways("RegisterEvent2 == {0}", i);

			/*desc = new UIEventDescription("BoidCount", "BoidCount", "Sets the boid count");
			desc.Params = new Object[1];
			desc.Params[0] = new UIParameterDescription("Count", "Count", "Number of available boids", UIParameterType.Int);
			i = RegisterEvent("AngryBoids", UIEventDirection.SystemToUI, desc);*/
		}

		public static void LoadEvent(CryScript script)
		{
			UIEventAttribute attribute;
			if(script.ScriptType.TryGetAttribute(out attribute))
			{
				UIEventDescription eventDesc = new UIEventDescription(attribute.Name, attribute.Name, attribute.Description);
				UIEventDirection eventDirection = UIEventDirection.ToSystem;

				Collection<UIParameterDescription> parameterDescriptions = new Collection<UIParameterDescription>();

				foreach(var member in script.ScriptType.GetMembers())
					ProcessMember(member, attribute, ref parameterDescriptions, ref eventDirection);

				eventDesc.Params = parameterDescriptions.Cast<object>().ToArray();

				RegisterEvent(attribute.Category, eventDirection, eventDesc);
			}
		}

		static void ProcessMember(MemberInfo member, UIEventAttribute parentAttribute, ref Collection<UIParameterDescription> parameterDescriptions, ref UIEventDirection eventDirection)
		{
			PortAttribute attribute;
			if(member.TryGetAttribute(out attribute))
			{
				switch(member.MemberType)
				{
					case MemberTypes.Method:
						{
							if(!toSystemEventSystems.Contains(parentAttribute.Category))
							{
								RegisterToEventSystem(parentAttribute.Category, UIEventDirection.ToSystem);

								toSystemEventSystems.Add(parentAttribute.Category);
							}

							parameterDescriptions.Add(new UIParameterDescription(attribute.Name, attribute.Name, attribute.Description, UIParameterType.String));
						}
						break;
					case MemberTypes.Field:
					case MemberTypes.Property:
						{
							if(!toUIEventSystems.Contains(parentAttribute.Category))
							{
								RegisterToEventSystem(parentAttribute.Category, UIEventDirection.ToUI);

								toUIEventSystems.Add(parentAttribute.Category);
							}

							eventDirection = UIEventDirection.ToUI;

							parameterDescriptions.Add(new UIParameterDescription(attribute.Name, attribute.Name, attribute.Description, UIParameterType.String));
						}
						break;
				}
			}
		}

		/// <summary>
		/// Event systems handling UI -> System communication.
		/// </summary>
		static Collection<string> toSystemEventSystems = new Collection<string>();
		/// <summary>
		/// Event systems handling System -> UI communication.
		/// </summary>
		static Collection<string> toUIEventSystems = new Collection<string>();
	}

	public class UIEventAttribute : Attribute
	{
		public string Name { get; set; }
		public string Description { get; set; }

		public string Category { get; set; }
	}

	internal enum UIParameterType
	{
		Invalid = -1,
		Any = 0,
		Bool,
		Int,
		Float,
		String,
	};

	internal enum UIEventDirection
	{
		ToSystem = 0,
		ToUI,
	};

	internal struct UIParameterDescription
	{
		public UIParameterType Type;
		public string Name;
		public string DisplayName;
		public string Description;
		public UIParameterDescription(UIParameterType type = UIParameterType.Any)
		{
			Type = UIParameterType.Any;
			Name = "Undefined";
			DisplayName = "Undefined";
			Description = "Undefined";
		}
		public UIParameterDescription(string name, string displayname, string description, UIParameterType type = UIParameterType.Any)
		{
			Type = type;
			Name = name;
			DisplayName = displayname;
			Description = description;
		}
	};

	internal struct UIEventDescription
	{

		public UIParameterType Type;
		public string Name;
		public string DisplayName;
		public string Description;

		public Object[] Params;
		public bool IsDynamic;
		public string DynamicName;
		public string DynamicDesc;
		public UIEventDescription(UIParameterType type = UIParameterType.Any)
		{
			Type = UIParameterType.Any;
			Name = "Undefined";
			DisplayName = "Undefined";
			Description = "Undefined";

			IsDynamic = false;
			DynamicName = "Array";
			DynamicDesc = "";
			Params = null;
		}
		public UIEventDescription(string name, string displayname, string description, bool isdyn = false, string dynamicname = "Array", string dynamicdesc = "")
		{
			Type = UIParameterType.Any;
			Name = name;
			DisplayName = displayname;
			Description = description;

			IsDynamic = isdyn;
			DynamicName = dynamicname;
			DynamicDesc = dynamicdesc;
			Params = null;
		}
		public void SetDynamic(string dynamicname, string dynamicdesc)
		{
			IsDynamic = true;
			DynamicName = dynamicname;
			DynamicDesc = dynamicdesc;
		}
	};

	internal class UIEventArgs : System.EventArgs
	{
		public string EventName { get; private set; }
		public string EventSystem { get; private set; }
		public int EventID { get; private set; }
		public Object[] Args { get; private set; }
		public UIEventArgs(string eventSystem, string eventName, int eventID, object[] args)
		{
			EventName = eventName;
			EventSystem = eventSystem;
			EventID = eventID;
			Args = args;
		}
	}
}
