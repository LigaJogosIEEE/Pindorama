using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryObstacle : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Player") {
			Debug.Log ("Player Collided with Obstacle");
			other.transform.GetComponent<PlayerCharacter> ().TakeObstacleDamage ();
		}
	}
}
