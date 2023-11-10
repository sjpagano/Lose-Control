using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Car : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    public Vector3 car;
    bool canAccel = true;
    bool canJump = false;
    bool jumpCooldown = false;
    bool accelerating = false;
    static bool Stop = true;
    AudioSource jumpSound;
    float driftFactor = 0.98f;
    float turnFactor = 3f;
    float maxSpeed = 5;
    float steeringInput = 0;
    float rotationAngle = 0;
    float velocityVsUp = 0;
    // Start is called before the first frame update
    void Start()
    {
        jumpSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    async void Update()
    {
        if (Stop)
        {
            rb.velocity = Vector3.zero;
        }
        car = transform.position;
        if (canJump)
        {
            if (Input.GetKeyDown("space") && !jumpCooldown)
            {
                jumpCooldown = true;
                transform.gameObject.tag = "Airborn";
                Debug.Log("Jumped");
                jumpSound.Play();
                transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
                await Task.Delay(750);
                transform.gameObject.tag = "Player";
                canJump = false;
                transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);
                jumpCooldown = false;
            }
        }


        if (Input.GetKeyDown("space") && canAccel && transform.tag == "Player")
        {
            Stop = false;
            Debug.Log("Accelerating");
            accelerate();
            canAccel = false;
            await Task.Delay(1000);
            canAccel = true;
        }

        if (Input.GetKeyDown("i"))
        {
            SceneManager.LoadScene(0);
        }
    }

    private void FixedUpdate()
    {

        if (Stop)
            return;

        ApplyEngineForce();

        KillOrthogonalVelocity();

        ApplySteering();
    }

    void ApplyEngineForce()
    {

        if (accelerating)
            return;

        //Create a force for the engine
        Vector2 engineForceVector = transform.up * maxSpeed;

        //Caculate how much "forward" we are going in terms of the direction of our velocity
        velocityVsUp = Vector2.Dot(transform.up, rb.velocity);

        //Limit so we cannot go faster than the max speed in the "forward" direction
        if (velocityVsUp > maxSpeed)
            return;

        //Apply force and pushes the car forward
        rb.AddForce(engineForceVector, ForceMode2D.Force);
    }

    void ApplySteering()
    {
        if (Stop)
            return;
        //Update the rotation angle based on input
        rotationAngle -= steeringInput * turnFactor;

        //Apply steering by rotating the car object
        rb.MoveRotation(rotationAngle);
    }

    void KillOrthogonalVelocity()
    {
        //Get forward and right velocity of the car
        Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(rb.velocity, transform.right);

        //Kill the orthogonal velocity (side velocity) based on how much the car should drift. 
        rb.velocity = forwardVelocity + rightVelocity * driftFactor;
    }


    public void SetInputVector(Vector2 inputVector)
    {
        steeringInput = inputVector.x;
    }

    async void accelerate()
    {
        accelerating = true;
        velocityVsUp = Vector2.Dot(transform.up, rb.velocity);
        rb.AddForce(transform.up * 100, ForceMode2D.Force);
        while (velocityVsUp > maxSpeed)
        {   
            if (Stop)
            {
                break;
            }
            rb.velocity = transform.up * -8;
            await Task.Delay(30);
        }
        if (velocityVsUp <= maxSpeed)
        {
            accelerating = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Jumpbox")
        {
            canJump = true;
        }
    }

    public void StopCar()
    {
        rb.MoveRotation(transform.rotation);
        transform.tag = "Player";
        canJump = false;
        Stop = true;
        Debug.Log("Car Stopped");
    }

    public float carSpeed()
    {
        float speed = rb.velocity.magnitude;
        return speed;
    }

}
