using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Car : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    public Vector3 car;
    int carSpeed = 0;
    bool canAccel = true;
    bool canJump = false;
    bool jumpCooldown = false;
    static bool Stop;
    AudioSource jumpSound;
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
            carSpeed = 0;
            Stop = false;
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
            accelerate();
            canAccel = false;
            await Task.Delay(1000);
            canAccel = true;
        }
        Vector2 lateralVelocity = (Vector2.Dot(rb.velocity, transform.right) * transform.right);
        if (Input.GetKey("a"))
        {
            if (rb.velocity.x != 0 || rb.velocity.y != 0)
            {
                var rotation = Time.deltaTime * 300;
                transform.Rotate(new Vector3(0, 0, rotation));
                rb.velocity = Vector2.Lerp(rb.velocity, rb.velocity - lateralVelocity, 0.1f);
                rb.AddForce(transform.right * Time.deltaTime * 200);
            }
        }
        if (Input.GetKey("d"))
        {
            if (rb.velocity.x != 0 || rb.velocity.y != 0)
            {
                var rotation = Time.deltaTime * -300;
                transform.Rotate(new Vector3(0, 0, rotation));
                rb.velocity = Vector2.Lerp(rb.velocity, rb.velocity - lateralVelocity, 0.1f);
                rb.AddForce(transform.right * Time.deltaTime * -200);
            }
        }
        if (Input.GetKeyDown("i"))
        {
            SceneManager.LoadScene(0);
        }
        drive();
    }

    void drive()
    {
        rb.velocity = transform.up * carSpeed;
    }

    async void accelerate()
    {
        carSpeed = 12;
        while (carSpeed != 6)
        {
            carSpeed -= 1;
            await Task.Delay(60);
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
        transform.tag = "Player";
        Stop = true;
        Debug.Log("Car Stopped");
    }

}
