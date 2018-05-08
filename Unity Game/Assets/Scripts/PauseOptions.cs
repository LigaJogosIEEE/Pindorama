using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseOptions : MonoBehaviour {
	public Material material;

	public bool paused;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("escape")) {
			paused = !paused;
			if (paused)
				Time.timeScale = 0.0f;
			else
				Time.timeScale = 1.0f;
		}
	}

	void OnGUI() {
		if (!paused)
			return;

        GUI.Box (new Rect (Screen.width / 2 - 100, Screen.height / 2 - 150, 250, 250), "Paused");
        GUI.Button (new Rect (Screen.width / 2 - 100, Screen.height / 2 - 100, 250, 50), "Test Button");
	}
}
