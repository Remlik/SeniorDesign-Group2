﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNearby : MonoBehaviour {

	public List<GameObject> nearbyAllies;
	public int aloneThreshold = 5;
	public bool isAlone;
	public float friendShareDistance = 3;
	private float waitTime;
	private float count;

	// Use this for initialization
	void Start () {
		nearbyAllies = new List<GameObject> ();
		waitTime = Random.Range (3f, 6f);
	}
	
	// Update is called once per frame
	void Update () {
		isAlone = nearbyAllies.Count >= aloneThreshold;
	}

	void FixedUpdate() {
		if (count >= waitTime) {
			GetComponent<SphereCollider> ().enabled = true;
			StartCoroutine (KillCollider (0.05f));
		} else {
			count += Time.fixedDeltaTime;
		}
	}

	private IEnumerator KillCollider(float time) {
		yield return new WaitForSeconds (time);
		GetComponent<SphereCollider> ().enabled = false;
		count = 0;
	}

	public void Call(Vector3 target) {
		foreach (GameObject ally in nearbyAllies) {
			ally.GetComponent<EnemyAI>().CalledForBackup (target);
		}
	}

	public void CallFirst(Vector3 target) {
		if (nearbyAllies.Count > 0) {
			nearbyAllies [0].GetComponent<EnemyAI>().CalledForBackup (target);
		}
	}

	public void OnTriggerEnter(Collider col) {
		if (col.tag == "Enemy") {
			if (!nearbyAllies.Contains (col.gameObject) && col.gameObject != this.gameObject) {
				nearbyAllies.Add (col.gameObject);
			}
		}
	}

	public void OnTriggerExit(Collider col) {
		if (col.tag == "Enemy") {
			nearbyAllies.Remove(col.gameObject);
		}
	}

	public void ShareWithFriends(GameObject sharedTarget) {
		foreach (GameObject ally in nearbyAllies) {
			if (Vector3.Distance (transform.position, ally.transform.position) <= friendShareDistance) {
				ally.GetComponent<EnemyAI>().JoinAttackWithFriends (sharedTarget);
			}
		}
	}
}
