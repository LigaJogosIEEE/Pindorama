using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopySkill : MonoBehaviour {
    public int damage;
    public float speed, cd, forca;
    public float TTL;

    private Animator myAnimator;
    private bool start;
    private float currentTime, direction;
    private SpriteRenderer mySpriteRenderer;

	// Use this for initialization
	void Start () {
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        currentTime += Time.deltaTime;
        if (currentTime >= cd) {
            //if(!myAnimator.GetBool("ATTACK"))
            //    myAnimator.SetBool("ATTACK", true);
            //else
            transform.Translate(direction * speed * Time.deltaTime, 0, 0);
        }
        if (currentTime >= TTL)
            Destroy(gameObject);
	}

    public void setParameters(int damage, float speed, float cd) {
        TTL += cd;
        currentTime = 0;
        this.damage = damage;
        this.speed = speed;
        this.cd = cd;
       // myAnimator.SetBool("START", true);
       if(!mySpriteRenderer)
            mySpriteRenderer = GetComponent<SpriteRenderer>();
        direction = mySpriteRenderer.flipX ? -1 : 1;
    }


    void OnTriggerEnter2D(Collider2D c) {
        string tag = c.gameObject.tag;
        if (tag == "Player") {
            GameObject p = c.gameObject;
            p.GetComponent<Player>().dano(damage);
            if(mySpriteRenderer.flipX)
                p.GetComponent<Rigidbody2D>().AddForce(Vector2.left * forca, ForceMode2D.Impulse);
            else
                p.GetComponent<Rigidbody2D>().AddForce(Vector2.right * forca, ForceMode2D.Impulse);

            gameObject.GetComponent<Collider2D>().enabled = false;
        }
        else if (tag == "Parede")
            gameObject.SetActive(false);
    }
}
