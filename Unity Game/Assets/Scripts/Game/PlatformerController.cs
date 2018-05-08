using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformerController : MonoBehaviour {
    [SerializeField] private float m_MovementScale;
    [SerializeField] private float m_FieldOfView;

    private Transform m_Camera;
    private Transform m_LastChild;

    private bool m_LastPartReached;

    // Use this for initialization
    void Start () {
        m_LastPartReached = false;
        m_Camera = Camera.main.transform;
        ReordenarSubFilhos(transform);
        m_LastChild = GetLastChild();
    }

    // Update is called once per frame
    void Update() {
        transform.Translate(new Vector2(m_MovementScale, 0) * Time.deltaTime);

        if (m_Camera.position.x + m_FieldOfView > m_LastChild.position.x - m_FieldOfView && !m_LastPartReached) {
            Debug.Log("We are on the last part of the platformer...\nPrepare to Restart");
            m_LastPartReached = true;
            CriadorFase.get().SetNewGameMode(CriadorFase.GameMode.RUNNER);
        }
    }

    private void ReordenarSubFilhos(Transform filho) {
        for (int i = 0; i < filho.childCount / 2; i++) {
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

    private Transform GetLastChild() {
        float max = transform.GetChild(0).localPosition.x;
        int index = 0;

        for (int i = 0; i < transform.childCount; i++) {
            if (max <= transform.GetChild(i).localPosition.x) {
                index = i;
                max = transform.GetChild(i).localPosition.x;
            }
        }

        return transform.GetChild(index);
    }
}
