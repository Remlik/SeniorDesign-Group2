﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Author: Greg Kilmer
 * Last Updated: 2/28/2018
 * */

public class ViveControllerGrabObject : MonoBehaviour {

	public Transform snapPoint;

	private SteamVR_TrackedObject trackedObj;
	private GameObject collidingObject; 
	private GameObject objectInHand; 

	private SteamVR_Controller.Device Controller
	{
		get { return SteamVR_Controller.Input((int)trackedObj.index); }
	}

	void Awake()
	{
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}

	private void SetCollidingObject(Collider col)
	{
		if (collidingObject || !col.GetComponent<Rigidbody>())
		{
			return;
		}
		if (collidingObject.GetComponent<OutlineMirrorObject> () != null) {
			collidingObject.GetComponent<OutlineMirrorObject> ().activeOutlines = true;
		}
		collidingObject = col.gameObject;
	}

	public void OnTriggerEnter(Collider other)
	{
		SetCollidingObject(other);
	}

	public void OnTriggerStay(Collider other)
	{
		SetCollidingObject(other);
	}

	public void OnTriggerExit(Collider other)
	{
		if (!collidingObject)
		{
			return;
		} else if (collidingObject.GetComponent<OutlineMirrorObject> () != null) {
			collidingObject.GetComponent<OutlineMirrorObject> ().activeOutlines = false;
		}
		collidingObject = null;
	}

	private void GrabObject()
	{
		if (collidingObject != null && collidingObject.GetComponent<OutlineMirrorObject> () != null) {
			collidingObject.GetComponent<OutlineMirrorObject> ().activeOutlines = false;
		}
		objectInHand = collidingObject;
		collidingObject = null;
		if (objectInHand.tag != "CannotGrab") {
			ItemUse itemUse = objectInHand.GetComponent<ItemUse> ();
			if (itemUse != null) {
				AnchorObject (itemUse);
			}
			var joint = AddFixedJoint ();
			joint.connectedBody = objectInHand.GetComponent<Rigidbody> ();
			objectInHand.GetComponent<Collider> ().enabled = false;
		}
	}

	private FixedJoint AddFixedJoint()
	{
		FixedJoint fx = gameObject.AddComponent<FixedJoint>();
		fx.breakForce = 20000;
		fx.breakTorque = 20000;
		return fx;
	}

	private void AnchorObject(ItemUse itemUse) {
//		Vector3 obj_pos = objectInHand.transform.position;
//		Vector3 anchor_pos = itemUse.GetAnchorPoint ().position;
//		Vector3 difference = anchor_pos - obj_pos;
//		Vector3 target = transform.position;// - difference;
//		Quaternion rotation = transform.rotation;
//		objectInHand.transform.position = target;
//		objectInHand.transform.rotation = Quaternion.Euler (rotation.eulerAngles - itemUse.GetAnchorEulerAngles());
//


		Quaternion rotation = transform.rotation;
		objectInHand.transform.rotation = Quaternion.Euler (rotation.eulerAngles - itemUse.GetAnchorEulerAngles());

		Vector3 obj_pos = objectInHand.transform.position;
		Vector3 anchor_pos = itemUse.GetAnchorPoint ().position;
		Vector3 target_pos = snapPoint.position;
		Vector3 anchor_2_obj = obj_pos - anchor_pos;
		Vector3 obj_2_target = target_pos - obj_pos;
		objectInHand.transform.position = obj_pos + anchor_2_obj + obj_2_target;

	}

	private void ReleaseObject()
	{
		if (GetComponent<FixedJoint>())
		{
			GetComponent<FixedJoint>().connectedBody = null;
			Destroy(GetComponent<FixedJoint>());
			objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
			objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
			objectInHand.GetComponent<Collider> ().enabled = true;
		}
		objectInHand = null;
	}
	
	// Update is called once per frame
	void Update () {
		if (Controller.GetPressDown (SteamVR_Controller.ButtonMask.Grip))
		{
			if (collidingObject)
			{
				GrabObject();
			}
		}

		if (Controller.GetPressUp (SteamVR_Controller.ButtonMask.Grip))
		{
			if (objectInHand)
			{
				ReleaseObject();
			}
		}

		if (Controller.GetHairTriggerDown() && objectInHand != null)
		{
			ItemUse itemUse = objectInHand.GetComponent<ItemUse> ();
			if (itemUse != null) {
				itemUse.Use ();
				Controller.TriggerHapticPulse (durationMicroSec: 1000);
			}
		}
	}
}
