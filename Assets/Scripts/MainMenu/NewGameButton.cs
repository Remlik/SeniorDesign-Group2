﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameButton : MonoBehaviour {

	public MainMenuBehavior mainMenuBehavior;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown()
    {
		mainMenuBehavior.NewGameClicked();
    }
}
