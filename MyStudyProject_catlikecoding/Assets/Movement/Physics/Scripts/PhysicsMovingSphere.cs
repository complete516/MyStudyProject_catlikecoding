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
    [SerializeField,Range(0f,10f)]
    private float jumpHeight = 2f;

    Vector3 velocity, desiredVelocity;
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
        velocity = body.velocity;


        float maxSpeedChange = maxAcceleration * Time.deltaTime;


        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

        if (desiredJump)
        {
            desiredJump = false;
            Jump();
        }

        body.velocity = velocity;
    }

    void Jump()
    {
        velocity.y += Mathf.Sqrt(-2*Physics.gravity.y * jumpHeight);

    }
}
