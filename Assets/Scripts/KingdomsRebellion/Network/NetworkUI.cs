using UnityEngine;
using System.Text;
using KingdomsRebellion.Core;

namespace KingdomsRebellion.Network {

	[RequireComponent (typeof(NetworkAPI))]
	public class NetworkUI : KRBehaviour {

		static StringBuilder Builder = new StringBuilder().AppendLine("Console :");
		static int BuilderNbLines = 0;
		static string Console = "Console :";
		static bool IsSetup, IsConnected, IsLaunched;
		string ip, port;

		void Start() {
			ip = "127.0.0.1";
			port = "8888";
		}

		void OnEnable() {
			On("OnConnection");
		}

		void OnDisable() {
			Off("OnConnection");
		}

		void OnConnection() {
			Log("Second player is in the place !");
			IsConnected = true;
		}

		public static void ClearLog() {
			Builder = new StringBuilder().AppendLine("Console :");
			BuilderNbLines = 0;
		}

		public static void Log(string log) {
			if (BuilderNbLines > 30) {
				ClearLog();
			}
			Console = Builder.AppendLine(log).ToString();
			++BuilderNbLines;
		}

		void OnGUI() {
			GUILayout.BeginVertical("box");

			ip = GUILayout.TextField(ip, 32);
			port = GUILayout.TextField(port, 4);

			GUILayout.Label("playerID " + NetworkAPI.PlayerId); // TEST display player id

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

			// GUILayout.Label(Console);

			GUILayout.EndVertical();
		}
	}

}