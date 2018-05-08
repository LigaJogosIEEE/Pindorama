using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainProjectile : MonoBehaviour {
    public float velocidade;
    public int dano;
    public Vector2 direcao;

    void Start() {
    }

    void Update() {
        if (direcao.y > 1)
            direcao.y = 1;
        else if (direcao.y < -1)
            direcao.y = -1;

        if (direcao.x > 1)
            direcao.x = 1;
        else if (direcao.x < -1)
            direcao.x = -1;

        transform.Translate(direcao * velocidade * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D c) {
        string tag = c.gameObject.tag;
        if (tag == "Player") {
            c.gameObject.GetComponent<Player>().dano(dano);
            dano = 0;
        }
        else if (tag == "Chao")
            gameObject.SetActive(false);
    }
}
