using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour {
    [SerializeField] private bool m_ShouldMove;
    [SerializeField] private float m_MovementScale;
    [SerializeField] private float m_FieldOfView;

    private bool onField;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(new Vector2(m_MovementScale, 0) * Time.deltaTime);

        if (transform.position.x < m_FieldOfView + 10) {
            onField = true;
        }

        if (onField && Mathf.Abs(transform.position.x) > m_FieldOfView + 10) {
            enabled = false;
            gameObject.SetActive (false);
        }
	}
}
