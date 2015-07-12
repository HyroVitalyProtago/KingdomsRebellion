using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent (typeof(NetworkAPI))]
public class Lockstep : MonoBehaviour {

	float accumulatedTime;
	int gameFrame;
	float gameFrameTurnLength;
	int gameFramesPerLockstepTurn;
	int firstLockStepTurnID;
	int lockStepTurnID;
	int numberOfPlayers;

	//
	int[] waitingForOthers;
	//

	GameAction[][] actions; // [turn, player]
	int[] numberOfPlayerWhoSendAction; // [turn]

	bool[][] playerHaveConfirmedMyAction; // [turn, player]
	int[] numberOfPlayerWhoConfirmedMyAction; // [turn]

	void Start() {
		enabled = false;

		accumulatedTime = 0;
		gameFrame = 0;
		gameFrameTurnLength = 50f; // 50 miliseconds
		gameFramesPerLockstepTurn = 4;

		firstLockStepTurnID = 0;
		lockStepTurnID = 0;

		numberOfPlayers = 2;

		// --------------------------------------
		waitingForOthers = new int[32];
		for (int i = 0; i < waitingForOthers.Length; ++i) {
			waitingForOthers[i] = -1;
		}
		// --------------------------------------

		actions = new GameAction[6][];
		for (int i = 0; i < actions.Length; ++i) {
			actions[i] = new GameAction[numberOfPlayers];
			for (int j = 0; j < numberOfPlayers; ++j) {
				actions[i][j] = null;
			}
		}

		numberOfPlayerWhoSendAction = new int[6];
		for (int i = 0; i < numberOfPlayerWhoSendAction.Length; ++i) {
			numberOfPlayerWhoSendAction[i] = 0;
		}

		playerHaveConfirmedMyAction = new bool[6][];
		for (int i = 0; i < playerHaveConfirmedMyAction.Length; ++i) {
			playerHaveConfirmedMyAction[i] = new bool[numberOfPlayers];
			for (int j = 0; j < numberOfPlayers; ++j) {
				playerHaveConfirmedMyAction[i][j] = false;
			}
		}
		
		numberOfPlayerWhoConfirmedMyAction = new int[6];
		for (int i = 0; i < numberOfPlayerWhoConfirmedMyAction.Length; ++i) {
			numberOfPlayerWhoConfirmedMyAction[i] = 0;
		}
	}

	void OnEnable() {
		NetworkAPI.ReceiveAction += OnAction;
		NetworkAPI.ReceiveConfirmation += OnConfirmation;
	}

	void OnDisable() {
		NetworkAPI.ReceiveAction -= OnAction;
		NetworkAPI.ReceiveConfirmation -= OnConfirmation;
	}

	void OnAction(int playerID, GameAction action) {
		int turn = action.LockStepTurn - lockStepTurnID;

		if (turn < 0 || turn > 4) {
			Debug.LogError("Confirmation from player " + playerID + " on impossible turn [0;4] : " + turn);
			return;
		}

		NetworkUI.Log("Receive action from player " + playerID + " for turn " + turn);

		if (actions[turn][playerID] == null) {
			actions[turn][playerID] = action;
			++numberOfPlayerWhoSendAction[turn];
		} else {
			NetworkUI.Log("Action allready received for LockstepTurn " + action.LockStepTurn);
			NetworkUI.Log("Maybe my confirmation has not been received...");
		}

		NetworkAPI.SendConfirmation(new GameConfirmation(action.LockStepTurn, turn > 2));
	}

	void OnConfirmation(int playerID, GameConfirmation confirmation) {
		int turn = confirmation.LockStepTurn - lockStepTurnID;

		if (turn < 0 || turn > 2) {
			Debug.LogError("Confirmation from player " + playerID + " on impossible turn [0;2] : " + turn);
			return;
		}

		NetworkUI.Log("Receive confirmation from player " + playerID + " for turn " + turn);

		if (!playerHaveConfirmedMyAction[turn][playerID]) {
			playerHaveConfirmedMyAction[turn][playerID] = true;
			if (confirmation.Wait) { // need to wait the other player for a better sync
				waitingForOthers[turn*4 + 16] = turn;
			} else {
				++numberOfPlayerWhoConfirmedMyAction[turn];
			}
		} else {
			NetworkUI.Log("Confirmation allready received for LockstepTurn " + confirmation.LockStepTurn);
		}
	}

	/*
	 * Launch GameFrameTurn every (<gameFrameTurnLength> * 1000) miliseconds, in this case, every 50ms
	 */
	void Update() {
		accumulatedTime = accumulatedTime + Convert.ToInt32((Time.deltaTime * 1000));

		while (accumulatedTime > gameFrameTurnLength) {
			GameFrameTurn();
			accumulatedTime = accumulatedTime - gameFrameTurnLength;
		}
	}

	/*
	 * Check for LockStepTurn on first gameFrame
	 * When LockStepTurn is ok, go to the next frame
	 * Update Game on the other frames
	 */
	void GameFrameTurn() {
		NetworkUI.ClearLog(); // TEST network ui clear log on game frame turn
		NetworkUI.Log("LockstepTurn : " + lockStepTurnID + " ; gameFrame : " + gameFrame);

		// Wait others if there is confirmation(s) with waiting
		if (waitingForOthers[0] != -1) {
			++numberOfPlayerWhoConfirmedMyAction[waitingForOthers[0]];
		}

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

		// send action to all who haven't confirmed
		SendActionToAll();
	}

	void NextTurn() {
		++lockStepTurnID;

		for (int i = 0; i < waitingForOthers.Length-1; ++i) {
			waitingForOthers[i] = waitingForOthers[i+1];
		}
		waitingForOthers[waitingForOthers.Length-1] = -1;


		// Shift left actions
		for (int i = 0; i < actions.Length-1; ++i) {
			actions[i] = actions[i + 1];
			NetworkUI.Log("actions[" + i + "][NetworkAPI.PlayerId] = " + actions[i][NetworkAPI.PlayerId]);
			NetworkUI.Log("actions[" + (i + 1) + "][NetworkAPI.PlayerId] = " + actions[i + 1][NetworkAPI.PlayerId]);
		}
		// Last reset
		actions[actions.Length - 1] = new GameAction[numberOfPlayers];
		for (int j = 0; j < numberOfPlayers; ++j) {
			actions[actions.Length - 1][j] = null;
		}
		
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
		playerHaveConfirmedMyAction[playerHaveConfirmedMyAction.Length - 1] = new bool[numberOfPlayers];
		for (int j = 0; j < numberOfPlayers; ++j) {
			playerHaveConfirmedMyAction[playerHaveConfirmedMyAction.Length - 1][j] = false;
		}

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

		if ((allActionsReceived && allActionsConfirmed) || lockStepTurnID < firstLockStepTurnID + 2) {
			NetworkUI.Log("Ready for turn " + lockStepTurnID);

			SetAction(); // set wantedActions[1]

			if (lockStepTurnID >= firstLockStepTurnID + 3) {
				ProcessActions();
				NetworkUI.Inc(); // TEST NetworUI inc frame number
			}

			return true;
		} else {
			if (!allActionsReceived) {
				NetworkUI.Log("Waiting of " + (numberOfPlayers - numberOfPlayerWhoSendAction[0]) + " actions...");
			}
			if (!allActionsConfirmed) {
				NetworkUI.Log("Waiting of " + (numberOfPlayers - numberOfPlayerWhoConfirmedMyAction[0]) + " confirmations...");
			}
		}

		return false;
	}

	void ProcessActions() {
		for (int i = 0; i < actions[0].Length; ++i) {
			actions[0][i].Process();
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
				if (!playerHaveConfirmedMyAction[i][j]) {
					if (j == NetworkAPI.PlayerId) {
						Debug.LogError("Player can't have not confirmed it's action...");
					} else {
						NetworkUI.Log("Send action for turn " + i + "to all...");
						NetworkAPI.SendAction(action); // TODO send to a specific player
					}
				} else {
					NetworkUI.Log("Player " + j + " have confirmed my action for turn " + i);
				}
			}
		}
	}

	void SetAction() {
		if (actions[2][NetworkAPI.PlayerId] == null) {
			actions[2][NetworkAPI.PlayerId] = new GameAction(lockStepTurnID + 2);
			NetworkUI.Log("SetAction " + actions[2][NetworkAPI.PlayerId]);

		}

		++numberOfPlayerWhoSendAction[2];

		playerHaveConfirmedMyAction[2][NetworkAPI.PlayerId] = true;
		++numberOfPlayerWhoConfirmedMyAction[2];
	}
	
	void UpdateGame() {
		// [...]
	}
}