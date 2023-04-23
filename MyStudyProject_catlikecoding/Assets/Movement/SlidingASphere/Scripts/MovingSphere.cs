using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSphere : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;

    [SerializeField]
    Rect allowedArea = new Rect(-5f, -5f, 10f, 10f);

    Vector3 velocity;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {



        Vector2 playerInput = Vector2.zero;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");

        playerInput = Vector2.ClampMagnitude(playerInput, 1.0f);
        Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        if (velocity.x < desiredVelocity.x)
        {
            velocity.x = Mathf.Min(velocity.x + maxSpeedChange, desiredVelocity.x);
        }
        else {
            velocity.x = Mathf.Min(velocity.x - maxSpeedChange, desiredVelocity.x);
        }

        velocity.x = Mathf.MoveTowards(velocity.x,desiredVelocity.x,maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z,desiredVelocity.z,maxSpeedChange);

        Vector3 displacement = velocity * Time.deltaTime;
        Vector3 newPosition = transform.localPosition + displacement;
        if (!allowedArea.Contains(new Vector2(newPosition.x,newPosition.z))) {
            newPosition = transform.localPosition;
        }

        transform.localPosition = newPosition;
    }
}
