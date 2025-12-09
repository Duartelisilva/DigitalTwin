using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pid_class;





public class movimentopid : MonoBehaviour
{

    public float v=0f; // Forward velocity of the car
    public float b = 6f; // Distance between front and rear axles
    public float a = 3f; // Distance between front axle and car's center of mass

    public float dist1 = 3f; // Distance between front axle and car's center of mass
    public float dist2 = 3f; // Distance between front axle and car's center of mass


    public float maxdegrees = 13.436f;
    public float frictionCoefficient = 0.1f;
    public float decelerationRate = 7f;

    public float error = 0f;
    public double output = 0f;


    public float delta; // Steering angle
    float forwardSpeed = 75.59f;
    float maxSpeed = 1.068f; //910068f
    public float centreOfGravityOffset = -0.7f;
    public float acceleration = 273.20f;
    private Rigidbody rb; // Reference to the Rigidbody component
    private float direcao = 1f;


    PID pid = new PID(1, 0.1, 0.01); //Kp  Ki  Kd

    // Start is called before the first frame update

    // Update is called once per frame

     void Start()
    {
        // Get the reference to the Rigidbody component
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass += Vector3.up * centreOfGravityOffset;
    }

    void FixedUpdate(){

        float offsetAngle1 = -70f;  // Offset angle of -70 degrees (adjust as needed)
        float offsetAngle2 = 70f;  // Offset angle of 70 degrees (adjust as needed)


        // Calculate the desired rotation by applying the offset to the car's rotation
        Quaternion offsetRotation1 = Quaternion.Euler(0f, offsetAngle1, 0f) * rb.transform.rotation;
        Quaternion offsetRotation2 = Quaternion.Euler(0f, offsetAngle2, 0f) * rb.transform.rotation;

        // Calculate the direction vector based on the rotated forward direction
        Vector3 offsetDirection1 = offsetRotation1 * Vector3.forward;
        Vector3 offsetDirection2 = offsetRotation2 * Vector3.forward;

        RaycastHit hit1, hit2;
        float maxDistance = 100f;

        if (Physics.Raycast(rb.transform.position, offsetDirection1, out hit1, maxDistance)) {
            // Ray hit something
            Debug.DrawLine(rb.transform.position, hit1.point, Color.black); // Optional: Visualize the ray in Scene view
            dist1 = hit1.distance; 
            dist1 = dist1 * Mathf.Cos(Mathf.Deg2Rad * 30);        

        }

        if (Physics.Raycast(rb.transform.position, offsetDirection2, out hit2, maxDistance)) {
            // Ray hit something
            Debug.DrawLine(rb.transform.position, hit2.point, Color.black); // Optional: Visualize the ray in Scene view
            dist2 = hit2.distance;  
            dist2 = dist2 * Mathf.Cos(Mathf.Deg2Rad * 30); 
        }


        error = dist1 - dist2;
        output = pid.OutputPID(dist1, dist2, 0.02);
        // Debug.Log("Output: " + output);


        // Calculate the forward velocity based on the input
        v = maxSpeed;

        // Calculate steering angle (in radians)
        //float delta = steeringInput * 9f *  Mathf.Deg2Rad;
        delta =  (float)output * Mathf.Deg2Rad;
        delta = Mathf.Clamp(delta, -maxdegrees * Mathf.Deg2Rad, maxdegrees * Mathf.Deg2Rad);

        if(rb.velocity.magnitude > 0)
        direcao = 1;
        else if(rb.velocity.magnitude < 0)
        direcao = -1;

        // Calculate velocities
        float alfa = Mathf.Atan(a * Mathf.Tan(delta) / b); // Steering angle of the front wheels
        float theta = transform.eulerAngles.y * Mathf.Deg2Rad; // Convert car's rotation to radians
        float x_dot = v * Mathf.Cos(alfa + theta); // x velocity
        float y_dot = v * Mathf.Sin(alfa + theta); // y velocity
        float theta_dot = rb.velocity.magnitude * direcao / b * Mathf.Tan(delta); // Angular velocity


        float speed = rb.velocity.magnitude;
        Debug.Log("Magnitude Speed: " + speed);
        Debug.Log("V Speed: " + v);
        // Update car's position and rotation based on velocities
        /*Vector3 newPosition = rb.position + new Vector3(-y_dot * Time.deltaTime, 0f, -x_dot * Time.deltaTime);
        rb.MovePosition(newPosition);*/

       // Get the forward direction of the car
        Vector3 forwardDirection = transform.forward;

        // Calculate the movement direction based on the forward direction
        Vector3 movementDirection = Vector3.ClampMagnitude(new Vector3(-y_dot, 0f, -x_dot), 1f);
        movementDirection = Vector3.ProjectOnPlane(movementDirection, Vector3.up).normalized;

        // Calculate the force to apply for movement
        Vector3 force = Vector3.Dot(movementDirection, forwardDirection) * forwardDirection * acceleration * Time.deltaTime;

        // Apply the force to the Rigidbody
        rb.AddForce(force, ForceMode.VelocityChange);

        // Apply damping to reduce sideways drifting
        float dampingFactor = 0.3f;
        Vector3 relativeVelocity = transform.InverseTransformDirection(rb.velocity);
        relativeVelocity.x *= dampingFactor;
        rb.velocity = transform.TransformDirection(relativeVelocity);

        // Limit the speed as before
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed-5);



        Quaternion deltaRotation = Quaternion.Euler(0f, theta_dot * Mathf.Rad2Deg * Time.deltaTime, 0f);
        rb.MoveRotation(rb.rotation * deltaRotation);









    }



}