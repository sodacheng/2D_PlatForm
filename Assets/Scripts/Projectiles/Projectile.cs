using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private AttackDetails attackDetails;

    private float speed;
    private float travelDistance;
    private float xStartPos;

    private bool isGravityOn;
    private bool hasHitGround;

    private Rigidbody2D rb;

    [SerializeField]
    private float gravity;
    [SerializeField]
    private float damageRadius; // 伤害半径


    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private LayerMask whatIsPlayer;
    [SerializeField]
    private Transform damagePosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0.0f;

        rb.velocity = transform.right * speed;

        xStartPos = transform.position.x;

        isGravityOn = false;
    }

    private void Update()
    {
        if(!hasHitGround)
        {
            attackDetails.position = transform.position;
            if(isGravityOn) //开启重力后旋转射弹
            {
                float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;//返回两条线间的角度  
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }

    private void FixedUpdate()
    {
        if(!hasHitGround)
        {
            Collider2D damageHit = Physics2D.OverlapCircle(damagePosition.position, damageRadius, whatIsPlayer);
            Collider2D groundHit = Physics2D.OverlapCircle(damagePosition.position, damageRadius, whatIsGround);

            if(damageHit)
            {
                damageHit.transform.SendMessage("Damage", attackDetails);
                Destroy(gameObject);
            }

            if(groundHit)
            {
                hasHitGround = true;
                rb.gravityScale = 0f;
                rb.velocity = Vector2.zero;
            }

            if (Mathf.Abs(xStartPos - transform.position.x) >= travelDistance && !isGravityOn)
            {
                isGravityOn = true;
                rb.gravityScale = gravity;
            }
        }


    }

    public void FireProjectil(float speed, float travelDistance, float damage)
    {
        this.speed = speed;
        this.travelDistance = travelDistance;
        attackDetails.damageAmout = damage;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(damagePosition.position, damageRadius);
    }
}
