﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayerDetectionNearby : MonoBehaviour {

	public EnemySight es;

	//private ObjectStrangeLocation objToInvestigate;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider col) {
		if (col.tag == "Player"  && !es.seesPlayer) {
			es.PlayerNearby (col.ClosestPoint(transform.position));
		}
	}

	void OnTriggerStay(Collider col) {
		if (col.tag == "Player"  && !es.seesPlayer) {
			es.PlayerNearby (col.ClosestPoint(transform.position));
		}
	}
}
