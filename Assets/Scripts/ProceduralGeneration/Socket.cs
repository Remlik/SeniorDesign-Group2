﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socket : MonoBehaviour {
	public static bool timeTrial;
	public enum SocketType {SQUARE,RECTANGLE,ELBOW,ELEVATOR};

	public SocketType socketType;
	public GameObject[] roomOptions;
	public float[] rotationOptions;

	public bool canBeRotated;
	public GameObject room;
	private GameObject projection;

	// Use this for initialization
	void Start () {
		//Random.InitState (GameObject.Find ("LevelGenerator").GetComponent<LevelGeneration> ().seed);
		if (rotationOptions.Length <= 0) {
			canBeRotated = false;
		}

		projection = transform.GetChild (0).gameObject;
		if (projection == null) {
			projection = transform.GetChild (0).gameObject;
		}
		projection.SetActive (false);
		//GenerateRoom ();
	}

	public bool GenerateRoom(int roomInd) {
		if (room == null) {
			if (projection == null) {
				projection = transform.GetChild (0).gameObject;
			}
			projection.SetActive (false);
			GameObject newRoom = Instantiate (roomOptions [roomInd]);
			if (timeTrial && newRoom.tag == "TimeTrial") {
				return GenerateRoom(LevelGeneration.SharedInstance.GetRandomRoomInd(this));
			} else if (newRoom.tag == "TimeTrial") {
				timeTrial = true;
			}
			newRoom.transform.position = transform.position;
			newRoom.transform.parent = transform;
			if (canBeRotated) {
				int rotationInd = Random.Range (0, rotationOptions.Length);
				Vector3 newDir = transform.rotation.eulerAngles;
				newDir.y += rotationOptions [rotationInd];
				Quaternion newRotation = Quaternion.Euler (newDir);
				newRoom.transform.rotation = newRotation;
			} else {
				newRoom.transform.rotation = transform.rotation;
				//newRoom.transform.localScale = new Vector3 (1, 1, 1);
			}
			room = newRoom;
			return true;
		}
		return false;
	}

	public void GenerateDoors() {
		for (int j = 0; j < room.transform.childCount; j++) {
			GameObject child = room.transform.GetChild (j).gameObject;
			if (child.name == "Doors") {
				DoorSpawner[] spawners = child.GetComponentsInChildren<DoorSpawner> ();
				for (int i = 0; i < spawners.Length; i++) {
					spawners [i].SpawnDoor ();
				}
			}
		}
	}

	public void Reset() {
		room.SetActive (false);
		projection.SetActive (true);
	}
}
