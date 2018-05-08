using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriadorFase : MonoBehaviour {
    //Objeto unico para acesso entre Objetos
    private static CriadorFase Instance;

    //Retorna a Instancia do Criador de Fases
    public static CriadorFase get() {
        return Instance;
    }

    [SerializeField] private Elemento m_SignNextGameMode;
    [Tooltip("Numero maximo de Scrolls da fase. Basicamente isso e o tamanho maximo da fase")]
    [SerializeField] private uint m_MaxNumberOfScrolls;
    //TODO: Implementar algo que facilite a criacao de modos de jogo
    [Tooltip("Diz se a fase tem ou nao modos de jogo diferentes.")]
    [SerializeField] private bool m_HasGameModeChange;
    [Tooltip("Numero de vezes que deve ocorrer o Scroll para trocar o modo de jogo. Valores <= 0 significa aleatorio")]
    [SerializeField] private int m_GameModeChangeScrolls;
    [Tooltip("Elementos que devem ser repetidos na fase.")]
    [SerializeField] private Elemento[] m_ElementosRepetidos;
    [Tooltip("Elementos que podem aparecer eventualmente apos um Scroll da fase.")]
    [SerializeField] private ScrollElemento[] m_ElementosPorScroll;

    [SerializeField] private Elemento m_PlatformerGameMode;
    
    //Variavel utilizada para impedir varios "OnScroll" no mesmo frame
    private float m_ScrollTime = 0;
    //Lista de Objetos instanciados na fase
    private List<Transform> m_InstantiatedObjects = new List<Transform>();

    //Numero de vezes que a fase foi chamou Scroll
    private uint m_ScrollNumber;

    //Modo de jogo atual
    private GameMode m_CurrentMode;

    //Indicador que faltam 1 scroll's para comecar a brincadeira
    private bool m_NextIsNewGameMode;

    //Instancia do modo plataformer
    private Transform m_PlatformerInstance;

    // Chamado na inicializacao da fase
    public void Awake() {
        if (Instance == null)
            Instance = this;
    }

    // Inicio do Jogo (Ou do objeto)
    // Tem muita carga no inicio do jogo, isso pode acabar sendo ruim para o load inicial (instanciando mts objetos)
    void Start () {
        m_CurrentMode = GameMode.RUNNER;
        m_ScrollNumber = 0;
        //Coloca na fase os objetos base da fase
        foreach (Elemento elemento in m_ElementosRepetidos) {
            //Objetos sao adicionados na lista para caso sejam necessarios depois
            m_InstantiatedObjects.Add(Instantiate(elemento.transform, elemento.localizacao, Quaternion.identity));
        }

        if (m_GameModeChangeScrolls <= 0 && m_HasGameModeChange) {
            //Ter sempre certeza que temos mais que 10 scrolls por fase
            float randomValue = Random.Range(m_MaxNumberOfScrolls / 2 - 3, m_MaxNumberOfScrolls / 2 + 3);
            m_GameModeChangeScrolls = Mathf.CeilToInt(randomValue);
        }

        m_PlatformerInstance = Instantiate(m_PlatformerGameMode.transform, m_PlatformerGameMode.localizacao, Quaternion.identity);
        m_PlatformerInstance.gameObject.SetActive(false);
    }

    // Atualizacao a cada frame
    void Update () {
        m_ScrollTime += Time.deltaTime;
    }

    // A cada scroll, este metodo sera chamado para instanciar os elementos de scroll.
    public void OnScroll(Transform transform, Parallax scrolled) {
        if (scrolled.GetParallaxType() == Parallax.ParallaxType.FLOOR) {
            if (m_ScrollTime >= 0.5f) {
                m_ScrollTime = 0;

                Debug.Log("On Scroll!");
                m_ScrollNumber++;


                if (m_NextIsNewGameMode) {
                    ChangeGameMode(scrolled);
                    m_NextIsNewGameMode = false;
                }

                if (m_HasGameModeChange)
                    CheckGameModeState();

            }
        }

        if (scrolled.GetAssociatedGameMode() != m_CurrentMode) {
            transform.gameObject.SetActive(false);
        } else {
            transform.gameObject.SetActive(true);
        }
    }

    private void CheckGameModeState() {
        if (m_ScrollNumber != m_GameModeChangeScrolls)
            return;

        m_NextIsNewGameMode = true;
        Debug.Log("Preparing to Change Gamemode...");
        
        if (m_SignNextGameMode.transform) {
            Instantiate(m_SignNextGameMode.transform, m_SignNextGameMode.localizacao, Quaternion.identity);
        }

    }

    private void ChangeGameMode(Parallax scrolled) {
        Debug.Log("GameMode Changed!");
        m_CurrentMode = GameMode.PLATFORMER;

        m_PlatformerInstance.gameObject.SetActive(true);
    }

    public void SetNewGameMode(GameMode gameMode) {
        m_CurrentMode = gameMode;
    }

    [System.Serializable]
    //Estrutura base para cada elemento da fase
    public struct Elemento {
        public Transform transform;
        public Vector3 localizacao;
    }

    [System.Serializable]
    //Estrutura base para cada elemento que contem uma chance de aparecer na fase
    public struct ScrollElemento {
        public Transform transform;
        public int minEixoZ, maxEixoZ;
        public Vector2 localizacao;
        public float chance;
        public int maximo;
    }

    //Modos de jogo existentes
    public enum GameMode {
        RUNNER = 1,
        PLATFORMER = 2,
    }
}
