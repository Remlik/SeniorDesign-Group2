﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour {

	public int damageValue;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == "Player") {
			GameObject.FindGameObjectWithTag ("GameController").GetComponent<PlayerHealth> ().DoDamage (damageValue);
			Destroy (this.gameObject);
		}
		else if(col.gameObject.tag == "Interactible"){
			col.GetComponent<DestructableObject>().takeDamage(damageValue);
		}
	}
}
