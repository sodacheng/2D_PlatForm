using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private float movementInputDirection; // 水平输入
    private float jumpTimer;

    private int amountOfJumpsLeft; // 剩余跳跃次数计数
    private int facingDirection = 1; // 面向方向

    private bool isFacingRight = true; // 是否面向右方
    private bool isWalking; // 是否在走路
    private bool isGrounded; // 是否接触地面
    private bool isTouchingWall; // 是否接触墙面
    private bool isWallSliding; // 是否在滑墙
    //private bool canJump; // 是否能跳跃
    private bool canNormalJump;
    private bool canWallJump;
    private bool isAttemptingToJump; // 是否想要跳跃(在还不满足跳跃条件的时候)

    private Rigidbody2D rb;
    private Animator anim;

    public int amountOfJumps = 1; // 主角能跳跃的次数

    public float movementSpeed = 10.0f; // 移动速度
    public float jumpForce = 16.0f; // 跳跃力(设置速度)
    public float groundCheckRadius; // 地面检测半径范围
    public float wallCheckDistance; // 墙壁检测距离
    public float wallSlideSpeed; // 滑墙最大速度
    public float movementForceInAir;  // 在空中移动时施加给角色的力的大小
    public float airDragMultiplier = 0.95f; // 空气阻力系数 
    public float variableJumpHeightMultiplier = 0.5f; // 可变跳跃高度系数, 参见 CheckInput() 方法
    public float wallHopForce;  // 从墙上跳下来的力度
    public float wallJumpForce; // 从墙上蹬墙跳(向上)的力度
    public float jumpTimerSet = 0.15f;
    

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    public Transform groundCheck;
    public Transform wallCheck;

    public LayerMask whatIsGround; // Layer层 -> 在Inspector中选择Ground层

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        amountOfJumpsLeft = amountOfJumps; 
        wallHopDirection.Normalize(); // 将方向归一化
        wallJumpDirection.Normalize(); // 将方向归一化
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallSliding(); // 检测当前是否该进行滑墙
        CheckJump();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurroundings();
    }

    /// <summary>
    /// 检测当前是否该进行滑墙
    /// </summary>
    private void CheckIfWallSliding()
    {
        if (isTouchingWall && movementInputDirection == facingDirection) // 触墙且输入方向和面向方向(对着墙)相同
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    /// <summary>
    /// 判断Player周边状态, 是否触地, 是否触墙等
    /// </summary>
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround); // 是否触地

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround); // 是否触墙
    }

    /// <summary>
    /// 判断Player能否进行地面跳跃或者反墙跳
    /// </summary>
    private void CheckIfCanJump()
    {
        if(isGrounded && rb.velocity.y <= 0.01F)
        {
            amountOfJumpsLeft = amountOfJumps; // 重置 Player跳跃次数
        }

        if(isTouchingWall)
        {
            canWallJump = true;
        }

        if(amountOfJumpsLeft <= 0)
        {
            canNormalJump = false; // 跳跃计数用完则不能跳跃
        }
        else
        {
            canNormalJump = true;
        }
      
    }

    /// <summary>
    /// 移动方向相关判断
    /// 根据移动方向控制角色专项
    /// 判断角色是否在水平方向上走动
    /// </summary>
    private void CheckMovementDirection()
    {
        if(isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if(!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }

        if(rb.velocity.x != 0)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }
    }

    /// <summary>
    /// 更新动画状态机相关变量
    /// </summary>
    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
    }

    /// <summary>
    /// 输入检测
    /// </summary>
    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            if(isGrounded || (amountOfJumpsLeft > 0 && isTouchingWall))
            {
                NormalJump();
            }
            else
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
        }
        
        // 当我们长按空格, 和之前跳的一样高, 短按空格就可以小跳
        if (Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier); 
        }

    }

    /// <summary>
    /// 跳跃检查, 当玩家在不能跳跃的时候点击了跳跃键, 则启动一个计时器, 如果在计时器时间内满足了跳跃条件,则进行相应的跳跃(地面跳跃/反墙跳)
    /// </summary>
    private void CheckJump()
    {
        if (jumpTimer > 0)
        {
            //WallJump
            if (!isGrounded && isTouchingWall && movementInputDirection != 0 && movementInputDirection != facingDirection) 
            {
                WallJump();
            }
            else if(isGrounded)
            {
                NormalJump();
            }
        }

        if(isAttemptingToJump)
        {
            jumpTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 地面跳跃
    /// </summary>
    private void NormalJump()
    {
        //if (canJump && !isWallSliding) // 地面上的跳跃
        if(canNormalJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;

            #region 取消 JumpCheck
            jumpTimer = 0;
            isAttemptingToJump = false;
            #endregion
        }
    }

    /// <summary>
    /// 反墙跳
    /// </summary>
    private void WallJump()
    {
        //if ((isWallSliding || isTouchingWall) && movementInputDirection != 0 && canJump) // Wall Jump 蹬墙跳(向上跳) isWallSliding || isTouchingWall贴住墙可以立即反墙跳
        if(canWallJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0); // 跳跃前先重置Y方向的速度, 可以保证跳跃的手感一致
            isWallSliding = false;
            amountOfJumpsLeft = amountOfJumps; // 从墙面跳跃和地面跳跃一样是总跳跃次数-1
            amountOfJumpsLeft--;
            Vector2 forceToAdd = new Vector2(wallJumpForce * wallJumpDirection.x * movementInputDirection, wallJumpForce * wallJumpDirection.y); // 向人物输出面向方向添加的力, 根据水平输入控制追加力的方向
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);

            #region 取消 JumpCheck
            jumpTimer = 0;
            isAttemptingToJump = false;
            #endregion
        }
    }

    /// <summary>
    /// 驱动角色移动
    /// </summary>
    private void ApplyMovement()
    {

        if (!isGrounded && !isWallSliding && movementInputDirection == 0) // 角色在空中且无水平输入
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y); // 水平方向速度受空气阻力系数相乘, 保持减速趋势 // 设置为0 就是松开按键立即停止(类似空洞骑士那种手感)?
        }
        else
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }
        
        if (isWallSliding) // 角色在滑墙
        {
            if(rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }

    /// <summary>
    /// 翻转角色
    /// </summary>
    private void Flip()
    {
        // 当角色非滑墙状态时才会翻转
        if (!isWallSliding)
        {
            facingDirection *= -1; // 变更角色的面向方向, 每次我们翻转角色的时候, 他都会在 1 和 -1 之前切换, 用于与归一化的表示方向的向量相乘
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    /// <summary>
    /// 在Scene标注判定范围变化
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
}
