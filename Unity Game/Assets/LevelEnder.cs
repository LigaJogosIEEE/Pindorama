using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnder : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D Other) {
        if (Other.tag == "Player") {
            SceneManager.LoadSceneAsync (1);
            SceneManager.LoadSceneAsync (0);
        }
    }
}
