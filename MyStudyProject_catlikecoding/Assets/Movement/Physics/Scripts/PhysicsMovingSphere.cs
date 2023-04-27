using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsMovingSphere : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;
    bool desiredJump;
    // [SerializeField]
    // Rect allowedArea = new Rect(-5f, -5f, 10f, 10f);

    // [SerializeField, Range(0.1f, 1f)]
    // private float bounciness = 0.5f;

    private Rigidbody body;
    [SerializeField, Range(0f, 10f)]
    private float jumpHeight = 2f;
    [SerializeField, Range(0f, 10f)]
    private int maxAirJumps = 0;

    int jumpPhase;
    Vector3 velocity, desiredVelocity;

    private bool onGround; //‘⁄µÿ…œ
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {



        Vector2 playerInput = Vector2.zero;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");

        playerInput = Vector2.ClampMagnitude(playerInput, 1.0f);
        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;


        desiredJump |= Input.GetButtonDown("Jump");
        // if (velocity.x < desiredVelocity.x)
        // {
        //     velocity.x = Mathf.Min(velocity.x + maxSpeedChange, desiredVelocity.x);
        // }
        // else if(velocity.x > desiredVelocity.x)
        // {
        //     velocity.x = Mathf.Min(velocity.x - maxSpeedChange, desiredVelocity.x);
        // }

    }

    private void FixedUpdate()
    {
        UpdateState();

        float maxSpeedChange = maxAcceleration * Time.deltaTime;


        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        if (desiredJump)
        {
            desiredJump = false;
            Jump();
        }

        body.velocity = velocity;
        onGround = false;
    }

    void UpdateState()
    {
        velocity = body.velocity;
        if (onGround)
        {
            jumpPhase = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        onGround = true; ;
    }

    private void OnCollisionStay(Collision collision)
    {
        onGround = true;
    }


    //private void OnCollisionExit(Collision collision)
    //{
    //    onGround = false;
    //}
    void Jump()
    {
        if (onGround || jumpPhase < maxAirJumps)
        {
            jumpPhase += 1;

             float jumpSpeed = Mathf.Sqrt(-2 * Physics.gravity.y * jumpHeight);
            if (velocity.y > 0)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - velocity.y,0f);
            }
            velocity.y += jumpSpeed;

        }

    }

    private void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            onGround |= normal.y >= 0.9f;
        }
    }
}
