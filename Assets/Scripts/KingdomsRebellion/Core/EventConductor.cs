using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using System.Linq;

namespace KingdomsRebellion.Core {

	/// <summary>
	/// Event conductor is useful for abstract event links between objects.
	/// Because an object generally don't need to know who declench the event,
	/// this class play the role of middleman for attach and detach events.
	/// </summary>
	public static class EventConductor {

		public class EventNotFoundException : Exception {}
		public class EventNotRegisteredException : Exception {}
		public class EventAllreadyOffered : Exception {} // TODO
		public class CallbackNotFoundException : Exception {}
		public class CallbackBadTypeException : Exception {}
		public class CallbackAllreadyConnected : Exception {} // TODO
		public class CallbackNotRegisteredException : Exception {}
		public class EventNotMatchCallbackException : Exception {}

		static readonly String EventPrefixAdd = "add_";
		static readonly int EventAddID = 0;
		static readonly String EventPrefixRemove = "remove_";
		static readonly int EventRemoveID = 1;

		static readonly BindingFlags StaticNonPublic = BindingFlags.Static | BindingFlags.NonPublic;
		static readonly BindingFlags InstanceNonPublic = BindingFlags.Instance | BindingFlags.NonPublic;

		/// <summary>
		/// The static talkers correspond to classes who launch static events.
		/// </summary>
		static Dictionary<Type, Dictionary<String, MethodInfo[]>> StaticTalkers =
			new Dictionary<Type, Dictionary<String, MethodInfo[]>>();

		/// <summary>
		/// The dynamic talkers correspond to classes who launch events which refer to them.
		/// </summary>
		static Dictionary<System.Object, Dictionary<String, MethodInfo[]>> DynamicTalkers =
			new Dictionary<System.Object, Dictionary<String, MethodInfo[]>>();

		/// <summary>
		/// The listeners correspond to classes who attend some events for fire callbacks.
		/// There can be two kinds of listeners : Type (for static method) and Object (for instance method)
		/// </summary>
		static Dictionary<String, Dictionary<System.Object, Delegate>> Listeners =
			new Dictionary<String, Dictionary<System.Object, Delegate>>();

		#region Talkers

		static void AbstractOffer<T>(
			T talker,
			String eventName,
			BindingFlags flags,
			Dictionary<T, Dictionary<String, MethodInfo[]>> talkers
		) {
			if (talker == null || eventName == null) {
				throw new ArgumentNullException();
			}

			Type typ = talker is Type ? talker as Type : talker.GetType();

			MethodInfo eventAdd = typ.GetMethod(EventPrefixAdd + eventName, flags);
			MethodInfo eventRemove = typ.GetMethod(EventPrefixRemove + eventName, flags);
			if (eventAdd == null || eventRemove == null) {
				throw new EventNotFoundException();
			}

			System.Object invokedTalker = talker is Type ? null : talker as System.Object;

			// Connect all listeners
			if (Listeners.ContainsKey(eventName)) {
				foreach (var pair in Listeners[eventName]) {
					try {
						eventAdd.Invoke(invokedTalker, new object[] { pair.Value });
					} catch(ArgumentException) {
						throw new EventNotMatchCallbackException();
					}
				}
			}

			// Add in static talkers
			if (!talkers.ContainsKey(talker)) {
				talkers[talker] = new Dictionary<String, MethodInfo[]>();
			}
			talkers[talker].Add(eventName, new MethodInfo[]{ eventAdd, eventRemove });
		}

		static void AbstractDenial<T>(
			T talker,
			string eventName,
			Dictionary<T, Dictionary<String, MethodInfo[]>> talkers
		) {
			if (talker == null || eventName == null) {
				throw new ArgumentNullException();
			}
			
			if (!talkers.ContainsKey(talker) || !talkers[talker].ContainsKey(eventName)) {
				throw new EventNotRegisteredException();
			}

			System.Object invokedTalker = talker is Type ? null : talker as System.Object;

			// Disconnect all listeners
			if (Listeners.ContainsKey(eventName)) {
				foreach (var pair in Listeners[eventName]) {
					talkers[talker][eventName][EventRemoveID].Invoke(invokedTalker, new object[] { pair.Value });
				}
			}
			
			talkers[talker].Remove(eventName);
		}

		public static void Offer(Type typ, String eventName) {
			AbstractOffer<Type>(typ, eventName, StaticNonPublic, StaticTalkers);
		}

		public static void Denial(Type typ, string eventName) {
			AbstractDenial<Type>(typ, eventName, StaticTalkers);
		}

		public static void Offer(System.Object self, string eventName) {
			AbstractOffer<System.Object>(self, eventName, InstanceNonPublic, DynamicTalkers);
		}

		public static void Denial(System.Object self, string eventName) {
			AbstractDenial<System.Object>(self, eventName, DynamicTalkers);
		}

		#endregion

		#region Listeners

		static void AbstractOn(System.Object self, String eventName, BindingFlags flags) {
			if (self == null || eventName == null) {
				throw new ArgumentNullException();
			}

			Type typ = self is Type ? self as Type : self.GetType();

			MethodInfo method = typ.GetMethod(eventName, flags);
			if (method == null) {
				throw new CallbackNotFoundException();
			}
			
			Delegate callback;
			try {
				if (self is Type) {
					callback = Delegate.CreateDelegate(DelegateType(method), typ, eventName);
				} else {
					callback = Delegate.CreateDelegate(DelegateType(method), self, eventName);
				}
			} catch(ArgumentException) {
				throw new CallbackBadTypeException();
			} // MethodAccessException
			
			// Connect all talkers
			foreach (var pair in StaticTalkers) {
				if (pair.Value.ContainsKey(eventName)) {
					pair.Value[eventName][EventAddID].Invoke(null, new object[] { callback });
				}
			}
			foreach (var pair in DynamicTalkers) {
				if (pair.Value.ContainsKey(eventName)) {
					pair.Value[eventName][EventAddID].Invoke(pair.Key, new object[] { callback });
				}
			}
			
			// Add in listeners
			if (!Listeners.ContainsKey(eventName)) {
				Listeners[eventName] = new Dictionary<System.Object, Delegate>();
			}
			Listeners[eventName].Add(self, callback);
		}

		static void AbstractOff(System.Object obj, String eventName) {
			if (obj == null || eventName == null) {
				throw new ArgumentNullException();
			}
			
			if (!Listeners.ContainsKey(eventName) || !Listeners[eventName].ContainsKey(obj)) {
				throw new CallbackNotRegisteredException();
			}
			
			// Disconnect all talkers
			foreach (var pair in StaticTalkers) {
				if (pair.Value.ContainsKey(eventName)) {
					pair.Value[eventName][EventRemoveID].Invoke(null, new object[] { Listeners[eventName][obj] });
				}
			}
			foreach (var pair in DynamicTalkers) {
				if (pair.Value.ContainsKey(eventName)) {
					pair.Value[eventName][EventRemoveID].Invoke(pair.Key, new object[] { Listeners[eventName][obj] });
				}
			}
			
			Listeners[eventName].Remove(obj);
		}

		public static void On(Type typ, string eventName) {
			AbstractOn(typ, eventName, StaticNonPublic);
		}

		public static void Off(Type typ, string eventName) {
			AbstractOff(typ, eventName);
		}

		public static void On(System.Object self, string eventName) {
			AbstractOn(self, eventName, InstanceNonPublic);
		}

		public static void Off(System.Object self, string eventName) {
			AbstractOff(self, eventName);
		}

		#endregion

		static Type DelegateType(MethodInfo method) {
			List<Type> args = new List<Type>(method.GetParameters().Select(p => p.ParameterType));
			if (method.ReturnType == typeof(void)) {
				return Expression.GetActionType(args.ToArray());
			} else {
				args.Add(method.ReturnType);
				return Expression.GetFuncType(args.ToArray());
			}
		}
	}

}