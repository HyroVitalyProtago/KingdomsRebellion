using System;
using System.Diagnostics;
using UnityEngine;

public class ConfirmedActions {

	private bool[] confirmedCurrent;
	private bool[] confirmedPrior;

	private int confirmedCurrentCount;
	private int confirmedPriorCount;

	//Stop watches used to adjust lockstep turn length
	private Stopwatch currentSW;
	private Stopwatch priorSW;

	private LockStepManager lsm;

	public ConfirmedActions(LockStepManager lsm) {
		this.lsm = lsm;
		confirmedCurrent = new bool[lsm.numberOfPlayers];
		confirmedPrior = new bool[lsm.numberOfPlayers];

		ResetArray(confirmedCurrent);
		ResetArray (confirmedPrior);

		confirmedCurrentCount = 0;
		confirmedPriorCount = 0;

		currentSW = new Stopwatch();
		priorSW = new Stopwatch();
	}

	public int GetPriorTime() {
		return ((int)priorSW.ElapsedMilliseconds);
	}

	public void StartTimer() {
		currentSW.Start ();
	}

	public void NextTurn() {
		//clear prior actions
		ResetArray(confirmedPrior);

		bool[] swap = confirmedPrior;
		Stopwatch swapSW = priorSW;

		//last turns actions is now this turns prior actions
		confirmedPrior = confirmedCurrent;
		confirmedPriorCount = confirmedCurrentCount;
		priorSW = currentSW;

		//set this turns confirmation actions to the empty array
		confirmedCurrent = swap;
		confirmedCurrentCount = 0;
		currentSW = swapSW;
		currentSW.Reset();
	}

	public void ConfirmAction(int confirmingPlayerID, int currentLockStepTurn, int confirmedActionLockStepTurn) {
		if(confirmedActionLockStepTurn == currentLockStepTurn) {
			//if current turn, add to the current Turn Confirmation
			confirmedCurrent[confirmingPlayerID] = true;
			confirmedCurrentCount++;
			//if we recieved the last confirmation, stop timer
			//this gives us the length of the longest roundtrip message
			if(confirmedCurrentCount == lsm.numberOfPlayers) {
				currentSW.Stop ();
			}
		} else if(confirmedActionLockStepTurn == currentLockStepTurn -1) {
			//if confirmation for prior turn, add to the prior turn confirmation
			confirmedPrior[confirmingPlayerID] = true;
			confirmedPriorCount++;
			//if we recieved the last confirmation, stop timer
			//this gives us the length of the longest roundtrip message
			if(confirmedPriorCount == lsm.numberOfPlayers) {
				priorSW.Stop ();
			}
		} else {
			// TODO: Error Handling
			UnityEngine.Debug.Log("WARNING!!!! Unexpected lockstepID Confirmed : " + confirmedActionLockStepTurn + " from player: " + confirmingPlayerID);
		}
	}

	public bool ReadyForNextTurn() {
		//check that the action that is going to be processed has been confirmed
		if(confirmedPriorCount == lsm.numberOfPlayers) {
			return true;
		}
		//if 2nd turn, check that the 1st turns action has been confirmed
		if(lsm.LockStepTurnID == LockStepManager.FirstLockStepTurnID + 1) {
			return confirmedCurrentCount == lsm.numberOfPlayers;
		}
		//no action has been sent out prior to the first turn
		if(lsm.LockStepTurnID == LockStepManager.FirstLockStepTurnID) {
			return true;
		}
		//if none of the conditions have been met, return false
		return false;
	}

	public int[] WhosNotConfirmed() {
		//check that the action that is going to be processed has been confirmed
		if(confirmedPriorCount == lsm.numberOfPlayers) {
			return null;
		}
		//if 2nd turn, check that the 1st turns action has been confirmed
		if(lsm.LockStepTurnID == LockStepManager.FirstLockStepTurnID + 1) {
			if(confirmedCurrentCount == lsm.numberOfPlayers) {
				return null;
			} else {
				return WhosNotConfirmed (confirmedCurrent, confirmedCurrentCount);
			}
		}
		//no action has been sent out prior to the first turn
		if(lsm.LockStepTurnID == LockStepManager.FirstLockStepTurnID) {
			return null;
		}

		return WhosNotConfirmed (confirmedPrior, confirmedPriorCount);
	}

	/// <summary>
	/// Returns an array of player IDs of those players who have not
	/// confirmed are prior action.
	/// </summary>
	/// <returns>An array of not confirmed player IDs, or null if all players have confirmed</returns>
	private int[] WhosNotConfirmed(bool[] confirmed, int confirmedCount) {
		if(confirmedCount < lsm.numberOfPlayers) {
			//the number of "not confirmed" is the number of players minus the number of "confirmed"
			int[] notConfirmed = new int[lsm.numberOfPlayers - confirmedCount];
			int count = 0;
			//loop through each player and see who has not confirmed
			for(int playerID = 0; playerID < lsm.numberOfPlayers; playerID++) {
				if(!confirmed[playerID]) {
					//add "not confirmed" player ID to the array
					notConfirmed[count] = playerID;
					count++;
				}
			}

			return notConfirmed;
		} else {
			return null;
		}
	}

	/// <summary>
	/// Sets every element of the boolean array to false
	/// </summary>
	/// <param name="a">The array to reset</param>
	private void ResetArray(bool[] a) {
		for(int i=0; i<a.Length; i++) {
			a[i] = false;
		}
	}
}