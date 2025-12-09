using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movimentomanual : MonoBehaviour
{

    public float v=0f; // Forward velocity of the car
    public float b = 6f; // Distance between front and rear axles
    public float a = 3f; // Distance between front axle and car's center of mass

    public float maxdegrees = 13.436f;
    public float frictionCoefficient = 0.1f;
    public float decelerationRate = 7f;



    private float delta; // Steering angle
    //float forwardSpeed = 75.59f;
    float maxSpeed = 91.068f;
    public float centreOfGravityOffset = -0.7f;
    public float acceleration = 273.20f;
    private Rigidbody rb; // Reference to the Rigidbody component
    private float direction = 1f;



    // Start is called before the first frame update

    // Update is called once per frame

     void Start()
    {
        // Get the reference to the Rigidbody component
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass += Vector3.up * centreOfGravityOffset;
    }


     void FixedUpdate()
    {
        // Get steering input (e.g., left and right arrow keys)
        float steeringInput = Input.GetAxis("Horizontal");
        float movementInput = Input.GetAxis("Vertical");

        // Calculate the forward velocity based on the input
        v = movementInput;

        // Calculate steering angle (in radians)
        float delta = steeringInput * maxdegrees *  Mathf.Deg2Rad;
        delta = Mathf.Clamp(delta, -maxdegrees * Mathf.Deg2Rad, maxdegrees * Mathf.Deg2Rad);

        if(movementInput > 0)
        direction = 1;
        else if(movementInput < 0)
        direction = -1;
        // Calculate velocities
        float alfa = Mathf.Atan(a * Mathf.Tan(delta) / b); // Steering angle of the front wheels
        float theta = transform.eulerAngles.y * Mathf.Deg2Rad; // Convert car's rotation to radians
        float x_dot = v * Mathf.Cos(alfa + theta); // x velocity
        float y_dot = v * Mathf.Sin(alfa + theta); // y velocity
        float theta_dot = rb.velocity.magnitude * direction / b * Mathf.Tan(delta); // Angular velocity

        //v = rb.velocity.magnitude;
        float speed = rb.velocity.magnitude;
        Debug.Log("Magnitude Speed: " + speed);
        //Debug.Log("V Speed: " + v);
        // Update car's position and rotation based on velocities
        /*Vector3 newPosition = rb.position + new Vector3(-y_dot * Time.deltaTime, 0f, -x_dot * Time.deltaTime);
        rb.MovePosition(newPosition);*/

       // Get the forward direction of the car


        // Calculate the movement direction based on the forward direction
        Vector3 movementDirection = Vector3.ClampMagnitude(new Vector3(-y_dot, 0f, -x_dot), 1f);
        movementDirection = Vector3.ProjectOnPlane(movementDirection, Vector3.up).normalized;

        // Calculate the force to apply for movement
        Vector3 force = Vector3.Dot(movementDirection, transform.forward) * transform.forward * acceleration * Time.deltaTime;

        // Apply the force to the Rigidbody
        rb.AddForce(force, ForceMode.VelocityChange);
        

        // Atrito para o carro
        // **********************************************
    
        if(movementInput == 0) {
            Vector3 velocity = rb.velocity;
            Vector3 deceleration = -velocity * decelerationRate;
            rb.AddForce(deceleration, ForceMode.Acceleration);
        }



        // Calculate the velocity of the object
        /*Vector3 velocity = rb.velocity;

        // Calculate the damping force based on the velocity
        Vector3 dampingForce = -velocity * frictionCoefficient;

        // Apply the damping force to the Rigidbody
        rb.AddForce(dampingForce, ForceMode.Acceleration);*/

        //***************************************************
        
        // Apply damping to reduce sideways drifting
        float slippingFactor = 0.3f;
        Vector3 relativeVelocity = transform.InverseTransformDirection(rb.velocity);
        relativeVelocity.x *= slippingFactor;
        rb.velocity = transform.TransformDirection(relativeVelocity);

        // Limit the speed as before
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed-5);

        Quaternion deltaRotation = Quaternion.Euler(0f, theta_dot * Mathf.Rad2Deg * Time.deltaTime, 0f);
        rb.MoveRotation(rb.rotation * deltaRotation);

    }
}
