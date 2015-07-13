using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

/*
 * disable by default
 * only one instance
 * 
 * @todo handle errors, display, ...
 */
public class NetworkAPI : MonoBehaviour {

	// Event throw when a new player is connected
	public delegate void ConnectionEvent();

	public static event ConnectionEvent Connection;

	// Event throw when action is received
	public delegate void ActionEvent(int playerID,GameAction action);

	public static event ActionEvent ReceiveAction;

	// Event throw when confirmation is received
	public delegate void ConfirmationEvent(int playerID,GameConfirmation confirmation);

	public static event ConfirmationEvent ReceiveConfirmation;

	public static int[] Players;

	public static int PlayerId { get; /*private*/ set; } // TODO player id set to private
//	public static int NumberOfPlayers { get; private set; };

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

	void Start() {
		Debug.Assert(self == null, "NetworkAPI can't be instantiate more than one time...");
		enabled = false;
		self = this;

		Players = new int[maxConnection];
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
	}
	
	public static void SendConfirmation(GameConfirmation confirmation) {
		byte[] buffer = confirmation.ToBytes();
		NetworkTransport.Send(hostId, connectionId, channelConfirmationId, buffer, buffer.Length, out error);
	}
	
	public static void SetupClient(string ip, string port) {
		PlayerId = 1; // TEST set player id to 1 for client

		GameObject realMainCamera = GameObject.Find("Cameras/Camera ("+PlayerId+")") as GameObject;
		realMainCamera.GetComponent<Camera>().enabled = true;
		Camera.main.enabled = false; // Camera.main is now equal to realMainCamera.GetComponent<Camera>()

		connectionId = NetworkTransport.Connect(hostId, ip, Convert.ToInt32(port), 0, out error);
	}
	
	void Update() {
		eventType = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, buffer, bufferSize, out dataSize, out error);
		if (channelId == channelSetupId) {
			switch (eventType) {
				case NetworkEventType.ConnectEvent:
					NetworkUI.Log("Connection on socket " + recHostId + ", connection : " + connectionId + ", channelId : " + channelId);
					if (PlayerId == 0) {
						Connection();
					}
					break;
//			case NetworkEventType.DataEvent: PlayerId = buffer[0]; print("playerid is now "+PlayerId.ToString()); break;
				case NetworkEventType.DisconnectEvent:
					Application.Quit(); // TEST quit on disconnect
					break;
			}
		} else if (eventType == NetworkEventType.DataEvent) {
			if (channelId == channelActionId && ReceiveAction != null) {
				ReceiveAction(PlayerId == 0 ? 1 : 0, GameActionFactory.Get(buffer));
			} else if (channelId == channelConfirmationId && ReceiveConfirmation != null) {
				ReceiveConfirmation(PlayerId == 0 ? 1 : 0, GameConfirmation.FromBytes(buffer));
			}
		}
	}
	
	void OnApplicationQuit() {
		NetworkTransport.Shutdown();
	}
}
