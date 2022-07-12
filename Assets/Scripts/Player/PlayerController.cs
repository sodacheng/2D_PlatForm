using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : MonoBehaviour
{
    #region 变量
    private float movementInputDirection; // 水平输入
    private float jumpTimer;
    private float turnTimer;
    private float wallJumpTimer;
    private float dashTimeLeft; // 冲刺剩余时间
    private float lastImageXpos;
    private float lastDash = -100; // 跟踪上次冲刺开始的时间, 设置为负数, 让游戏一开始我们就能冲刺
    private float knockbackStartTime;
    [SerializeField]
    private float knockbackDuration;

    private int amountOfJumpsLeft; // 剩余跳跃次数计数
    private int facingDirection = 1; // 面向方向
    private int lastWallJumpDirection; // 最后一次反墙跳的方向

    private bool isFacingRight = true; // 是否面向右方
    private bool isWalking; // 是否在走路
    private bool isGrounded; // 是否接触地面
    private bool isTouchingWall; // 是否接触墙面
    private bool isWallSliding; // 是否在滑墙
    //private bool canJump; // 是否能跳跃
    private bool canNormalJump;
    private bool canWallJump;
    private bool isAttemptingToJump; // 是否想要跳跃(在还不满足跳跃条件的时候)
    private bool checkJumpMultipier; // 用于小跳, 当Player在跳跃中松开空格, 则对Y轴进行减速, 限制Y轴方向的跳跃高度
    private bool hasWallJumped;
    private bool isTouchingLedge; // 攀爬射线检测
    private bool canClimbLedge = false; // 能否攀爬
    private bool ledgeDetected;
    private bool isDashing; // 是否冲刺
    private bool knockback;

    // 添加这两个bool 是为了优化反墙跳时的判读
    private bool canMove;
    private bool canFlip;

    [SerializeField]
    private Vector2 knockbackSpeed; 

    private Vector2 ledgePosBot; // 攀爬检测时墙壁射线击中的位置
    private Vector2 ledgePos1; // 攀爬检测
    private Vector2 ledgePos2; // 攀爬检测

    private Rigidbody2D rb;
    private Animator anim;
    [Header("需要将Rigidbody2D 的 碰撞检测改为连续")]
    public int amountOfJumps = 2; // 主角能跳跃的次数

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
    public float jumpTimerSet = 0.15f; // 跳跃按键的缓冲时间, 如果按下跳跃按键时不满足跳跃条件,且在计时器内又重新满足跳跃条件, Player则会进行跳跃
    public float turnTimerSet = 0.1f;
    public float wallJumpTimerSet = 0.5f;

    public float ledgeClimbXOffset1 = 0f;
    public float ledgeClimbYOffset1 = 0f;
    public float ledgeClimbXOffset2 = 0f;
    public float ledgeClimbYOffset2 = 0f;

    public float dashTime; // 冲刺时间
    public float dashSpeed; // 冲刺速度
    public float distanceBetweenImages; // 冲刺时残像放置的间距
    public float dashCoolDown; // 冲刺冷却;

    public Vector2 wallHopDirection;
    public Vector2 wallJumpDirection;

    public Transform groundCheck;
    public Transform wallCheck;
    public Transform ledgeCheck; // 攀爬检测

    public LayerMask whatIsGround; // Layer层 -> 在Inspector中选择Ground层
    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        amountOfJumpsLeft = amountOfJumps;
        wallHopDirection.Normalize(); // 将方向归一化
        wallJumpDirection.Normalize(); // 将方向归一化
    }

    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallSliding(); // 检测当前是否该进行滑墙
        CheckJump();
        CheckLedgeClimb();
        CheckDash();
        CheckKnockback();
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
        if (isTouchingWall && movementInputDirection == facingDirection && rb.velocity.y < 0 && !canClimbLedge) // 触墙且输入方向和面向方向(对着墙)相同 [只有贴墙向下的时候才表现为滑墙, 向上则不会] [在攀爬的时候不会滑墙]
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    public bool GetDashStatus()
    {
        return isDashing;
    }

    public void Knockback(int direction)
    {
        knockback = true;
        knockbackStartTime = Time.time;
        rb.velocity = new Vector2(knockbackSpeed.x * direction, knockbackSpeed.y);
    }

    private void CheckKnockback()
    {
        if(Time.time > knockbackStartTime + knockbackDuration &&knockback)
        {
            knockback = false;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void CheckLedgeClimb()
    {
        if (ledgeDetected && !canClimbLedge)
        {
            canClimbLedge = true;

            if (isFacingRight)
            {
                // 因为我们的瓷砖是一个一个单位的 所以使用 Mathf.Floor() 指向瓷砖的角落 (左边)
                ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) - ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) + ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }
            else
            {
                // 因为我们的瓷砖是一个一个单位的 所以使用 Mathf.Ceil() 指向瓷砖的角落 (右边)
                ledgePos1 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) + ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) - ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }

            // 一旦获得了这两个点, 我们想要移除玩家的控制权,
            canMove = false; // 在攀爬动画运行时 玩家无法移动角色
            canFlip = false; // 同理
            anim.SetBool("canClimbLedge", canClimbLedge);
        }

        if (canClimbLedge)
        {
            // 保持玩家的位置在Pos1
            transform.position = ledgePos1;
        }

    }

    /// <summary>
    /// 结束攀爬时调用的函数
    /// </summary>
    public void FinishLedgeClimb()
    {
        canClimbLedge = false;
        transform.position = ledgePos2;
        canMove = true;
        canFlip = true;
        ledgeDetected = false;
        anim.SetBool("canClimbLedge", canClimbLedge);
    }

    /// <summary>
    /// 判断Player周边状态, 是否触地, 是否触墙等
    /// </summary>
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround); // 是否触地

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround); // 是否触墙

        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, whatIsGround); // 攀爬检测的射线, 当触墙且攀爬射线没有被阻挡时可以攀爬

        if (isTouchingWall && !isTouchingLedge && !ledgeDetected)
        {
            ledgeDetected = true;
            ledgePosBot = wallCheck.position;
        }
    }

    /// <summary>
    /// 判断Player能否进行地面跳跃或者反墙跳
    /// </summary>
    private void CheckIfCanJump()
    {
        if (isGrounded && rb.velocity.y <= 0.01F)
        {
            amountOfJumpsLeft = amountOfJumps; // 重置 Player跳跃次数
        }

        if (isTouchingWall)
        {
            checkJumpMultipier = false;
            canWallJump = true;
        }

        if (amountOfJumpsLeft <= 0)
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
        if (isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }

        if (Mathf.Abs(rb.velocity.x) >= 0.01f)
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
            if (isGrounded || (amountOfJumpsLeft > 0 && !isTouchingWall))
            {
                NormalJump();
            }
            else
            {
                jumpTimer = jumpTimerSet;
                isAttemptingToJump = true;
            }
        }

        if (Input.GetButtonDown("Horizontal") && isTouchingWall)
        {
            if (!isGrounded && movementInputDirection != facingDirection)
            {
                canMove = false;
                canFlip = false;

                turnTimer = turnTimerSet;
            }
        }

        if (turnTimer >= 0)
        {
            turnTimer -= Time.deltaTime;

            if (turnTimer <= 0)
            {
                canMove = true;
                canFlip = true;
            }
        }

        // 当在跳跃中松开了空格, 则将Y轴速度与Multiplier相乘
        if (checkJumpMultipier && !Input.GetButton("Jump"))
        {
            checkJumpMultipier = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeightMultiplier);
        }


        if(Input.GetButtonDown("Dash"))
        {
            if(Time.time >= (lastDash + dashCoolDown))
            AttemptToDash();   
        }
    }

    private void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;

    }

    public int GetFacingDirection()
    {
        return facingDirection;
    }

    /// <summary>
    /// 设置冲刺速度, 检测冲刺是否应该停止
    /// </summary>
    private void CheckDash()
    {
        if (isDashing)
        {
            if(dashTimeLeft > 0)
            {
                canMove = false;
                canFlip = false;
                rb.velocity = new Vector2(dashSpeed * facingDirection, 0);
                dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs((transform.position.x - lastImageXpos)) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXpos = transform.position.x;
                }
            }
            
            if(dashTimeLeft <= 0 ||isTouchingWall)
            {
                isDashing = false;
                canMove = true;
                canFlip = true;
            }
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
            else if (isGrounded)
            {
                NormalJump();
            }
        }

        if (isAttemptingToJump)
        {
            jumpTimer -= Time.deltaTime;
        }

        #region 阻止玩家"单面上墙"
        if (wallJumpTimer > 0)
        {
            if (hasWallJumped && movementInputDirection == -lastWallJumpDirection) // 在wallJumpTimer计时内玩家输入面向跳出的那面墙, 则会停止玩家上升, 阻止玩家"单面上墙"
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                hasWallJumped = false;
            }
            else
            {
                wallJumpTimer -= Time.deltaTime;
            }
        }
        else if (wallJumpTimer <= 0)
        {
            hasWallJumped = false;
        }

        #endregion

    }

    /// <summary>
    /// 地面跳跃
    /// </summary>
    private void NormalJump()
    {
        //if (canJump && !isWallSliding) // 地面上的跳跃
        if (canNormalJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            amountOfJumpsLeft--;

            #region 取消 JumpCheck
            jumpTimer = 0;
            isAttemptingToJump = false;
            #endregion

            checkJumpMultipier = true;
        }
    }

    /// <summary>
    /// 反墙跳
    /// </summary>
    private void WallJump()
    {
        //if ((isWallSliding || isTouchingWall) && movementInputDirection != 0 && canJump) // Wall Jump 蹬墙跳(向上跳) isWallSliding || isTouchingWall贴住墙可以立即反墙跳
        if (canWallJump)
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

            checkJumpMultipier = true;

            #region 反墙跳后恢复正常的控制状态
            turnTimer = 0;
            canMove = true;
            canFlip = true;
            #endregion

            #region 阻止"单面上墙" - 更新状态 在 CheckJump() 中处理
            hasWallJumped = true;
            wallJumpTimer = wallJumpTimerSet;
            lastWallJumpDirection = -facingDirection;
            #endregion

        }
    }

    /// <summary>
    /// 驱动角色移动
    /// </summary>
    private void ApplyMovement()
    {

        if (!isGrounded && !isWallSliding && movementInputDirection == 0 && !knockback) // 角色在空中且无水平输入
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiplier, rb.velocity.y); // 水平方向速度受空气阻力系数相乘, 保持减速趋势 // 设置为0 就是松开按键立即停止(类似空洞骑士那种手感)?
        }
        else if (canMove && !knockback) // 运动不能覆盖击退
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }

        if (isWallSliding) // 角色在滑墙
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }


    public void DisableFlip()
    {
        canFlip = false;
    }

    public void EnableFlip()
    {
        canFlip = true; 
    }

    /// <summary>
    /// 翻转角色
    /// </summary>
    private void Flip()
    {
        // 当角色非滑墙状态时才会翻转
        if (!isWallSliding && canFlip && !knockback)
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
