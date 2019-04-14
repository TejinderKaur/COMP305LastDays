using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CanHitController
{
    // PUBLIC VARIABLES
    public InventoryController inventoryController;
    public Sprite spriteBoard;
    //using SpriteRenderer in order to know where to place the player in the board

    // PRIVATE VARIABLES
    private Animator animator;
    private SpriteRenderer sprite;
    private bool faceRight = true;
    private Rigidbody2D rBody;
    private const float moveRate = 0.05f;//time to move in secs

    private float timeToMove;//time to move in secs
    private float timeToAttack = 0.05f;
    private float timeToRest = 0.5f;
    private float restingTime = 0f;

    private float timeToWarm = 0.05f;
    private float warmingTime = 0f;

    private bool isAttacking;
    private bool isDead;
    private float timeToDie = 3f;

    private float time;//time in game
    private Vector3 previousPosition; // keeps the previous position in order to block the movement
    private float DistanceWalked;

    public float delayRate = 1;
    public float speed = 3;
    
    public delegate void Action(InventoryController inventory);

    public Action EndLevel;
    public Action GameOver;

    private float timeAtack = 0;

    public SoundManager gameSound;

    public AttackController attackController;
    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        //in the case of unity doesn't have a reference for the sprite
        if (spriteBoard == null) {
            spriteBoard = sprite.sprite;
        }
        DistanceWalked = 0;
        timeToMove = moveRate;
        timeAtack = timeToAttack;

    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// Use FixedUpdate for Physics-based movement only
    /// </summary>
    void FixedUpdate()
    {
        time += Time.deltaTime;

        CheckPlayerHealth();
        if (isDead) return;

        //It is better not use velocity in order to give the impression of squared movement to the player
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");
        
        if (horiz < 0)
        {
            moveUsingTransform(Vector2.left);
            if (faceRight) {
                ChangeSide();
            }
        }
        else if (horiz > 0)
        {
            
            moveUsingTransform(Vector2.right);
            if (!faceRight) {
                ChangeSide();
            }
        } 
        
        if (vert < 0)
        {
            moveUsingTransform(Vector2.down);
        }
        else if (vert > 0)
        {
            moveUsingTransform(Vector2.up);
        }
        UpdatePlayerFatigue(vert!=0 && horiz!= 0);         
        Attack();        
    }

    public void CheckPlayerHealth() {
        if (!isDead && inventoryController.health < 0.5f && inventoryController.medicine > 0) {
            inventoryController.health = inventoryController.health  + 0.5f;
            gameSound.PlayUseSoda();
            inventoryController.medicine = inventoryController.medicine - 1;
        } else if (!isDead && inventoryController.health <= 0) {
            isDead = true;
            time = 0;
        }

        if (isDead && time >= timeToDie) {
            GameOver.Invoke(inventoryController);
        }
        animator.SetBool("isDead", isDead);
    }

    //changes the side in which the player is facing
    private void ChangeSide()
    {
        faceRight = !faceRight;
        
        Vector3 temp = transform.localScale;
        temp.x *= -1;
        transform.localScale = temp;
    }

    private void Attack() {
        timeAtack += Time.deltaTime;

        if (isAttacking) {
            if (timeAtack < timeToAttack) {
                return;
            }
            gameSound.PlayChop();
            isAttacking = false;
            animator.SetBool("isAttacking", isAttacking);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //Debug.Log("Attack");
            Hit();
            isAttacking = true;
            animator.SetBool("isAttacking", isAttacking);
            timeAtack = 0;
        }
    }


    int controllingSound = 0;
    private void moveUsingTransform(Vector2 direction)
    {
        if (time < timeToMove) {
            return;
        }
        time = 0;
        controllingSound++;
        Vector2 nextPosition = direction * spriteBoard.bounds.size.y/(speed * delayRate);
        previousPosition = transform.localPosition;
        transform.localPosition += (Vector3)nextPosition;
        
        if (controllingSound >= 3) {
            gameSound.PlayWalk();
            controllingSound=0;
        }

        DistanceWalked = DistanceWalked + Mathf.Abs(Vector2.Distance(previousPosition, nextPosition))/sprite.bounds.size.x;        
    }

    //Verify if the player colides with an item or the exit(marked as triggered)
    private void OnTriggerEnter2D(Collider2D other)
    {
        //print( "trigger:" +  other.gameObject.tag);
        if (other.gameObject.tag == "Food")
        {
            gameSound.PlayGetFruit();
            Destroy(other.gameObject);
            inventoryController.food++;
        } else if (other.gameObject.tag == "Soda")
        {
            gameSound.PlayGetSoda();
            Destroy(other.gameObject);
            inventoryController.medicine++;
        } else if (other.gameObject.tag == "Exit")
        {
            EndLevel.Invoke(this.inventoryController);
        } else if (other.gameObject.tag == "Enemy")
        {
            gameSound.PlayDie();
            GetHit(Game.DamageRate/2);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "barrel")
        {
            //print("collision stay:" + other.gameObject.tag);
            warmingTime += Time.deltaTime;
            if (warmingTime >= timeToWarm) {
                if (inventoryController.bodyTemperature < Game.RegularBodyTemperature) {
                    inventoryController.bodyTemperature++;
                    if (inventoryController.fatigue >= 0.05f) {
                        inventoryController.fatigue = inventoryController.fatigue - 2*Game.FatigueRate;
                    }
                }
                warmingTime = 0;
            }
        }
    }

    //verify if the player collides with a something that is not supposed to pass through
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

    public void UpdateTime(int hours, int minutes) {
        inventoryController.UpdateTime(hours, minutes);
        UpdatePlayerHungry(hours, minutes);
    }

    public void UpdateTemperature() {
        int temp = inventoryController.UpdateTemperature();

        int rate = -15;
        if (temp < rate) {
            delayRate = temp/rate;
        }
        int bt = inventoryController.UpdateBodyTemperature();
        if (bt < Game.RegularBodyTemperature) {
            inventoryController.health = inventoryController.health - Game.HealthRate;
        }
    }

    private int lastTime = 0;
    public void UpdatePlayerHungry(int hours, int minutes) {
        //update hungry at each 2 hours
        if(hours == lastTime) return;
        if (hours!= 0 && hours%2 == 0 && minutes == 0) {
            lastTime = hours;
            inventoryController.hungry = inventoryController.hungry + Game.StarvenessRate;
            //Debug.Log("time: " + inventoryController.GetTime());
            if (inventoryController.hungry >= 1){
                inventoryController.health = inventoryController.health - Game.HealthRate;
                inventoryController.hungry = 1;                
            }
        }

        if (inventoryController.hungry > 0.5 && inventoryController.food > 0) {
            gameSound.PlayUseFruit();
            inventoryController.hungry = inventoryController.hungry  - 0.5f;
            inventoryController.food = inventoryController.food - 1;
        }
    }
    
    public void UpdatePlayerFatigue(bool isMoving) {

        //update fatigue
        if (DistanceWalked >= 100) { //simulating 1/2 Km
            //Debug.Log("Distance: " + DistanceWalked);
            inventoryController.fatigue = inventoryController.fatigue + Game.FatigueRate;
            DistanceWalked = 0;
        }

        if (!isMoving){
            restingTime += Time.deltaTime;
            if (restingTime >= timeToRest) {
                inventoryController.fatigue = inventoryController.fatigue - Game.FatigueRate;
                restingTime = 0;
            }
        }

        if (inventoryController.fatigue >= 1) inventoryController.fatigue = 1;

        float fatigueRate = inventoryController.fatigue*10;
        if (fatigueRate > 7) {
            //0.05 is faster than 0.5
            timeToMove = moveRate*(10 - fatigueRate);
        } else {
            timeToMove = moveRate;
        }        
    }

    
    public override void GetHit(float damage){
        inventoryController.health = inventoryController.health - damage;
    }

    public override void Hit(){
        //Debug.Log("isHitting" + isAttacking);
        if (target != null) {
            target.GetHit(Game.DamageRate);
        }  
    }
}
