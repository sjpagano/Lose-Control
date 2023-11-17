using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Car : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] TextMeshProUGUI level1;
    [SerializeField] TextMeshProUGUI level2;
    [SerializeField] TextMeshProUGUI level3;
    [SerializeField] TextMeshProUGUI level4;
    [SerializeField] TextMeshProUGUI level5;
    [SerializeField] TextMeshProUGUI level6;
    [SerializeField] TextMeshProUGUI finalTime;
    [SerializeField] TextMeshProUGUI finalTimeNumber;
    [SerializeField] Canvas winGui;
    [SerializeField] Camera backCam;
    public Vector3 car;
    bool canAccel = true;
    bool canJump = false;
    bool canDoubleJump = false;
    bool jumpCooldown = false;
    bool accelerating = false;
    bool doubleJumpCooldown = false;
    bool hasAcceledBefore = false;
    static bool Stop = true;
    AudioSource jumpSound;
    float driftFactor = 0.98f;
    float turnFactor = 3f;
    float maxSpeed = 5;
    float steeringInput = 0;
    float rotationAngle = -55;
    float velocityVsUp = 0;
    static float newAngle = -55;
    static int level = 1;
    static bool isNextLevel = true;
    string finalTimeNum;
    Stopwatch timer;
    Stopwatch finalTimer;
    public TimeSpan timeElapsed { get; private set; }
    public TimeSpan finalTimeElapsed { get; private set; }


    // Start is called before the first frame update
    void Start()
    {
        jumpSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    async void Update()
    {
        if (level == 1 && !isNextLevel)
        {
            timeElapsed = timer.Elapsed;
            finalTimeElapsed = finalTimer.Elapsed;
            level1.text = "Level 1: " + timeElapsed.ToString("mm\\:ss\\.ff");
            finalTime.text = "Final Time: " + finalTimeElapsed.ToString("mm\\:ss\\.ff");

        }
        else if (level == 2 && !isNextLevel)
        {
            timeElapsed = timer.Elapsed;
            finalTimeElapsed = finalTimer.Elapsed;
            level2.text = "Level 2: " + timeElapsed.ToString("mm\\:ss\\.ff");
            finalTime.text = "Final Time: " + finalTimeElapsed.ToString("mm\\:ss\\.ff");
        }
        else if (level == 3 && !isNextLevel)
        {
            timeElapsed = timer.Elapsed;
            finalTimeElapsed = finalTimer.Elapsed;
            level3.text = "Level 3: " + timeElapsed.ToString("mm\\:ss\\.ff");
            finalTime.text = "Final Time: " + finalTimeElapsed.ToString("mm\\:ss\\.ff");
        }
        else if (level == 4 && !isNextLevel)
        {
            timeElapsed = timer.Elapsed;
            finalTimeElapsed = finalTimer.Elapsed;
            level4.text = "Level 4: " + timeElapsed.ToString("mm\\:ss\\.ff");
            finalTime.text = "Final Time: " + finalTimeElapsed.ToString("mm\\:ss\\.ff");
        }
        else if (level == 5 && !isNextLevel)
        {
            timeElapsed = timer.Elapsed;
            finalTimeElapsed = finalTimer.Elapsed;
            level5.text = "Level 5: " + timeElapsed.ToString("mm\\:ss\\.ff");
            finalTime.text = "Final Time: " + finalTimeElapsed.ToString("mm\\:ss\\.ff");
        }
        else if (level == 6 && !isNextLevel)
        {
            timeElapsed = timer.Elapsed;
            finalTimeElapsed = finalTimer.Elapsed;
            level6.text = "Level 6: " + timeElapsed.ToString("mm\\:ss\\.ff");
            finalTime.text = "Final Time: " + finalTimeElapsed.ToString("mm\\:ss\\.ff");
        }
        else if (level == 7)
        {
            finalTimeNum = finalTimeElapsed.ToString("mm\\:ss\\.ff");
            finalTimeNumber.text = finalTimeNum;
            backCam.gameObject.SetActive(true);
            winGui.gameObject.SetActive(true);

        }
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
                jumpAnimation();
                await Task.Delay(750);
                canJump = false;
                jumpCooldown = false;
            }
        }
        if (canDoubleJump)
        {
            if (Input.GetKeyDown("space") && !doubleJumpCooldown)
            {
                doubleJumpCooldown = true;
                jumpAnimation();
                await Task.Delay(350);
                canDoubleJump = false;
                doubleJumpCooldown = false;
            }
        }


        if (Input.GetKeyDown("space") && canAccel && transform.tag == "Player")
        {
            if (level == 1 && isNextLevel)
            {
                finalTimer = new Stopwatch();
                finalTimer.Start();
            }
            if (isNextLevel)
            {
                timer = new Stopwatch();
                timer.Reset();
                timer.Start();
                finalTimer.Start();
            }
            isNextLevel = false;
            Stop = false;
            accelerate();
            canAccel = false;
            await Task.Delay(1000);
            canAccel = true;
        }
        if (isNextLevel && hasAcceledBefore)
        {
            finalTimer.Stop();
        }
    }

    private void FixedUpdate()
    {
        if (Stop)
        {
            rotationAngle = newAngle;
            rb.MoveRotation(rotationAngle);
            return;
        }

        ApplyEngineForce();

        KillOrthogonalVelocity();

        ApplySteering();
    }

    void ApplyEngineForce()
    {

        if (accelerating)
            return;
        if (jumpCooldown)
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
        if (jumpCooldown)
            return;
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
        hasAcceledBefore = true;
        accelerating = true;
        rb.AddForce(transform.up * 150, ForceMode2D.Force);
        while (rb.velocity.magnitude > maxSpeed)
        {   
            if (Stop)
            {
                break;
            }
            rb.velocity -= 0.01f * rb.velocity;
            await Task.Delay(30);
        }
        if (rb.velocity.magnitude <= maxSpeed)
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
        if (collision.gameObject.tag == "Jumpbox2")
        {
            canDoubleJump = true;
        }
    }

    public void StopCar(float angle)
    {
        newAngle = angle;
        transform.tag = "Player";
        canJump = false;
        canDoubleJump = false;
        Stop = true;
    }

    public float carSpeed()
    {
        float speed = rb.velocity.magnitude;
        return speed;
    }

    public void nextLevel()
    {
        isNextLevel = true;
        level++;
    }

    private async void jumpAnimation()
    {
        transform.gameObject.tag = "Airborn";
        jumpSound.Play();
        transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        await Task.Delay(750);
        transform.localScale = new Vector3(1f, 1f, 1f);
        transform.gameObject.tag = "Player";
    }

}
