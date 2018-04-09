﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {
    public enum State { Default, Investigate, MoveTo, Attack, Doors };
    public WorldState worldState;

    public State cState;
    public State prevState;

    public bool isAlone;
    public bool hasCalled = false;

    public float InvestigatingTime = 1;
    public float MoveToTime = 10;

    public int count = 0;

	//DEBUG
	public GameObject investigate_prefab;

	private Vector3 targetLocation;
	private Vector3 preInvestigateTarget;
	private State statePreInvestigate;
	private EnemyMovment movement;

	void Start(){
		movement = GetComponent<EnemyMovment> ();
        cState = State.Default;
		movement.ReturnToPatrol (); //Start Patroling
        prevState = cState;
	}

	void Update(){
        switch (cState){
			case State.Investigate:
				if (movement.isStopped) { //DO THE INVESTIGATING
					if (count >= InvestigatingTime * 1000) {
						switch (statePreInvestigate) {
							case State.Default:
								ToDefault ();
								break;
							case State.Doors:
								ToDoors ();
								break;
							case State.MoveTo:
								ToMoveTo (preInvestigateTarget);
								break;
							default:
								ToDefault ();
								break;
							}
							count = 0;
					} else {
						count += 1;
					}
				}
                break;
            case State.MoveTo:
				if (movement.isStopped){
					if (count >= MoveToTime) {
						ToDefault ();
						count = 0;
					} else {
						count +=1;
					}
                }
                break;
            default:
                break;
        }

		//DEBUG STUFF
		if (Input.GetMouseButtonDown (0)) {
			Vector3 tempTargetLocation = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			tempTargetLocation.y = 0;
			MajorActivity (tempTargetLocation);
		}

		if (Input.GetMouseButtonDown (1)) {
			Vector3 tempTargetLocation = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			tempTargetLocation.y = 0;
			GameObject invest = Instantiate (investigate_prefab);
			invest.transform.position = tempTargetLocation;
			MinorActivity (tempTargetLocation);
		}
	}

	public void MinorActivity(Vector3 target)
    {
        worldState.MinorActivity();
        switch (cState){
			case State.Investigate:
				ToInvestigate(target);
                break;
			case State.MoveTo:
                break;
			case State.Attack:
				ToMoveTo (target);//targetLocation needs to be set to the player
                break;
			case State.Doors:
				ToInvestigate (target);
                break;
			default:
				ToInvestigate (target);
                break;
        }
    }

	public void MajorActivity(Vector3 target){
        worldState.MajorActivity();

        if(isAlone == true && !hasCalled){
            CallForBackup();
            hasCalled = true;
        }

        switch(cState){
			case State.Investigate:
				ToMoveTo (target);
                break;
			case State.MoveTo:
				ToInvestigate (target);
                break;
			case State.Attack:
				ToInvestigate (target);
                break;
			case State.Doors:
				ToMoveTo (target);
                break;
			default:
				ToMoveTo (target);
                break;
        }
    }

	public void CalledForBackup(Vector3 callerLocation){
		ToMoveTo (callerLocation); //Need to get the callers location
        count = 0;
    }

    public void CallForBackup(){
		Debug.Log ("HELP!");
    }

    public void NearDoor(){
        switch(cState){
            case State.MoveTo:
                prevState = cState;
                cState = State.Doors;
                break;
            default:
                prevState = cState;
                cState = State.Doors;
                break;

        }
        count = 0;
    }

	public void SpottedPlayer(Vector3 playerLocation){
		targetLocation = playerLocation;
		ToAttack ();
    }

	private void ToDefault() {
		if (cState != State.Default) {
			prevState = cState;
			cState = State.Default;
			movement.ReturnToPatrol ();
			count = 0;
		}
	}

	private void ToMoveTo(Vector3 target) {
		if (cState != State.MoveTo) {
			targetLocation = target;
			prevState = cState;
			cState = State.MoveTo;
			movement.MoveTo (targetLocation);
			count = 0;
		}
	}

	private void ToInvestigate(Vector3 target) {
		if (cState != State.Investigate) {
			preInvestigateTarget = targetLocation;
			targetLocation = target;
			prevState = cState;
			statePreInvestigate = prevState;
			cState = State.Investigate;
			movement.InvestigateLocation (targetLocation);
			count = 0;
		} else {
			targetLocation = target;
			movement.InvestigateLocation (targetLocation);
			count = 0;
		}
	}

	private void ToAttack() {
		if (cState != State.Attack) {
			prevState = cState;
			cState = State.Attack;
			count = 0;

		}
	}

	private void ToDoors() {
		if (cState != State.Doors) {
			prevState = cState;
			cState = State.Doors;
			count = 0;

		}
	}

    public State GetState(){
        return cState;
    }
}

