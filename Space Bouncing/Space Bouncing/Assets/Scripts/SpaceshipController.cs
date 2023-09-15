using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    private bool launched;
    private float horizontalInput;
    public float turnSpeed = 45.0f;
    public float forwardSpeed = 10.0f;
    private int bounceTime;
    public int maxLandingTime = 3;
    public GameObject arrow; // Reference to your arrow GameObject

    // Start is called before the first frame update
    void Start()
    {
        launched = false;
        bounceTime = 0;
        arrow.SetActive(true); // Show the arrow when the spaceship is to be launched
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // speed control
        if (launched)
        {
            transform.Translate(Vector2.up * forwardSpeed * Time.deltaTime);
        }

        // game start
        else
        {
            horizontalInput = Input.GetAxis("Horizontal");
            transform.Rotate(Vector3.back, turnSpeed * horizontalInput * Time.deltaTime);
        }

        // launching the spaceship
        if (Input.GetKey(KeyCode.Space))
        {
            launched = true;
            arrow.SetActive(false); // Hide the arrow when the spaceship is launched
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Border")
        { // collision with the border
            launched = false;
            Debug.Log("You lose!");
        }

        else if (collision.gameObject.tag == "Planet")
        { // collision with planets
            CollideWithPlanet(collision);
        }

        else if (collision.gameObject.tag == "Finish")
        { // collision with destination
            launched = false;
            Debug.Log("You win!");
        }
    }

    private void CollideWithPlanet(Collision2D collision)
    {
        // reflection
        float angleZ = (transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad;
        Vector2 prevDir = new Vector2(Mathf.Cos(angleZ), Mathf.Sin(angleZ));
        Vector2 outDir = Vector2.Reflect(prevDir, collision.GetContact(0).normal);
        angleZ = Mathf.Atan2(outDir.y, outDir.x);
        transform.rotation = Quaternion.Euler(0, 0, angleZ * Mathf.Rad2Deg - 90);

        // deal with landing
        bounceTime++;
        if (bounceTime == maxLandingTime)
        {
            launched = false;
            bounceTime = 0;
            arrow.SetActive(true); // Show the arrow when the spaceship is to be launched
        }
    }
}
