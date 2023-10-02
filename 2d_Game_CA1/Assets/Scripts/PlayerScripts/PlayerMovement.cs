using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{//variables
    //basic movement and jump variables (Video Reference/Guide Follow: https://www.youtube.com/watch?v=K1xZ-rycYY8)

    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;

    //coyote Time Variables(Video Reference/Guide Follow:https://www.youtube.com/watch?v=RFix_Kg2Di0&t=55s)
    private float coyoteTime = 0.2f;//the higher the value the longer u have coyote time
    private float coyoteTimeCounter;

    //double jump variables (Video Reference/Guide Follow: https://www.youtube.com/watch?v=RdhgngSUco0)
    private bool doubleJump;

    //Wall slide and jump Variables (Video Reference/Guide Follow: https://www.youtube.com/watch?v=O6VX6Ro7EtA)
    private bool isWallSliding;//indicadtes wall climbing
    private float WallSlidingSpeed = 2f;
    private bool isWallJumping;//indicates if player is wall jumping
    private float WallJumpingDirection;//wall jumping direction
    private float wallJumpingTime = 0.2f;//time wall jumping
    private float wallJumpingCounter;//wall jump counter
    private float wallJumpingDuration = 0.4f;//wall jumping duration
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);//power of wall jump

    //dash variables (Video Reference/Guide Follow: https://www.youtube.com/watch?v=2kFGmuPHiA0)
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

        if(IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;//if we are grounded set the time counter to the coyote time
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;//if we arent grounded subtract the time
        }

        if (coyoteTimeCounter > 0f && Input.GetButtonDown("Jump"))//check if coyote time is greater than 0 and if jump button is pressed
        {
            doubleJump = false;

        }

        if (Input.GetButtonDown("Jump")) 
        {
            if (coyoteTimeCounter > 0f || doubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

                doubleJump = !doubleJump;
            }
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)//allows the player to jump higher by pressing the jump button(space) longer by multiply it by 0.5
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            coyoteTimeCounter = 0f;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Joystick1Button2)&& canDash)
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