using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    public int vidaAtual;
    public int vidaTotal;
    public float velocidade, forcaPulo, maxSalto;
    public Collider2D pes;

    public Image hud_vida;

    private SpriteRenderer meuSprite;
    private Rigidbody2D meuCorpo;
    private Animator animator;
    private Vector2 maxAltSalto;
    private bool noChao, podeDescer;
   
	// Use this for initialization
	void Start () {
        meuCorpo = gameObject.GetComponent<Rigidbody2D>();
        meuSprite = gameObject.GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        vidaAtual = vidaTotal;
        maxAltSalto = new Vector2(0, maxSalto);
    }
	
	// Update is called once per frame
	void Update () {
        movimentacao();
    }

    void FixedUpdate() {
        if (Input.GetKey(KeyCode.UpArrow))
            pular();

        if (Input.GetKey(KeyCode.DownArrow)) {
            StartCoroutine(descer());
            StopCoroutine(descer());                       
        }
    }

    private void movimentacao() {
        if (Input.GetKey(KeyCode.LeftArrow))
            andarEsquerda();

        if (Input.GetKey(KeyCode.RightArrow))
            andarDireita();
    }

    private void andarEsquerda() {
        meuSprite.flipX = true;
        transform.Translate(Vector2.left * velocidade * Time.deltaTime);
    }

    private void andarDireita() {
        meuSprite.flipX = false;
        transform.Translate(Vector2.right * velocidade * Time.deltaTime);
    }

    private void pular() {
        if (noChao) {
            meuCorpo.AddRelativeForce(Vector2.up * forcaPulo, ForceMode2D.Impulse);
            if (meuCorpo.velocity.y > maxSalto) {
                maxAltSalto.x = meuCorpo.velocity.x;
                meuCorpo.velocity = maxAltSalto;                   
            }
        }
    }

    private IEnumerator descer() {
        if (podeDescer) {
            pes.isTrigger = true;
            yield return new WaitForSeconds(0.3f);
            pes.isTrigger = false;
        }
    }

    public void curar(int quantidade){
        //se não está morto pode se curar
        if (!animator.GetBool("MORTO")) { 
            vidaAtual += quantidade;
            if (vidaAtual > vidaTotal)
                vidaAtual = vidaTotal;

            atualizaHud(hud_vida);
        }
    }

    public void dano(int quantidade) {
        if (!animator.GetBool("MORTO")) {
            vidaAtual -= quantidade;
            if (vidaAtual < 0) {
                animator.SetBool("MORTO", true);
                vidaAtual = 0;
            }

            atualizaHud(hud_vida);
        }
    }

    void OnCollisionExit2D(Collision2D colidiu) {
        if (colidiu.gameObject.tag == "Chao")
            noChao = false;
        if (colidiu.gameObject.tag == "Plataforma") {
            noChao = false;
            podeDescer = false;
        }
    }

    void OnCollisionEnter2D (Collision2D colidiu) {
        if (colidiu.gameObject.tag == "Chao")
            noChao = true;
        if (colidiu.gameObject.tag == "Plataforma") {
            podeDescer = true;
            noChao = true;
        }
    }

    private void atualizaHud(Image hud) {
        Vector3 t = hud.transform.localScale;
        t.x = (float) vidaAtual / vidaTotal;

        if (t.x > 1)
            t.x = 1;
        else if (t.x < 0)
            t.x = 0;

        hud.transform.localScale = t;
    }
}
