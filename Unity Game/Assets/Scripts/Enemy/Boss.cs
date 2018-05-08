using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour {
    public int currentHP, totalHP;

    public Image hud_hp;
    public GameObject skillRain, skill1PlayerCopy;

    public float speed, safeZone, damageSkill1, castTimeSkill1, speedCopySKill1, distanceCastSkill1;
    public float cdAllSkills, cdSafeZone, cdWalking, timeForWalking;

    [Range(0.01f, 1f)][Tooltip("Parâmetro em porcentagem da vida do boss")]
    public float [] simpleIA;

    public AudioSource soundCastGp, soundCastAzir, soundDamaged;

    private Animator animator;
    private GameObject player;
    private SpriteRenderer mySpriteRender;

    private float currentCdAllSkills, currentCdSafeZone, playerDistance, currentTimeWalking, negDistanceCastSkill1;
    private bool endWalk;

    // Use this for initialization
    void Awake() {
        animator = GetComponent<Animator>();
        currentHP = totalHP;
        mySpriteRender = GetComponent<SpriteRenderer>();
        
        //gambi
        negDistanceCastSkill1 = -1 * distanceCastSkill1;
        setSkillAzirDistance();

        if (player == null)
            searchPlayer();
    }

    private void searchPlayer() {
        //se o player nao existe
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update() {
        //pegando a distancia do player
        playerDistance = player.transform.position.x - transform.position.x;

        IA();
    }

    private void IA() {
        //mover
        Walk();

        if (endWalk) {
            //olhar pro player
            Vision();
            //utilizando as skills
            CastSkills();
        }
    }

    private void leftWalk() {
        mySpriteRender.flipX = true;
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    private void rightWalk() {
        mySpriteRender.flipX = false;
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void Walk() {
        //me afasto do player       
        float AuxPlayerDistance = Vector2.Distance(this.transform.localPosition, player.transform.localPosition);
        //se o player entrou na zona segura
        if (AuxPlayerDistance < safeZone) {
            //inicia o CD
            currentCdSafeZone += Time.deltaTime;
            if (currentCdSafeZone >= cdSafeZone) {
                currentTimeWalking += Time.deltaTime;
                //se o player esta a esquerda, ando pra direita
                if (playerDistance < 0)
                    rightWalk();
                //se o player esta a direita, ando pra esquerda
                else
                    leftWalk();

                //não acabou ainda de andar
                endWalk = false;
            }
        }
        //esta fora da zona de perigo
        else if (currentTimeWalking > timeForWalking) {
            currentTimeWalking = 0;
            currentCdSafeZone = 0;
            setSkillAzirDistance();
            endWalk = true;
        }
    }

    private void Vision() {
        if (playerDistance < 0 && !mySpriteRender.flipX || playerDistance > 0 && mySpriteRender.flipX) {
            mySpriteRender.flipX = !mySpriteRender.flipX;
            distanceCastSkill1 *= -1;
        }
    }

    private void CastSkills() {
        float vida = (float) currentHP / totalHP;

        currentCdAllSkills += Time.deltaTime;
        if (currentCdAllSkills >= cdAllSkills) {
            //resetando o cd de skills
            currentCdAllSkills = 0;

            //verificando qual skill
            int skill = Random.Range(1, 10);            

            //se for 1 utiliza as 2
            if (skill == 1) {
                azirSkill(vida);
                gpSkill(vida);
            }
            //se for menor que 5 utiliza a do azir
            else if (skill <= 5) {
                azirSkill(vida);
            }
            //se for maior que 5 utiliza a do gp
            else {
                gpSkill(vida);
            }
        }
    }

    private void gpSkill(float percentHP) {
        //se não estiver ativo, ativa
        if (!skillRain.activeSelf) {
            SearchTarget script = skillRain.GetComponent<SearchTarget>();
            uint qtd = 0;
            for (int i = 0; i < simpleIA.Length; i++) {
                //quanto menor o HP, mais vezes vai atirar
                if (percentHP <= simpleIA [i])
                    qtd++;
            }
            script.qtdVezes = qtd;
            skillRain.SetActive(true);
        }
    }

    private void azirSkill(float hp) {
        float danoMult, cdMult, velocidadeMult, mult;
        Vector2 pos = new Vector2(transform.position.x + distanceCastSkill1, skill1PlayerCopy.transform.position.y);

        for (int i = 0; i < simpleIA.Length; i++) {
            if (hp <= simpleIA [i]) {
                mult = (1 - simpleIA [i]) + 1;
                //increase speed
                velocidadeMult = speedCopySKill1 * mult;
                //increase damage
                danoMult = damageSkill1 * mult;
                //decrease cast time
                cdMult = castTimeSkill1 * simpleIA [i];
                
                //cast skill
                castSkill1((int) danoMult, cdMult, velocidadeMult, pos);
            }
        }
    }

    private void castSkill1(int damage, float cd, float speed, Vector2 pos) {
        GameObject clone = Instantiate(skill1PlayerCopy, pos, transform.rotation) as GameObject;
        //set position
        clone.GetComponent<SpriteRenderer>().flipX = mySpriteRender.flipX;

        //set atributes
        CopySkill script = clone.GetComponent<CopySkill>();
        script.setParameters(damage, speed, cd);
        clone.SetActive(true);
    }

    public void dano(int quantidade) {
        if (!animator.GetBool("MORTO")) {
            currentHP -= quantidade;
            if (currentHP < 0) {
                animator.SetBool("MORTO", true);
                currentHP = 0;
            }

            atualizaHud(hud_hp);
        }
    }

    private void atualizaHud(Image hud) {
        Vector3 t = hud.transform.localScale;
        t.y = (float) currentHP / totalHP;

        if (t.y > 1)
            t.y = 1;
        else if (t.y < 0)
            t.y = 0;

        hud.transform.localScale = t;
    }

    private void setSkillAzirDistance() {
        if (mySpriteRender.flipX)
            distanceCastSkill1 = negDistanceCastSkill1;
        else
            distanceCastSkill1 = -1 * negDistanceCastSkill1;
    }
}
