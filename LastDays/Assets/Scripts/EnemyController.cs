using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : CanHitController
{
    private Animator animator;
    public GameObject player;
    public Slider Healthy;
    public float health = 1;

    private bool isAttacking;
    private bool isDead;
    private bool canAttack;

    private float time;
    private float timeToAttack = 0.5f;

    private float timeFollow;
    private float timeToFollow = 0.08f;
    private bool faceRight = true;

    private Vector3 previousPosition; // keeps the previous position in order to block the movement
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead) return;

        time += Time.deltaTime;

        if(health <= 0) {
            isDead = true;
            //animator.SetBool("isDead", isDead);
            Destroy(this.gameObject);
            return;
        }
        Healthy.value = health;
        
        canAttack = target != null;
        Attack();
        FollowPlayer();
    }

    public void FollowPlayer() {

        if (gameObject.name.Contains("Enemy02")) {
            //enemy 2 always facing player
            if((gameObject.transform.position.x < player.transform.position.x) &&
                faceRight) {
                    ChangeSide();
            } else if ((gameObject.transform.position.x >= player.transform.position.x) &&
                !faceRight) {
                    ChangeSide();
            }
            if (Game.enemyFollowsPlayer) {
                timeFollow += Time.deltaTime;
                moveUsingTransform();
               
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D other)
    {        
        //print("collision:" + other.gameObject.layer);
        //check if is possible to move
        if (other.gameObject.layer == 8) //"BlockingLayer"
        {
            //Debug.Log("Blocking");
            //return the player to the last position
            transform.localPosition = previousPosition;
        }
    }
    private void moveUsingTransform()
    {
        float distance = Vector2.Distance(gameObject.transform.position, player.transform.position);        
        bool FollowPlayer = distance <= 5;
        if ((timeFollow < timeToFollow) || !FollowPlayer) {
            return;
        }
        timeFollow = 0;
        Vector3 direction =  player.transform.position - gameObject.transform.position;

        Vector3 nextPosition = direction * Game.enemySpeed * Time.deltaTime;
        previousPosition = transform.localPosition;  

        transform.localPosition += nextPosition;       

    }

    public void Attack() {
        if (isAttacking) {
            if (time < timeToAttack) {
                return;
            }
            isAttacking = false;
            animator.SetBool("isAttacking", isAttacking);
        }

        if (canAttack)
        {
            //Debug.Log("Attack");
            Hit();
            isAttacking = true;
            animator.SetBool("isAttacking", isAttacking);
            time = 0;
        }
    }

    public override void GetHit(float damage){
        health = health - damage;
    }

    public override void Hit(){
        if (target != null) {
            target.GetHit(Game.DamageRate);
        }  
    }

    public void ChangeSide()
    {
        faceRight = !faceRight;
        
        Vector3 temp = transform.localScale;
        temp.x *= -1;
        transform.localScale = temp;
    }
}
