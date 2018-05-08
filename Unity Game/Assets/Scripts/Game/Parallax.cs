using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {
    [SerializeField] private bool  m_DeveMover;
    [SerializeField] private float m_EscalaMovimento;
    [SerializeField] private float m_TamanhoPedaco;
    [SerializeField] private float m_CampoDeVisao;
    [SerializeField] private bool m_DeveReordenarSubFilhosNoScroll;
    [SerializeField] private ParallaxType m_ObjectType;
    [SerializeField] private CriadorFase.GameMode m_GameMode;

    //Camera
    private Transform m_Camera;
    //Geralmente tem 3 filhos
    private Transform[] m_Filhos;

    private int m_IndiceEsquerda;
    private int m_IndiceDireita;
    //Posicao X da camera
    private float m_CameraX;

    //Posicao Z do Transform
    private float m_PosicaoEixoZ;

    // Start
    void Start () {
        //Obtem referencia para a camera
        m_Camera = Camera.main.transform;
        //Quantidade de filhos desse objeto
        int qtdFilhos = transform.childCount;
        //Cria o Vetor de filhos
        m_Filhos = new Transform[qtdFilhos];

        for (int i = 0; i < qtdFilhos; i++) {
            //Coloca cada filho no vetor
            m_Filhos[i] = transform.GetChild(i);
        }

        m_IndiceEsquerda = 0;
        m_IndiceDireita = qtdFilhos - 1;
        m_PosicaoEixoZ = transform.position.z;
	}
	
    // Update
	void Update () {
        for (int i = 0; i < m_Filhos.Length; i++) {
            //Move todos os filhos
            m_Filhos[i].transform.Translate(new Vector2(m_EscalaMovimento, 0) * Time.deltaTime);
        }

        if (m_Camera.position.x > m_Filhos[m_IndiceDireita].transform.position.x - m_CampoDeVisao) {
            //Se a camera nao ve mais o objeto, ele pode ser movido para a direita
            Scroll();
            if (m_DeveReordenarSubFilhosNoScroll)
                ReordenarSubFilhos(m_Filhos[m_IndiceDireita]);
        }
	}

    private void Scroll() {
        float posicao_y = m_Filhos[m_IndiceEsquerda].position.y;

        //Move
        m_Filhos[m_IndiceEsquerda].position = new Vector3(m_Filhos[m_IndiceDireita].position.x + m_TamanhoPedaco, posicao_y, m_PosicaoEixoZ);
        Transform moved = m_Filhos[m_IndiceEsquerda];
        //Modifica indices
        m_IndiceDireita = m_IndiceEsquerda;
        m_IndiceEsquerda++;

        if (m_IndiceEsquerda == m_Filhos.Length)
            m_IndiceEsquerda = 0;

        CriadorFase.get().OnScroll(moved, this);
    }

    private void ReordenarSubFilhos(Transform filho) {
        for (int i = 0; i < filho.childCount/2; i++) {
            float f1 = Random.Range(0, filho.childCount - 1) + 0.5f;
            float f2 = Random.Range(0, filho.childCount - 1) + 0.5f;
            int i1 = Mathf.CeilToInt(f1);
            int i2 = Mathf.CeilToInt(f2);

            Transform o1 = filho.GetChild(i1);
            Transform o2 = filho.GetChild(i2);

            Vector3 pos1 = o1.position;
            Vector3 pos2 = o2.position;

            o2.position = pos1;
            o1.position = pos2;
        }
    }

    public ParallaxType GetParallaxType () {
        return m_ObjectType;
    }

    public CriadorFase.GameMode GetAssociatedGameMode() {
        return m_GameMode;
    }

    public enum ParallaxType {
        NONE = 0,
        FLOOR = 1,
        ENVIROMENT = 2,
    }
}
