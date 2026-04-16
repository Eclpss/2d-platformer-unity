using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayeMovement : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public ParticleSystem smokeFX;
    public ParticleSystem speedFX;
    [Header("Movement")]
    bool isFacingRight = true;
    public float moveSpeed = 5f;
    float horizontalMovement;
    float speedMultiplier = 1f;
    BoxCollider2D playerCollider;

    [Header("Dashing")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.1f;
    public float dashCooldown = 0.1f;
    bool isDashing;
    bool canDash = true;
    TrailRenderer trailRenderer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Jumping")]
    public float jumpPower = 10f;
    public int maxJumps = 2;
    int jumpsRemaining;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
    public LayerMask groundLayer;
    bool isGrounded;
    bool isOnPlatform;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMutiplier = 2f;

    [Header("WallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.49f, 0.03f);
    public LayerMask wallLayer;
    

    [Header("WallMovement")]
    public float wallSildeSpeed = 2;
    bool isWallSliding;
    //wall jumping
    bool isWallJumping; 
    float wallJumpDirection;
    float wallJumpTime = 0.5f;
    float wallJumpTimer;
    public Vector2 wallJumpPower  = new Vector2(5f, 10f);



    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        playerCollider = GetComponent<BoxCollider2D>();
        SpeedItem.onSpeedCollected += StartSpeedBoost;
    }

    void StartSpeedBoost(float multiplyer)
    {
        StartCoroutine(SpeedBoostCourutine(multiplyer));
    }
    private IEnumerator SpeedBoostCourutine(float multiplyer)
    {
        speedMultiplier = multiplyer;
        speedFX.Play();
        yield return new WaitForSeconds(2f);
        speedMultiplier = 1f;
        speedFX.Stop();
    }

    // Update is called once per frame
    void Update()
    {

        animator.SetFloat("yVelocity", rb.velocity.y);
        animator.SetFloat("magnitude", rb.velocity.magnitude);
        animator.SetBool("isWallSliding", isWallSliding);
        if (isDashing)
        {
            return;
        }
        GroundCheck();
        Gravity();
        ProcessWallSlide();
        ProccessWallJump();


        if (!isWallJumping)
        {
            rb.velocity = new Vector2(horizontalMovement * moveSpeed * speedMultiplier, rb.velocity.y);
            Flip();
        }

        
    }

    private void Gravity(){
        if(rb.velocity.y < 0 ){
            rb.gravityScale = baseGravity * fallSpeedMutiplier; // fall increasingly fast
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        } else{
            rb.gravityScale = baseGravity;
        }
    }
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
        //movements for the horizontal
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)//this means if we pressed the dash button all the way
        {
            StartCoroutine(DashCoroutine());
        }
        //movements for the horizontal
    }
    private IEnumerator DashCoroutine()
    {
        Physics2D.IgnoreLayerCollision(7, 8, true);
        //when we want to start dashing
        canDash = false;
        isDashing = true;

        trailRenderer.emitting = true;
        float dashDirection = isFacingRight ? 1f : -1f; //if its facing right its gonna be 1f if not its gonna be -1f

        rb.velocity = new Vector2(dashDirection * dashSpeed, rb.velocity.y); //Dash Movement

        //we want the dash to last in a spesific amount of time

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = new Vector2(0f, rb.velocity.y); //reset horizontal velocity 
        isDashing = false;
        trailRenderer.emitting = false;
        Physics2D.IgnoreLayerCollision(7, 8, false);

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    public void Drop(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && isOnPlatform && playerCollider.enabled) //if we pressed the drop button
        {
            //courutine dropping
            StartCoroutine(DisablePlayerCollider(0.25f));
            
        }
    }
    private IEnumerator DisablePlayerCollider(float disableTime)
    {
        playerCollider.enabled = false; //we want this disable to be a lil bit
        yield return new WaitForSeconds(disableTime);
        playerCollider.enabled = true; //now we true it 

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Platform"))
        {
            isOnPlatform = false;
        }
    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0)
        {

            if (context.performed)//this is when we pressed the jump button
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);//the y is gonna be change by the jumppower whichis the 10f varible in the top
                jumpsRemaining--;
                JumpFX();

            }
            else if (context.canceled && rb.velocity.y > 0)
            {
                //light tap of jump button = half the height
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
                jumpsRemaining--;
                JumpFX();

            }
        }
        //wall jumpin
        if (context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y); //this will make it jump away from the wall
            wallJumpTimer = 0;
            JumpFX();

            //force flip
            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1; //we flip our sprite by - minus one change the scale in transform --> player
                transform.localScale = ls; //basicly what this does is we jump when we flip around not when we not 
            }

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f); //wall jump will last about 0.5f or 0.5 seconds -- and we can jump again at 0.6 seconds
        }


    }
    private void JumpFX()
    {
        animator.SetTrigger("jump");
        smokeFX.Play();

    }
 
 
    

    private void GroundCheck() {


        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer)) {
            jumpsRemaining = maxJumps;
            isGrounded = true;
        } else {
            isGrounded = false;
        }

    }
    private bool WallCheck(){
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);//same as the top in ground check

    }

    private void ProcessWallSlide(){
        //if we want to slide we need to not touching the ground, on a wall & movement needs != 0 or not movin
        if(!isGrounded & WallCheck() & horizontalMovement != 0)
        { //is grounded was made from the ground check bool now wee need to make the remaining bool check like the are we on a wall or nah
            isWallSliding = true; //if the wall slidin true we capin the slow speed
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSildeSpeed)); //caps the fall rate

        }
        else 
        {
            isWallSliding = false; 
        }

    }
    private void ProccessWallJump()
    {
        if (isWallSliding){
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x; //so that if we jump we jump in the opposite the direction boii
            wallJumpTimer = wallJumpTime; //reset the timer so we can jump again

            CancelInvoke(nameof(CancelWallJump)); 
        } 
        else if(wallJumpTimer > 0f){
            wallJumpTimer -= Time.deltaTime;

        }
    }
    private void CancelWallJump(){
        isWallJumping = false;
    }

    private void Flip(){
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1; //we flip our sprite by - minus one change the scale in transform --> player
            transform.localScale = ls;
            speedFX.transform.localScale = ls;

            if (rb.velocity.y == 0)
            {
                 smokeFX.Play();
            }
           
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);

          Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }

}
