using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{//variables
    //basic movement and jump variables

    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;

    //double jump variables
    private bool doubleJump;

    //Wall slide and jump Variables
    private bool isWallSliding;//indicadtes wall climbing
    private float WallSlidingSpeed = 2f;
    private bool isWallJumping;//indicates if player is wall jumping
    private float WallJumpingDirection;//wall jumping direction
    private float wallJumpingTime = 0.2f;//time wall jumping
    private float wallJumpingCounter;//wall jump counter
    private float wallJumpingDuration = 0.4f;//wall jumping duration
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);//power of wall jump

    //dash variables
    private bool canDash = true;//determines if player can dash
    private bool isDashing;//determines if player is already dashing
    private float dashingPower = 24f;//dashing power
    private float dashingTime = 0.2f;//time spent dashing
    private float dashingCooldown = 1f;//cooldown of dash ability

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Transform wallcheck;
    [SerializeField] private LayerMask wallLayer;
    // Update is called once per frame
    private void Update()
    {
        if (isDashing)
        {
            return;
        }
        horizontal = Input.GetAxisRaw("Horizontal");


        if (IsGrounded() && !Input.GetButton("Jump"))//check if coyote time is greater than 0 and if jump button is pressed
        {
            doubleJump = false;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (IsGrounded() || doubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

                doubleJump = !doubleJump;
            }
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)//allows the player to jump higher by pressing the jump button(space) longer by multiply it by 0.5
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        WallSlide();
        WallJump();

        if(!isWallJumping)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        if(!isWallJumping)
        {
            if (isDashing)
            {
                return;
            }
            
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
   {
        return Physics2D.OverlapCircle(wallcheck.position, 0.2f, wallLayer);//checks if the player is colliding with a wall
   }

   private void WallSlide()
   {
        if(IsWalled() && !IsGrounded() && horizontal != 0f)//if we arent on the ground and we are at a wall set wall sliding to true
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -WallSlidingSpeed, float.MaxValue));
           
        }
        else 
        {
            isWallSliding = false;
        }
   }

   private void WallJump()
   {
        if(isWallSliding)
        {
            isWallJumping = false;
            WallJumpingDirection = -transform.localScale.x;//flips the direction that the player is facing
            wallJumpingCounter = wallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));//cancels method if player is wall sliding
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(WallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;//prevents spamming jump button

            if(transform.localScale.x != WallJumpingDirection)//flips player to face direction of movement
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);//invoke method with a delay
        }
   }

   private void StopWallJumping()
   {
        isWallJumping = false;
   }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);//indicates direction player is facing
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}