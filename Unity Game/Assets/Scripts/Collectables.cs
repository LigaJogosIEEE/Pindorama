using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectables : MonoBehaviour {
    [SerializeField]
    private CollectableEffect CurrentEffect;

    // Use this for initialization
    void Start () {
		
    }
	
    // Update is called once per frame
    void Update () {
		
    }

    private void OnTriggerEnter2D (Collider2D collision) {
        if (collision.tag == "Player") {
            PlayerCharacter character = collision.GetComponent<PlayerCharacter> ();
            character.UseCollectable (CurrentEffect);
            Destroy (gameObject);
        }
    }

    [System.Serializable]
    public struct CollectableEffect {
        public Effect effect;
        public int value;
    }

    public enum Effect {
        RESTORE_HP, POINTS, INVULNERABILITY, DECREASE_POINTS
    }
}
