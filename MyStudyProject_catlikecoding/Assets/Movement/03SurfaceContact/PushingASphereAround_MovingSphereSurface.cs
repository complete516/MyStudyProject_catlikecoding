using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushingASphereAround_MovingSphereSurface : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f, maxAirAcceleration = 1f;

    [SerializeField, Range(0.1f, 1f)]
    private float bounciness = 0.5f;

    Vector3 velocity, desiredVelocity;
    bool desiredJump;

    [SerializeField, Range(0f, 10f), Header("跳跃高度")]
    float jumpHeight = 2f;

    [SerializeField, Range(0, 5)]
    int maxAirJumps = 0;

    int jumpPhase;

    Rigidbody body;
    //是否在地上
    bool onGround;
    [SerializeField, Range(0f, 90f)]
    float maxGroundAngle = 25f;

    float minGroundDotProduct;

    Vector3 contactNormal;
    int groundContactCount = 0;
    int stepsSinceLastGrounded, stepsSinceLastJump;

    [SerializeField, Range(0f, 100f)]
    float maxSnapSpeed = 100f;

    [SerializeField, Min(0)]
    float probeDistance = 1f;

    private void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        OnValidate();
    }

    // Update is called once per frame
    void Update()
    {

        Vector2 playerInput = Vector2.zero;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");


        playerInput = Vector2.ClampMagnitude(playerInput, 1.0f);
        //预期速度 
        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

        desiredJump |= Input.GetButtonDown("Jump");

        GetComponent<Renderer>().material.SetColor("_Color", onGround ? Color.black : Color.white);
    }

    private void FixedUpdate()
    {
        //velocity = body.velocity;
        UpdateState();
        AdjustVelocity();
        // v = at;
        //float acceleration = onGround ? maxAcceleration : maxAirAcceleration;
        //float maxSpeedChange = acceleration * Time.deltaTime;

        //velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        //velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        if (desiredJump)
        {
            desiredJump = false;
            Jump();
        }

        body.velocity = velocity;
        onGround = false;
    }

    private void UpdateState()
    {
        stepsSinceLastGrounded += 1;
        stepsSinceLastJump += 1;
        velocity = body.velocity;
        if (onGround || SnapToGround())
        {
            if (groundContactCount > 1)
            {
                contactNormal.Normalize();
            }

            stepsSinceLastGrounded = 0;
            jumpPhase = 0;
        }
        else
        {
            contactNormal = Vector3.up;
        }
    }

    bool SnapToGround()
    {
        if (stepsSinceLastGrounded > 1 || stepsSinceLastGrounded <= 2)
        {
            return false;
        }
        float speed = velocity.magnitude;
        if (speed > maxSnapSpeed)
        {
            return false;
        }


        if (!Physics.Raycast(body.position, Vector3.down, out RaycastHit hit, probeDistance))
        {
            return false;
        }
        if (hit.normal.y < minGroundDotProduct)
        {
            return false;
        }
        groundContactCount = 1;

        float dot = Vector3.Dot(velocity, hit.normal);
        if (dot > 0f)
        {
            velocity = (velocity - hit.normal * dot).normalized * speed;
        }
        contactNormal = hit.normal;

        return true;
    }

    private void Jump()
    {
        if (onGround || jumpPhase < maxAirJumps)
        {
            jumpPhase += 1;
            stepsSinceLastJump = 0;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            float alignedSpeed = Vector3.Dot(velocity, contactNormal);
            if (alignedSpeed > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            }
            velocity += contactNormal * jumpSpeed;
        }
    }

    /// <summary>
    /// 将向量投影到地面
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }

    void AdjustVelocity()
    {
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

        float currentX = Vector3.Dot(velocity, xAxis);
        float currentZ = Vector3.Dot(velocity, zAxis);

        float acceleration = onGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX =
            Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newZ =
            Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //onGround = true;
        EvaluateCollision(collision);
    }


    private void OnCollisionStay(Collision collision)
    {
        //onGround = true;
        EvaluateCollision(collision);
    }

    void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            //onGround |= normal.y >= minGroundDotProduct;
            if (normal.y >= minGroundDotProduct)
            {
                onGround = true;
                contactNormal += normal;
                groundContactCount += 1;
            }
        }
    }
}
