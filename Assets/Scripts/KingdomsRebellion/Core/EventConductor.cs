using System;
using System.Collections.Generic;
using System.Reflection;

namespace KingdomsRebellion.Core {

	/// <summary>
	/// Event conductor is useful for abstract event links between objects.
	/// Because an object generally don't need to know who declench the event,
	/// this class play the role of middleman for attach and detach events.
	/// </summary>
	public static class EventConductor {

		public class DenialBeforeOfferException : Exception {}
		public class OffBeforeOnException : Exception {}

		/// <summary>
		/// The static talkers correspond to classes who launch static events.
		/// </summary>
		static Dictionary<Type, IList<string>> StaticTalkers = new Dictionary<Type, IList<string>>();

		/// <summary>
		/// The dynamic talkers correspond to classes who launch events which refer to them.
		/// </summary>
		static Dictionary<Object, IList<string>> DynamicTalkers = new Dictionary<object, IList<string>>();

		/// <summary>
		/// The listeners correspond to classes who attend some events for fire callbacks.
		/// There is two kinds of listeners :
		/// - Object : correspond to listener who use callbacks which refer to it.
		/// - Delegate : correspond to callback declenched by an event.
		/// </summary>
		static Dictionary<string, IList<KeyValuePair<Object, Delegate>>> Listeners = new Dictionary<string, IList<KeyValuePair<Object, Delegate>>>();

		#region Talkers

		/// <summary>
		/// Abstracts the offer.
		/// </summary>
		/// <param name="talker">Talker.</param>
		/// <param name="eventName">Event name.</param>
		/// <param name="typ">Type of talker.</param>
		/// <param name="dic">Dictionary related to talker.</param>
		/// <typeparam name="T">Type of talker.</typeparam>
		static void AbstractOffer<T>(
			T talker,
			string eventName,
			Type typ,
			Dictionary<T, IList<string>> dic
		) {
			// Connect all listeners
			foreach (var pair in Listeners[eventName]) {
				typ.GetEvent(eventName).AddEventHandler(pair.Key, pair.Value);
			}
			
			// Add in dic talkers
			if (dic[talker] == null) {
				dic[talker] = new List<string>();
			}
			dic[talker].Add(eventName);
		}

		/// <summary>
		/// Abstracts the denial.
		/// </summary>
		/// <param name="talker">Talker.</param>
		/// <param name="eventName">Event name.</param>
		/// <param name="typ">Type of talker.</param>
		/// <param name="dic">Dictionary related to talker.</param>
		/// <typeparam name="T">Type of talker.</typeparam>
		static void AbstractDenial<T>(
			T talker,
			string eventName,
			Type typ,
			Dictionary<T, IList<string>> dic
		) {
			// Disconnect all listeners
			foreach (var pair in Listeners[eventName]) {
				typ.GetEvent(eventName).RemoveEventHandler(pair.Key, pair.Value);
			}
			
			// Remove from static talkers
			if (dic[talker] == null) {
				throw new DenialBeforeOfferException();
			}
			dic[talker].Remove(eventName);
		}

		/// <summary>
		/// Offer the specified typ and eventName.
		/// </summary>
		/// <param name="typ">Typ.</param>
		/// <param name="eventName">Event name.</param>
		public static void Offer(Type typ, string eventName) {
			AbstractOffer<Type>(typ, eventName, typ, StaticTalkers);
		}

		/// <summary>
		/// Denial the specified typ and eventName.
		/// </summary>
		/// <param name="typ">Typ.</param>
		/// <param name="eventName">Event name.</param>
		public static void Denial(Type typ, string eventName) {
			AbstractDenial<Type>(typ, eventName, typ, StaticTalkers);
		}

		/// <summary>
		/// Offer the specified obj and eventName.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="eventName">Event name.</param>
		public static void Offer(Object obj, string eventName) {
			AbstractOffer<Object>(obj, eventName, obj.GetType(), DynamicTalkers);
		}

		/// <summary>
		/// Denial the specified obj and eventName.
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="eventName">Event name.</param>
		public static void Denial(Object obj, string eventName) {
			AbstractDenial<Object>(obj, eventName, obj.GetType(), DynamicTalkers);
		}

		#endregion

		#region Listeners

		/// <summary>
		/// Abstracts the on.
		/// </summary>
		/// <param name="listener">Listener.</param>
		/// <param name="eventName">Event name.</param>
		/// <param name="callback">Callback.</param>
		static void AbstractOn(Object listener, string eventName, Delegate callback) {
			// Connect all talkers
			foreach (var pair in StaticTalkers) {
				if (pair.Value.Contains(eventName)) {
					pair.Key.GetEvent(eventName).AddEventHandler(listener, callback);
				}
			}
			foreach (var pair in DynamicTalkers) {
				if (pair.Value.Contains(eventName)) {
					pair.Key.GetType().GetEvent(eventName).AddEventHandler(listener, callback);
				}
			}
			
			// Add in listeners
			if (Listeners[eventName] == null) {
				Listeners[eventName] = new List<KeyValuePair<Object, Delegate>>();
			}
			Listeners[eventName].Add(new KeyValuePair<Object, Delegate>(listener, callback));
		}

		/// <summary>
		/// Abstracts the off.
		/// </summary>
		/// <param name="listener">Listener.</param>
		/// <param name="eventName">Event name.</param>
		/// <param name="callback">Callback.</param>
		static void AbstractOff(Object listener, string eventName, Delegate callback) {
			// Disonnect all talkers
			foreach (var pair in StaticTalkers) {
				if (pair.Value.Contains(eventName)) {
					pair.Key.GetEvent(eventName).RemoveEventHandler(listener, callback);
				}
			}
			foreach (var pair in DynamicTalkers) {
				if (pair.Value.Contains(eventName)) {
					pair.Key.GetType().GetEvent(eventName).RemoveEventHandler(listener, callback);
				}
			}

			// Remove from static talkers
			if (Listeners[eventName] == null) {
				throw new DenialBeforeOfferException();
			}
			foreach (var pair in Listeners[eventName]) {
				if (pair.Key == listener && pair.Value == callback) {
					Listeners.Remove(eventName);
					break;
				}
			}
		}

		/// <summary>
		/// Raises the  event.
		/// </summary>
		/// <param name="eventName">Event name.</param>
		/// <param name="callback">Callback.</param>
		public static void On(string eventName, Delegate callback) {
			AbstractOn(callback, eventName, callback);
		}

		/// <summary>
		/// Off the specified eventName and callback.
		/// </summary>
		/// <param name="eventName">Event name.</param>
		/// <param name="callback">Callback.</param>
		public static void Off(string eventName, Delegate callback) {
			AbstractOff(callback, eventName, callback);
		}

		/// <summary>
		/// Raises the  event.
		/// </summary>
		/// <param name="self">Self.</param>
		/// <param name="eventName">Event name.</param>
		/// <param name="callback">Callback.</param>
		public static void On(Object self, string eventName, Delegate callback) {
			AbstractOn(self, eventName, callback);
		}

		/// <summary>
		/// Off the specified self, eventName and callback.
		/// </summary>
		/// <param name="self">Self.</param>
		/// <param name="eventName">Event name.</param>
		/// <param name="callback">Callback.</param>
		public static void Off(Object self, string eventName, Delegate callback) {
			AbstractOff(self, eventName, callback);
		}

		#endregion
	}

}