using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float runSpeed;
    public float jumpSpeed;
    public float doubleJumpSpeed;
    public float restoreTime;
    public float climbSpeed;

    private Rigidbody2D myRigidbody;
    private Animator myAnim;
    private BoxCollider2D myFeet;
    private bool isGround;
    private bool canDoubleJump;
    private bool isOnewayPlatform;

    private bool isLadder;
    private bool isClimbing;

    private bool isJumping;
    private bool isFalling;
    private bool isDoubleJumping;
    private bool isDoubleFalling;

    private float playerGravity;


    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
        myFeet = GetComponent<BoxCollider2D>();
        playerGravity = myRigidbody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameController.isGameAlive)
		{
            CheckAirStatus();
            Run();
            Flip();
            Jump();
            Climb();
            //Attack();
            CheckGrounded();
            CheckLadder();
            SwitchAnimation();
            OnewayPlatformCheck();
            
        }
    }
    void CheckGrounded()
    {
        isGround = myFeet.IsTouchingLayers(LayerMask.GetMask("Ground")) ||
             //myFeet.IsTouchingLayers(LayerMask.GetMask("Spike")) ||
             myFeet.IsTouchingLayers(LayerMask.GetMask("MovingPlatform")) ||
             myFeet.IsTouchingLayers(LayerMask.GetMask("OnewayPlatform"));
        //Debug.Log(isGround);
        isOnewayPlatform = myFeet.IsTouchingLayers(LayerMask.GetMask("OnewayPlatform"));
    }

    void CheckLadder()
	{
        isLadder = myFeet.IsTouchingLayers(LayerMask.GetMask("Ladder"));

    }

    void Flip()
	{
        bool playerHasXAxisSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if(playerHasXAxisSpeed) //反转
		{
            if(myRigidbody.velocity.x > 0.1f)
			{
                transform.localRotation = Quaternion.Euler(0, 0, 0);
			}
            if (myRigidbody.velocity.x < -0.1f)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    void Run()
    {
        float moveDir = Input.GetAxis("Horizontal");

        Vector2 playerVel = new Vector2(moveDir * runSpeed, myRigidbody.velocity.y); //y不变
        myRigidbody.velocity = playerVel;
        bool playerHasXAxisSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnim.SetBool("Run", playerHasXAxisSpeed);// 	如果x轴有速度，run为True
    }

    void Jump()
	{
        if(Input.GetButtonDown("Jump"))
		{
            if(isGround)
			{
                myAnim.SetBool("Jump", true);
                Vector2 jumpVel = new Vector2(0.0f, jumpSpeed);
                myRigidbody.velocity = Vector2.up * jumpVel;
                canDoubleJump = true;
            }
            else
			{
                if(canDoubleJump)
				{
                    myAnim.SetBool("DoubleJump", true);
                    Vector2 doubleJumpVel = new Vector2(0.0f, doubleJumpSpeed);
                    myRigidbody.velocity = Vector2.up * doubleJumpVel;
                    canDoubleJump = false;
				}
			}
		}
	}

    void Climb()
	{
        float moveY = Input.GetAxis("Vertical");
        if (isClimbing)
		{
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, moveY * climbSpeed);
		}

        if(isLadder)
		{
            
            if(moveY>0.5f||moveY<-0.5f)
			{
                myAnim.SetBool("Jump", false);
                myAnim.SetBool("DoubleJump", false);
                myAnim.SetBool("Climb", true);
                myRigidbody.gravityScale = 0.0f;
                myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, moveY * climbSpeed);
                myRigidbody.gravityScale = 0.0f;
			}
			else
			{
                if(isJumping||isFalling||isDoubleFalling||isDoubleJumping)
				{
                    myAnim.SetBool("Climb",false);
				}
                else
				{
                    myAnim.SetBool("Climb", false);
                    myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, 0.0f);
				}
			}
		}

		else
		{
            myAnim.SetBool("Climb", false);
            myRigidbody.gravityScale = playerGravity;
		}

        if(isLadder&&isGround)
		{
            myRigidbody.gravityScale = playerGravity;
		}
	}
    /*void Attack()
	{
        if(Input.GetButtonDown("Attack"))
		{
            myAnim.SetTrigger("Attack");
        }
	}*/


    void SwitchAnimation()
	{
        myAnim.SetBool("Idle", false);
        if(myAnim.GetBool("Jump"))
		{
            if(myRigidbody.velocity.y<0.0f)
			{
                myAnim.SetBool("Jump", false);
                myAnim.SetBool("Fall", true); ;
            }
		}

        else if(isGround)
		{
            myAnim.SetBool("Fall", false);
            myAnim.SetBool("Idle", true);
		}

        if (myAnim.GetBool("DoubleJump"))
        {
            if (myRigidbody.velocity.y < 0.0f)
            {
                myAnim.SetBool("DoubleJump", false);
                myAnim.SetBool("DoubleFall", true); ;
            }
        }

        else if (isGround)
        {
            myAnim.SetBool("DoubleFall", false);
            myAnim.SetBool("Idle", true);
        }
    }

    void OnewayPlatformCheck()
    {
        if(isGround&&gameObject.layer!=LayerMask.NameToLayer("Player"))
		{
            gameObject.layer = LayerMask.NameToLayer("Player");
		}
        //if(isGround&&gameObject.)
        float MoveY = Input.GetAxis("Vertical");
        if(isOnewayPlatform&&MoveY<-0.1f)
		{
            gameObject.layer = LayerMask.NameToLayer("OnewayPlatform");
            Invoke("RestorePlayerLayer",restoreTime);
		}
    }

    void RestorePlayerLayer()
	{
        if (!isGround && gameObject.layer != LayerMask.NameToLayer("Player"))
		{
            gameObject.layer = LayerMask.NameToLayer("Player");
		}
	}

    void CheckAirStatus()
	{
        isJumping = myAnim.GetBool("Jump");
        isFalling = myAnim.GetBool("Fall");
        isDoubleJumping = myAnim.GetBool("DoubleJump");
        isDoubleFalling = myAnim.GetBool("DoubleFall");
        isClimbing = myAnim.GetBool("Climb");
    }
}
