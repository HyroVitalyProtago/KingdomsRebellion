using UnityEngine;
using System.Collections;
using System.Text;
using System;

namespace KingdomsRebellion.Network {

	[RequireComponent (typeof(NetworkAPI))]
	public class NetworkUI : MonoBehaviour {

		// TEST inc frame i
		// =====================
		static int i = 0;

		public static void Inc() {
			++i;
		}
		// =====================

		static int BuilderNbLines = 0;
		static StringBuilder builder = new StringBuilder().AppendLine("Console :");
		static string Console = "Console :";
		string ip, port;
		static bool IsSetup, IsConnected, IsLaunched;

		void Start() {
			ip = "127.0.0.1";
			port = "8888";
		}

		void OnEnable() {
			NetworkAPI.Connection += OnConnectionEvent;
		}

		void OnDisable() {
			NetworkAPI.Connection -= OnConnectionEvent;
		}

		void OnConnectionEvent() {
			Log("Second player is in the place !");
			IsConnected = true;
		}

		public static void ClearLog() {
			builder = new StringBuilder().AppendLine("Console :");
			BuilderNbLines = 0;
		}

		public static void Log(string log) {
			if (BuilderNbLines > 30) {
				ClearLog();
			}
			Console = builder.AppendLine(log).ToString();
			++BuilderNbLines;
		}

		void OnGUI() {
			GUILayout.BeginVertical("box");

			ip = GUILayout.TextField(ip, 32);
			port = GUILayout.TextField(port, 4);

			// TEST display player id
			// =====================
			GUILayout.Label("playerID " + NetworkAPI.PlayerId);
			// =====================

			if (!IsSetup && GUILayout.Button("Init")) {
				Log("Init network on port " + port);
				NetworkAPI.Setup(port);
				IsSetup = true;
			}
			if (IsSetup && !IsConnected && GUILayout.Button("Connect")) {
				Log("Setup connection on " + ip + ":" + port);
				NetworkAPI.SetupClient(ip, port);
				IsConnected = true;
			}
			if (IsSetup && IsConnected && !IsLaunched && GUILayout.Button("Launch Lockstep")) {
				Log("Launch Lockstep...");
				gameObject.GetComponent<Lockstep>().enabled = true;
				IsLaunched = true;
			}

			// TEST display frame id
			// =====================
			GUILayout.Label("" + i);
			// =====================

			GUILayout.Label(Console);

			GUILayout.EndVertical();
		}
	}

}