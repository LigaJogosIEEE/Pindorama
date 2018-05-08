using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SearchTarget : MonoBehaviour {
    public GameObject player;
    public GameObject [] munitions;
    public float topoFase;
    public float velocidade;
    public float cd;
    public uint qtdVezes;

    public Vector2 minSize;

    //auxiliares
    private Vector2 playerPosition, myPosition, posMunition, initalScale;
    private float contCast, distance;
    private uint qtdLancado, qtdMunition, velLancar;

    // Use this for initialization
    private void Start() {
        qtdLancado = 0;
        velLancar = 0;
        myPosition = transform.position;
        //cria um vector2 para servir de base para posição das munições
        posMunition = new Vector2();
        //salva a escala inicial da mira
        initalScale = transform.localScale;
    }

    private void Awake() {
        //se o player nao existe
        if (player == null)
            achaPlayer();
    }

    public void setParameters(GameObject player, float velocidade, int qtdVezes) {
        this.velocidade = velocidade;
        this.qtdVezes = (uint) qtdVezes;
        this.player = player;
    }

    /// <summary>
    /// Método para busca do personagem com a tag player
    /// </summary>
    private void achaPlayer() {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update() {
        //se não tiver nenhum player, busca um
        if (player == null)
            achaPlayer();

        //se castei menos do que deveria
        if (qtdLancado < qtdVezes) {
            //auxiliar para posição atual da mira
            myPosition = transform.position;
            //seta a variavel referente a posicao do player
            playerPosition = player.transform.position;

            //movimenta em direção ao player
            transform.position = Vector2.LerpUnclamped(myPosition, playerPosition, Time.deltaTime * velocidade);
        }
        //ja lançou mais do que deveria
        else {
            //me desativo
            qtdLancado = 0;
            this.gameObject.SetActive(false);
        }
    }

    private void tocou() {
        //incrementa o contador de cast
        contCast += Time.deltaTime;
        //definindo tamanho da mira
        transform.localScale = Vector2.Lerp(transform.localScale, minSize, Time.deltaTime * (1 - (contCast / cd)));

        //se o contador de cast for maior ou igual ao cd
        if (contCast >= cd) {
            castSkill();
            qtdLancado++;
            //resetando tamanho da mira após alguns segundos
            transform.localScale = initalScale;
            contCast = 0;
        }
    }

    private void castSkill() {
        //verifico se tem municoes
        if (munitions.Length > 0) {
            //atualizacao da posicao da municao
            posMunition.x = transform.position.x;
            posMunition.y = topoFase;
            //incremento na velocidade de queda
            velLancar++;
            //contador de indice da municao
            qtdMunition++;
            //se o contador for maior ou igual ao tamanho do vetor de municoes
            if (qtdMunition >= munitions.Length)
                qtdMunition = 0;
            //setando a posicao
            munitions [qtdMunition].transform.position = posMunition;
            //incrementando a velocidade
            munitions [qtdMunition].GetComponent<RainProjectile>().velocidade += velLancar;
            //ativando
            munitions [qtdMunition].SetActive(true);
        }
    }

    void OnTriggerStay2D(Collider2D c) {
        string tag = c.gameObject.tag;
        //se o player esta na area de efeito
        if (tag == "Player")
            tocou();
    }
}

