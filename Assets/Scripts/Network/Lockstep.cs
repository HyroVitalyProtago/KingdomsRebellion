using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent (typeof(NetworkAPI))]
public class Lockstep : MonoBehaviour {

	static int WaitingTime = 4; // in game frame

	float accumulatedTime;
	int gameFrame;
	float gameFrameTurnLength;
	int gameFramesPerLockstepTurn;
	int firstLockStepTurnID;
	int lockStepTurnID;
	int numberOfPlayers;

	Queue<GameAction> actionQueue;

	GameAction[][] actions; // [turn, player]
	int[] numberOfPlayerWhoSendAction; // [turn]

	bool[][] playerHaveConfirmedMyAction; // [turn, player]
	int[] numberOfPlayerWhoConfirmedMyAction; // [turn]

	int[][] wait; // [turn, player]
	
	void Start() {
		enabled = false;

		accumulatedTime = 0;
		gameFrame = 0;
		gameFrameTurnLength = 50f; // 50 miliseconds
		gameFramesPerLockstepTurn = 4;

		firstLockStepTurnID = 0;
		lockStepTurnID = 0;

		numberOfPlayers = 2;

		actionQueue = new Queue<GameAction>();

		actions = new GameAction[3][];
		for (int i = 0; i < actions.Length; ++i) {
			actions[i] = new GameAction[numberOfPlayers]; // allready set to null
		}

		numberOfPlayerWhoSendAction = new int[3]; // allready set to 0

		playerHaveConfirmedMyAction = new bool[3][];
		for (int i = 0; i < playerHaveConfirmedMyAction.Length; ++i) {
			playerHaveConfirmedMyAction[i] = new bool[numberOfPlayers]; // allready set to false
		}
		
		numberOfPlayerWhoConfirmedMyAction = new int[3]; // allready set to 0

		wait = new int[3][];
		for (int i = 0; i < wait.Length; ++i) {
			wait[i] = new int[numberOfPlayers]; // allready set to false
		}
	}

	void OnEnable() {
		NetworkAPI.ReceiveAction += OnAction;
		NetworkAPI.ReceiveConfirmation += OnConfirmation;

		Mouse.OnLeftClick += OnLeftClick;
		Mouse.OnRightClick += OnRightClick;
	}

	void OnDisable() {
		NetworkAPI.ReceiveAction -= OnAction;
		NetworkAPI.ReceiveConfirmation -= OnConfirmation;

		Mouse.OnLeftClick -= OnLeftClick;
		Mouse.OnRightClick -= OnRightClick;
	}

	void OnAction(int playerID, GameAction action) {
		int turn = action.LockStepTurn - lockStepTurnID;
		bool wait = turn > 2; // Send wait message for stop overloaded network

		NetworkUI.Log("Receive action from player " + playerID + " for turn " + turn);

		if (turn > 0) {
			if (turn <= 2 && actions[turn][playerID] == null) {
				actions[turn][playerID] = action;
				++numberOfPlayerWhoSendAction[turn];
			} else {
//				NetworkUI.Log("Action allready received for LockstepTurn " + action.LockStepTurn);
//				NetworkUI.Log("Maybe my confirmation has not been received...");
			}
		}

		NetworkAPI.SendConfirmation(new GameConfirmation(action.LockStepTurn, wait));
	}

	void OnConfirmation(int playerID, GameConfirmation confirmation) {
		int turn = confirmation.LockStepTurn - lockStepTurnID;

		if (turn < 0) { // Discard
			return;
		}

		if (turn > 2) {
			Debug.LogError("Lockstep : Confirmation from player " + playerID + " on impossible turn > 2 : " + turn);
			return;
		}

		NetworkUI.Log("Receive confirmation from player " + playerID + " for turn " + turn);

		if (confirmation.Wait) {
			NetworkUI.Log("Player " + playerID + " want me to wait, i'm too overhead !");
			wait[turn][playerID] = WaitingTime;
			return;
		}

		if (!playerHaveConfirmedMyAction[turn][playerID]) {
			playerHaveConfirmedMyAction[turn][playerID] = true;
			++numberOfPlayerWhoConfirmedMyAction[turn];
		} else {
			NetworkUI.Log("Confirmation allready received for LockstepTurn " + confirmation.LockStepTurn);
		}
	}

	void Update() {
		accumulatedTime = accumulatedTime + Convert.ToInt32((Time.deltaTime * 1000));

		while (accumulatedTime > gameFrameTurnLength) {
			GameFrameTurn();
			accumulatedTime = accumulatedTime - gameFrameTurnLength;
		}
	}

	void GameFrameTurn() {
		NetworkUI.ClearLog(); // TEST network ui clear log on game frame turn
		NetworkUI.Log("LockstepTurn : " + lockStepTurnID + " ; gameFrame : " + gameFrame);

		if (gameFrame == 0) {
			if (LockStepTurn()) {
				++gameFrame;
			}
		} else {
			UpdateGame();
			
			++gameFrame;
			if (gameFrame == gameFramesPerLockstepTurn) {
				gameFrame = 0;
				NextTurn();
			}
		}

//		SendActionToAll(); // send action to all who haven't confirmed
	}

	void NextTurn() {
		++lockStepTurnID;

		// Shift left actions
		for (int i = 0; i < actions.Length-1; ++i) {
			actions[i] = actions[i + 1];
			NetworkUI.Log("actions[" + i + "][NetworkAPI.PlayerId] = " + actions[i][NetworkAPI.PlayerId]);
			NetworkUI.Log("actions[" + (i + 1) + "][NetworkAPI.PlayerId] = " + actions[i + 1][NetworkAPI.PlayerId]);
		}
		// Last reset
		actions[actions.Length - 1] = new GameAction[numberOfPlayers]; // allready set to null
		
		// Shift left numberOfPlayerWhoSendAction
		for (int i = 0; i < numberOfPlayerWhoSendAction.Length-1; ++i) {
			numberOfPlayerWhoSendAction[i] = numberOfPlayerWhoSendAction[i + 1];
		}
		// Last reset
		numberOfPlayerWhoSendAction[numberOfPlayerWhoSendAction.Length - 1] = 0;

		// Shift left playerHaveConfirmedMyAction
		for (int i = 0; i < playerHaveConfirmedMyAction.Length-1; ++i) {
			playerHaveConfirmedMyAction[i] = playerHaveConfirmedMyAction[i + 1];
		}
		// Last reset
		playerHaveConfirmedMyAction[playerHaveConfirmedMyAction.Length - 1] = new bool[numberOfPlayers]; // allready set to false

		// Shift left numberOfPlayerWhoConfirmedMyAction
		for (int i = 0; i < numberOfPlayerWhoConfirmedMyAction.Length-1; ++i) {
			numberOfPlayerWhoConfirmedMyAction[i] = numberOfPlayerWhoConfirmedMyAction[i + 1];
		}
		// Last reset
		numberOfPlayerWhoConfirmedMyAction[numberOfPlayerWhoConfirmedMyAction.Length - 1] = 0;
	}

	/*
	 * Did we receive every client’s action for the next turn ?
	 * Did every client confirm that they received our action ?
	 */
	bool LockStepTurn() {
		bool allActionsReceived = numberOfPlayerWhoSendAction[0] == numberOfPlayers;
		bool allActionsConfirmed = numberOfPlayerWhoConfirmedMyAction[0] == numberOfPlayers;
		bool readyForTurn = (allActionsReceived && allActionsConfirmed) || lockStepTurnID < firstLockStepTurnID + 2;

		if (readyForTurn) {
			NetworkUI.Log("Ready for turn " + lockStepTurnID);

			SetAction(); // set wantedActions[1]

			if (lockStepTurnID >= firstLockStepTurnID + 3) {
				ProcessActions();
				NetworkUI.Inc(); // TEST NetworUI inc frame number
			}
		} else {
			if (!allActionsReceived) {
				NetworkUI.Log("Waiting of " + (numberOfPlayers - numberOfPlayerWhoSendAction[0]) + " actions...");
			}
			if (!allActionsConfirmed) {
				NetworkUI.Log("Waiting of " + (numberOfPlayers - numberOfPlayerWhoConfirmedMyAction[0]) + " confirmations...");
			}
		}

		SendActionToAll(); // send action to all who haven't confirmed

		return readyForTurn;
	}

	void ProcessActions() {
		for (int i = 0; i < actions[0].Length; ++i) {
			actions[0][i].Process(i);
		}
	}

	void SendActionToAll() {
		for (int i = 0; i < actions.Length; ++i) {
			GameAction action = actions[i][NetworkAPI.PlayerId];
			if (action == null) {
				NetworkUI.Log("action null for turn " + i);
				continue;
			}
			for (int j = 0; j < playerHaveConfirmedMyAction[i].Length; ++j) {
				if (wait[i][j] > 0) {
					--wait[i][j];
					continue;
				}
				if (!playerHaveConfirmedMyAction[i][j]) {
					if (j == NetworkAPI.PlayerId) {
						Debug.LogError("Player can't have not confirmed it's action...");
					} else {
						NetworkUI.Log("Send action for turn " + i + " to all...");
						NetworkAPI.SendAction(action); // TODO send to a specific player
					}
				} else {
					NetworkUI.Log("Player " + j + " have confirmed my action for turn " + i);
				}
			}
		}
	}

	void SetAction() {
		if (actionQueue.Count > 0) {
			GameAction action = actionQueue.Dequeue();
			action.LockStepTurn = lockStepTurnID + 2;
			actions[2][NetworkAPI.PlayerId] = action;
		} else {
			actions[2][NetworkAPI.PlayerId] = new NoAction(lockStepTurnID + 2);
		}

		NetworkUI.Log("SetAction " + actions[2][NetworkAPI.PlayerId]);

		++numberOfPlayerWhoSendAction[2];

		playerHaveConfirmedMyAction[2][NetworkAPI.PlayerId] = true;
		++numberOfPlayerWhoConfirmedMyAction[2];
	}
	
	void UpdateGame() {
		// [...]
	}

	void OnLeftClick(int playerId, Camera camera, Vector3 mousePosition) {
		// TODO check if the last action is of the same type,
		// if true, override the previous with it, else, enqueue it
		actionQueue.Enqueue(new SelectAction(
			lockStepTurnID, // bad lockStepTurnID for create action...
			camera.transform.localPosition,
			camera.transform.localEulerAngles,
			camera.orthographicSize,
			mousePosition
		));
	}

	void OnRightClick(int playerId, Camera camera, Vector3 mousePosition) {
		actionQueue.Enqueue(new MoveAction(
			lockStepTurnID, // bad lockStepTurnID for create action...
			camera.transform.localPosition,
			camera.transform.localEulerAngles,
			camera.orthographicSize,
			mousePosition
		));
	}
}