using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCharacter : MonoBehaviour {
    public int points;

    public float jumpForce;

    //Posicoes das vias
    public float[] lanePositions;

    //Objetos das Vias
    public Transform[] lanes;

    //Tempo de espera para o jogador trocar de via
    public float DefaultLaneChangeDelay;

    //Gerenciador de Animacoes do objeto
    private Animator animator;
    
    //Corpo Rigido do Objeto
    private Rigidbody2D characterBody;

    //Este objeto está tocando no chao
    private bool grounded;

    //Via atual do jogador
    private int currentLane;

    //Tempo de espera para o jogador trocar de via
    private float LaneUpdateDelay;

	//Status de Invencivel 
	private bool invencible;

	//Vida do Personagem Atual
    [SerializeField]
	private int health;

	//Vida maxima e inicial do personagem
	public int maxHealth;

    void Start () {
        //Obtem o animator
        animator = GetComponent<Animator> ();
        characterBody = GetComponent<Rigidbody2D> ();
        currentLane = 0;
		health = maxHealth;
        UpdateLanePosition ();
    }

    void Update () {
        //Incrementa ou decrementa as variaveis dependentes de tempo
        Counting ();

        if (grounded) {
            //Se o personagem esta no chao ele pode receber comandos do jogador
            ControlInputs ();
        }
    }

    //Toma acoes baseadas nos botao que o jogador apertar
    private void ControlInputs() {
        if (Input.GetKeyDown (KeyCode.Space)) {
            Jump ();
        } else if (Input.GetKeyDown(KeyCode.UpArrow) && LaneUpdateDelay == 0) {
            GoToUpperLane ();
        } else if (Input.GetKeyDown(KeyCode.DownArrow) && LaneUpdateDelay == 0) {
            GoToLowerLane ();
        }
    }

    private void Counting () {
        if (LaneUpdateDelay > 0) {
            LaneUpdateDelay -= Time.deltaTime / DefaultLaneChangeDelay;
            if (LaneUpdateDelay < 0)
                LaneUpdateDelay = 0;
        }
    }

    private void Jump () {
        characterBody.AddForce (Vector2.up * jumpForce, ForceMode2D.Impulse);
        animator.SetTrigger ("Jump");
    }

    private void GoToUpperLane () {
        if (currentLane < lanePositions.Length - 1) {
            currentLane++;
            UpdateLanePosition ();
            LaneUpdateDelay = DefaultLaneChangeDelay;
        }
    }

    private void GoToLowerLane () {
        if (currentLane > 0) {
            currentLane--;
            UpdateLanePosition ();
            LaneUpdateDelay = DefaultLaneChangeDelay;
        }
    }

    private void UpdateLanePosition () {
        Vector3 position = transform.position;
        position.y = lanePositions[currentLane];
        transform.position = position;

        /*foreach (Transform transform in lanes) {
            transform.GetComponent<BoxCollider2D> ().enabled = false;
        }*/

        lanes[currentLane].GetComponent<BoxCollider2D> ().enabled = true;
    }

    private void OnCollisionEnter2D (Collision2D collision) {
        //Caso ele toque no chao, modifica a flag para verdadeiro
        if (collision.gameObject.tag == "Ground") {
            grounded = true;
            animator.SetBool ("Grounded", grounded);
        }
    }

    private void OnCollisionExit2D (Collision2D collision) {
        //Caso ele saia do chao, modifica a flag para falsi
        if (collision.gameObject.tag == "Ground") {
            grounded = false;
            animator.SetBool ("Grounded", grounded);
        }
    }

	public void TakeObstacleDamage () {
		if (invencible)
			return;
		
		Vector3 position = transform.position;
		position.x -= 1.5f;
		transform.position = position;
		StartCoroutine (InvencibleStats(2f));

		TakeDamage (1);
	}

	public void TakeDamage (int amount) {
		health -= amount;
		if (health <= 0)
			Die ();
	}

	private void Die () {
		Debug.Log ("Player Died");
		if (!GameController.get ()) {
			Debug.Log ("There's no Game Controller on the Scene!!! Create one");
			return;
		}

        Time.timeScale = 0;

		GameController.get ().OnPlayerDeath ();
	}

	private IEnumerator InvencibleStats (float invencibleTime) {
		invencible = true;
		float timeElapsed = 0.0f;
		bool current = true;
		Color start = GetComponent<SpriteRenderer> ().color;
        Color aux = start;
        while (timeElapsed < invencibleTime) {
            if (current)
                aux.a = .6f;
            else
                aux.a = start.a;
			GetComponent<SpriteRenderer>().color = aux;

			current = !current;
			timeElapsed += 0.3f;
			yield return new WaitForSeconds (0.3f);
		}

		GetComponent<SpriteRenderer>().color = start;
		invencible = false;
	}

    public void IncreasePoints(int points) {
        this.points += points;
    }

    public void UseCollectable (Collectables.CollectableEffect effect) {
        switch (effect.effect) {
            case Collectables.Effect.POINTS:
                IncreasePoints (effect.value);
                break;
            case Collectables.Effect.DECREASE_POINTS:
                IncreasePoints (effect.value * -1);
                break;
            case Collectables.Effect.RESTORE_HP:
                TakeDamage (effect.value * -1);
                break;
            case Collectables.Effect.INVULNERABILITY:
                StartCoroutine (InvencibleStats (effect.value));
                break;
        }

        Debug.Log ("Collected: " + effect);
    }
}
