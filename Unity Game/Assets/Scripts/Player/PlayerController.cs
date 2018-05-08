using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (CharacterController))]
public class PlayerController : MonoBehaviour {
    [SerializeField] private float m_MaxGravityVelocity;
    [SerializeField] private float m_JumpForce;
    [SerializeField] private float m_Gravity = 1.0f;


    private Animator m_Animator;

    public float m_Offset;

    private float m_VerticalVelocity;

    private CharacterController m_CharacterController;

    // Use this for initialization
    void Start () {
        m_CharacterController = GetComponent<CharacterController> ();
        m_Animator = GetComponent<Animator> ();
        m_MaxGravityVelocity *= -1;
    }

    // Update is called once per frame
    void Update () {
        if (Input.touchCount >= 5) {
            Vector3 pos = transform.position;
            pos.y = -2.33f;
            transform.position = pos;
        }

        if (IsControllerGrounded ()) {
            Debug.Log ("Controller is Grounded!");
            if (m_VerticalVelocity <= 0)
                m_VerticalVelocity = 0;
            InputManager ();
        }
        else {
            m_VerticalVelocity -= m_Gravity * Time.deltaTime;
            if (m_VerticalVelocity < m_MaxGravityVelocity)
                m_VerticalVelocity = m_MaxGravityVelocity;
            Debug.Log ("Controller is Not Grounded!");
        }

        m_CharacterController.Move (new Vector2 (0, m_VerticalVelocity) * Time.deltaTime);
    }

    private void InputManager () {
        if (Input.touchCount > 0 || Input.GetKeyDown (KeyCode.Space)) {

            m_VerticalVelocity = m_JumpForce;
            m_Animator.SetTrigger ("Jump");
        }
    }

    private bool IsControllerGrounded () {
        Vector3 rayStart = m_CharacterController.bounds.center;
        Vector3 rayStartRight = m_CharacterController.bounds.center;
        Vector3 rayStartLeft = m_CharacterController.bounds.center;

        rayStartRight.x += m_CharacterController.bounds.extents.x;
        rayStartLeft.x -= m_CharacterController.bounds.extents.x;

        if (Physics2D.Raycast (rayStart, Vector3.down, (m_CharacterController.height / 2) + m_Offset))
            return true;

        if (Physics2D.Raycast (rayStartRight, Vector3.down, (m_CharacterController.height / 2) + m_Offset))
            return true;

        if (Physics2D.Raycast (rayStartLeft, Vector3.down, (m_CharacterController.height / 2) + m_Offset))
            return true;

        return false;
    }

}
