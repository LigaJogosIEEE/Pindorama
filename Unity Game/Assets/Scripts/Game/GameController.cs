using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	public static GameController get () {
		return Instance;
	}

	private static GameController Instance;

    public Transform end;
    public PlayerCharacter player;

    public Image progressImage;
    public Image blurPanel;
    public Text dieText;

    private float totalDistance;

	void Awake () {
		if (Instance == null)
			Instance = this;
	}

    // Use this for initialization
    void Start () {
        Vector3 playerStart = player.transform.position;
        totalDistance = end.position.x - playerStart.x;
        blurPanel.enabled = false;
        dieText.enabled = true;
        dieText.text = "";
    }
	
    // Update is called once per frame
    void Update () {
        PublishLevelProgress ();
    }

    private void PublishLevelProgress () {
        float currentDistance = Mathf.Abs(player.transform.position.x - end.position.x);
        float progress = ((totalDistance - currentDistance) / totalDistance) * 770;
        Vector3 position = progressImage.rectTransform.position;
        position.x = progress + 28;
        progressImage.rectTransform.position = position;
    }

	public void OnPlayerDeath () {
        blurPanel.enabled = true;
        StartCoroutine (OnDieText());
	}

    public IEnumerator OnDieText () {
        Debug.Log ("Keppo");
        yield return new WaitForSecondsRealtime (2f);
        Debug.Log ("Kappa");
        dieText.text = "Você Morreu";
    }
}
