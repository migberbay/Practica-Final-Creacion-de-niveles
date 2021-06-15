using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]

public class CharacterController2D : MonoBehaviour
{
    // Move player in 2D space
    public float maxSpeed = 3.4f;
    public float jumpHeight = 6.5f;
    public float gravityScale = 1.5f;

    public Collider2D[] excludedGroundColliders;

    bool facingRight = true;
    float moveDirection = 0;
    
    Rigidbody2D r2d;
    CapsuleCollider2D mainCollider;
    Transform t;

    public GameObject parry_object;

    public BombThrower thrower;

    public bool u_Dash, u_Bomb, u_Double_jump, u_Parry;

    public float dash_cd = .75f, dash_curr_cd = .75f, parry_cd = .75f, parry_curr_cd = .75f, bomb_cd = .5f, bomb_curr_cd = .5f, melee_cd = .33f, melee_curr_cd =.33f;
    
    public bool a_jump = false, isGrounded = false, isDashing = false, isParrying = false;

    public Animator animator;

    // Use this for initialization
    void Start()
    {
        t = transform;
        r2d = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<CapsuleCollider2D>();
        r2d.freezeRotation = true;
        r2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        r2d.gravityScale = gravityScale;
        facingRight = t.localScale.x > 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Movement controls
        if(!isDashing && !isParrying){
            if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
            {
                moveDirection = Input.GetKey(KeyCode.A) ? -1 : 1;
            }
            else{
                moveDirection = 0;
            }
        }

        // Change facing direction
        if (moveDirection != 0 && (!isDashing && !isParrying))
        {
            if (moveDirection > 0 && !facingRight)
            {
                facingRight = true;
                t.localScale = new Vector3(Mathf.Abs(t.localScale.x), t.localScale.y, transform.localScale.z);
            }
            if (moveDirection < 0 && facingRight)
            {
                facingRight = false;
                t.localScale = new Vector3(-Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
            }
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.W) && !isParrying && !isDashing) 
        {
            // floor jump
            if(isGrounded){
                r2d.velocity = new Vector2(r2d.velocity.x, jumpHeight);
                if(u_Double_jump){
                    a_jump = true;
                }
            }
            else if(a_jump && u_Double_jump){
                r2d.velocity = new Vector2(r2d.velocity.x, jumpHeight*0.75f);
                a_jump = false;
            } 
        }

        if (Input.GetKeyDown(KeyCode.Space) && u_Dash && dash_curr_cd <= 0 && !isParrying){ // dash
            dash_curr_cd = dash_cd;
            StartCoroutine(Dash());
        }

        if(Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift) && u_Parry && parry_curr_cd <= 0 && !isDashing){
            parry_curr_cd = parry_cd;
            StartCoroutine(Parry());
        }

        if(Input.GetMouseButtonDown(0) && melee_curr_cd < 0){
            melee_curr_cd = melee_cd;
            animator.SetTrigger("melee");
            Debug.Log("melee attack here.");
        }

        if(Input.GetMouseButtonDown(1) && u_Bomb && bomb_curr_cd < 0){
            bomb_curr_cd = bomb_cd;
            thrower.createNew();
        }
    }

    IEnumerator Dash(){
        float dash_time = 0.25f;
        isDashing = true;
        while(dash_time > 0){
            r2d.velocity = new Vector2((moveDirection) * (maxSpeed*4), r2d.velocity.y);
            yield return null;
            dash_time -= Time.deltaTime;
        }
        isDashing = false;
    }

    IEnumerator Parry(){
        float parry_time = 0.25f;
        isParrying = true;
        while(parry_time > 0){
            parry_object.SetActive(true);
            yield return null;
            parry_time -= Time.deltaTime;
        }
        parry_object.SetActive(false);
        isParrying = false;
    }
    void FixedUpdate()
    {
        Bounds colliderBounds = mainCollider.bounds;
        float colliderRadius = mainCollider.size.x * 0.4f * Mathf.Abs(transform.localScale.x);
        Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, colliderRadius * 0.9f, 0);
        // Check if player is grounded
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPos, colliderRadius);
        //Check if any of the overlapping colliders are not player collider, if so, set isGrounded to true
        isGrounded = false;
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != mainCollider && !excludedGroundColliders.Contains(colliders[i]))
                {
                    isGrounded = true;
                    break;
                }
            }
        }

        //Reduce dash cd
        dash_curr_cd -= Time.fixedDeltaTime;
        parry_curr_cd -= Time.fixedDeltaTime;
        bomb_curr_cd -= Time.fixedDeltaTime;
        melee_curr_cd -= Time.fixedDeltaTime;

        // Apply movement velocity
        if(!isDashing){
            r2d.velocity = new Vector2((moveDirection) * maxSpeed, r2d.velocity.y);
        }
        

        // Simple debug
        Debug.DrawLine(groundCheckPos, groundCheckPos - new Vector3(0, colliderRadius, 0), isGrounded ? Color.green : Color.red);
        Debug.DrawLine(groundCheckPos, groundCheckPos - new Vector3(colliderRadius, 0, 0), isGrounded ? Color.green : Color.red);
    }
}