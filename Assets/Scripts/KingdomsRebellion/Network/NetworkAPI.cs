using System;
using KingdomsRebellion.Core;
using KingdomsRebellion.Network.Link;
using UnityEngine;
using UnityEngine.Networking;

namespace KingdomsRebellion.Network {

	/// <summary>
	/// NetworkAPI is the higher level api for exchange messages with others players in network.
	/// </summary>
	public class NetworkAPI : KRBehaviour {

		static event Action OnMainCameraChange;
		event Action OnConnection; // Event throw when a new player is connected
		event Action<int, GameAction> OnAction; // Event throw when action is received
		event Action<int, GameConfirmation> OnConfirmation; // Event throw when confirmation is received

//		public static int[] Players;
		public static byte PlayerId { get; private set; } // TODO player id set to private
//		public static int NumberOfPlayers { get; private set; };

		// Parameters
		public static readonly int bufferSize = 1024;
		public static readonly int maxConnection = 2; // min 2 for localhost

		// Attributes
		static MonoBehaviour self;
		static byte error;
		static byte[] buffer = new byte[bufferSize];
		static int hostId, connectionId, channelSetupId, channelActionId, channelConfirmationId;
		static int dataSize, recHostId, channelId;
		static NetworkEventType eventType;

		void OnEnable() {
			Offer("OnMainCameraChange");
			Offer("OnConnection");
			Offer("OnAction");
			Offer("OnConfirmation");
		}

		void OnDisable() {
			Denial("OnMainCameraChange");
			Denial("OnConnection");
			Denial("OnAction");
			Denial("OnConfirmation");
		}

		void Start() {
			Debug.Assert(self == null, "NetworkAPI can't be instantiate more than one time...");
			enabled = false;
			self = this;

//			Players = new byte[maxConnection];
		}

		public static void Setup(string port) {
			NetworkTransport.Init();

			ConnectionConfig config = new ConnectionConfig();
			channelSetupId = config.AddChannel(QosType.Reliable); // for lobby
			channelActionId = config.AddChannel(QosType.Reliable); // action in game
			channelConfirmationId = config.AddChannel(QosType.Reliable); // confirmation in game

			HostTopology topology = new HostTopology(config, maxConnection);
			hostId = NetworkTransport.AddHost(topology, Convert.ToInt32(port));

			self.enabled = true;
		}
	
		public static void SendAction(GameAction action) {
			byte[] buffer = action.ToBytes();
			NetworkTransport.Send(hostId, connectionId, channelActionId, buffer, buffer.Length, out error);

			if (error != (byte)NetworkError.Ok) {
				Debug.LogError("[ERROR] NetworkAPI :: SendAction :: Send : " + error);
			}
		}
	
		public static void SendConfirmation(GameConfirmation confirmation) {
			byte[] buffer = confirmation.ToBytes();
			NetworkTransport.Send(hostId, connectionId, channelConfirmationId, buffer, buffer.Length, out error);

			if (error != (byte)NetworkError.Ok) {
				Debug.LogError("[ERROR] NetworkAPI :: SendConfirmation :: Send : " + error);
			}
		}
	
		public static void SetupClient(string ip, string port) {
            //PlayerId = 1; // TEST set player id to 1 for client

            //GameObject realMainCamera = GameObject.Find("Cameras/Camera (" + PlayerId + ")");
            //realMainCamera.GetComponent<Camera>().enabled = true;
            //Camera.main.enabled = false; // Camera.main is now equal to realMainCamera.GetComponent<Camera>()

			if (OnMainCameraChange != null)
				OnMainCameraChange();

			connectionId = NetworkTransport.Connect(hostId, ip, Convert.ToInt32(port), 0, out error);

			if (error != (byte) NetworkError.Ok) {
				Debug.LogError("[ERROR] NetworkAPI :: SetupClient :: Connect : " + error);
			}
		}
	
		void Update() {
			eventType = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, buffer, bufferSize, out dataSize, out error);
			if (error != (byte) NetworkError.Ok) {
				Debug.LogError("[ERROR] NetworkAPI :: Update :: Receive : " + error);
				if (dataSize > bufferSize) {
					Debug.LogError("[ERROR] NetworkAPI :: Update : Message too big for be handled by buffer...");
				}
				return;
			}
			if (channelId == channelSetupId) {
				switch (eventType) {
					case NetworkEventType.ConnectEvent:
						NetworkUI.Log("Connection on socket " + recHostId + ", connection : " + connectionId + ", channelId : " + channelId);
						if (PlayerId == 0) {
							OnConnection();
						}
						break;
//					case NetworkEventType.DataEvent: break;
					case NetworkEventType.DisconnectEvent:
						Application.Quit(); // TEST quit on disconnect
						break;
				}
			} else if (eventType == NetworkEventType.DataEvent) {
				if (channelId == channelActionId && OnAction != null) {
					OnAction(PlayerId == 0 ? 1 : 0, GameActionFactory.Get(buffer));
				} else if (channelId == channelConfirmationId && OnConfirmation != null) {
					OnConfirmation(PlayerId == 0 ? 1 : 0, GameConfirmation.FromBytes(buffer));
				}
			}
		}
	
		void OnApplicationQuit() {
			NetworkTransport.Shutdown();
		}
	}

}