using System.Collections.Generic;
using KingdomsRebellion.Core;
using KingdomsRebellion.Core.Math;
using KingdomsRebellion.Network.Link;
using UnityEngine;

namespace KingdomsRebellion.Network {

	[RequireComponent (typeof(NetworkAPI))]
	public class Lockstep : KRBehaviour {

		static readonly float gameFrameTurnLength = 50f; // in milliseconds

		static readonly uint WaitingTime = 4; // in game frame
		static readonly uint gameFramesPerLockstepTurn = 4;
		static readonly uint firstLockstepTurn = 0;
		static readonly uint numberOfPlayers = 2;

		float accumulatedTime;
		uint gameFrame;
		uint lockstepTurn;

		Queue<GameAction> actionQueue;

		GameAction[][] actions; // [turn, player]
		uint[] numberOfPlayerWhoSendAction; // [turn]
		bool[][] playerHaveConfirmedMyAction; // [turn, player]
		uint[] numberOfPlayerWhoConfirmedMyAction; // [turn]
		uint[][] wait; // [turn, player]
	
		void Start() {
			enabled = false;

			accumulatedTime = 0;
			gameFrame = 0;
			lockstepTurn = 0;

			actionQueue = new Queue<GameAction>();

			actions = new GameAction[3][];
			for (int i = 0; i < actions.Length; ++i) {
				actions[i] = new GameAction[numberOfPlayers]; // allready set to null
			}

			numberOfPlayerWhoSendAction = new uint[3]; // allready set to 0

			playerHaveConfirmedMyAction = new bool[3][];
			for (int i = 0; i < playerHaveConfirmedMyAction.Length; ++i) {
				playerHaveConfirmedMyAction[i] = new bool[numberOfPlayers]; // allready set to false
			}
		
			numberOfPlayerWhoConfirmedMyAction = new uint[3]; // allready set to 0

			wait = new uint[3][];
			for (int i = 0; i < wait.Length; ++i) {
				wait[i] = new uint[numberOfPlayers]; // allready set to false
			}
		}

		void OnEnable() {
			On("OnAction");
			On("OnConfirmation");

			On("OnModelSelectDemand");
			On("OnModelMoveDemand");
            On("OnModelAttackDemand");
			On("OnModelDragDemand");
			On("OnModelSpawnDemand");
		}

		void OnDisable() {
			Off("OnAction");
			Off("OnConfirmation");

			Off("OnModelSelectDemand");
			Off("OnModelMoveDemand");
            Off("OnModelAttackDemand");
			Off("OnModelDragDemand");
			Off("OnModelSpawnDemand");
		}

		void OnAction(int playerID, GameAction action) {
			if (action.LockstepTurn > lockstepTurn) {
				uint turn = action.LockstepTurn - lockstepTurn;

				NetworkUI.Log("Receive action from player " + playerID + " for turn " + turn);

				if (turn <= 2 && actions[turn][playerID] == null) {
					actions[turn][playerID] = action;
					++numberOfPlayerWhoSendAction[turn];
				}
			}

			// Send wait message for stop overloaded network
			bool wait = action.LockstepTurn > lockstepTurn + 2;

			NetworkAPI.SendConfirmation(new GameConfirmation(action.LockstepTurn, wait));
		}

		void OnConfirmation(int playerID, GameConfirmation confirmation) {
			if (confirmation.LockstepTurn < lockstepTurn) { // Discard
				return;
			}

			if (confirmation.LockstepTurn > lockstepTurn + 2) {
				Debug.LogError("Lockstep : Confirmation from player " + playerID + " on impossible turn (> 2)");
				return;
			}

			uint turn = confirmation.LockstepTurn - lockstepTurn;

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
				NetworkUI.Log("Confirmation allready received for LockstepTurn " + confirmation.LockstepTurn);
			}
		}

		void Update() {
			accumulatedTime += Time.deltaTime * 1000f;

			while (accumulatedTime > gameFrameTurnLength) {
				GameFrameTurn();
				accumulatedTime -= gameFrameTurnLength;
			}
		}

		void GameFrameTurn() {
			NetworkUI.ClearLog(); // TEST network ui clear log on game frame turn
			NetworkUI.Log("LockstepTurn : " + lockstepTurn + " ; gameFrame : " + gameFrame);

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

			// SendActionToAll(); // send action to all who haven't confirmed
		}

		void NextTurn() {
			++lockstepTurn;

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

		//
		// Did we receive every client’s action for the next turn ?
		// Did every client confirm that they received our action ?
		//
		bool LockStepTurn() {
			bool allActionsReceived = numberOfPlayerWhoSendAction[0] == numberOfPlayers;
			bool allActionsConfirmed = numberOfPlayerWhoConfirmedMyAction[0] == numberOfPlayers;
			bool readyForTurn = (allActionsReceived && allActionsConfirmed) || lockstepTurn < firstLockstepTurn + 2;

			if (readyForTurn) {
				NetworkUI.Log("Ready for turn " + lockstepTurn);

				SetAction(); // set wantedActions[1]

				if (lockstepTurn >= firstLockstepTurn + 3) {
					ProcessActions();
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
				action.LockstepTurn = lockstepTurn + 2;
				actions[2][NetworkAPI.PlayerId] = action;
			} else {
				actions[2][NetworkAPI.PlayerId] = new NoAction(lockstepTurn + 2);
			}

			NetworkUI.Log("SetAction " + actions[2][NetworkAPI.PlayerId]);

			++numberOfPlayerWhoSendAction[2];

			playerHaveConfirmedMyAction[2][NetworkAPI.PlayerId] = true;
			++numberOfPlayerWhoConfirmedMyAction[2];
		}
	
		void UpdateGame() {
			KRFacade.UpdateGame();
		}

		void OnModelSelectDemand(Vec2 modelPosition) {
			// TODO check if the last action is of the same type,
			// if true, override the previous with it, else, enqueue it
			actionQueue.Enqueue(new SelectAction(lockstepTurn, modelPosition));
		}

		void OnModelMoveDemand(Vec2 modelPosition) {
			actionQueue.Enqueue(new MoveAction(lockstepTurn, modelPosition));
		}

		void OnModelAttackDemand(Vec2 modelPosition) {
	        actionQueue.Enqueue(new AttackAction(lockstepTurn, modelPosition));
	    }

		void OnModelDragDemand(Vec2 beginModelPosition, Vec2 endModelPosition, Vec2 z) {
			actionQueue.Enqueue(new DragAction(lockstepTurn, beginModelPosition, endModelPosition, z));
		}

		void OnModelSpawnDemand(KeyCode k) {
			actionQueue.Enqueue(new SpawnAction(lockstepTurn, k));
		}
	}

}