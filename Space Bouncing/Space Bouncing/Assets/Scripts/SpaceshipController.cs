using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpaceshipController : MonoBehaviour
{
    private bool launched;
    private float horizontalInput;
    public float turnSpeed = 45.0f;
    public float forwardSpeed = 10.0f;
    private int bounceTime;
    public int maxLandingTime = 3;
    public GameObject arrow; // Reference to your arrow GameObject
    private Vector2? collisionNormal = null;
    public TextMeshProUGUI endGameText;

    // Start is called before the first frame update
    void Start()
    {
        endGameText.gameObject.SetActive(false);  // Initially hide the text
    
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

            float desiredAngle = transform.rotation.eulerAngles.z - turnSpeed * horizontalInput * Time.deltaTime;
            if (IsValidLaunchAngle(desiredAngle))
            {
                transform.Rotate(Vector3.back, turnSpeed * horizontalInput * Time.deltaTime);
            }
        }

        // launching the spaceship
        if (Input.GetKeyDown(KeyCode.Space) && !launched)
        {
            launched = true;
            arrow.SetActive(false); // Hide the arrow when the spaceship is launched
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Border")
        {
            launched = false;
            endGameText.text = "You lose!";
            endGameText.gameObject.SetActive(true);
        }
        else if (collision.gameObject.tag == "Finish")
        {
            launched = false;
            endGameText.text = "You win!";
            endGameText.gameObject.SetActive(true);
        }
        else if (collision.gameObject.tag == "Planet")
        { // collision with planets
            CollideWithPlanet(collision);
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

        collisionNormal = collision.GetContact(0).normal;

        // deal with landing
        bounceTime++;
        if (bounceTime == maxLandingTime)
        {
            launched = false;
            bounceTime = 0;
            arrow.SetActive(true); // Show the arrow when the spaceship is to be launched
        }
    }

    private bool IsValidLaunchAngle(float desiredAngle)
    {
        if (!collisionNormal.HasValue)
        {
            // If there's no collision normal (i.e., the spaceship hasn't hit a planet yet), any angle is valid.
            return true;
        }

        // Calculate the forward direction of the spaceship for the desired angle
        float desiredAngleInRadians = (desiredAngle + 90) * Mathf.Deg2Rad;
        Vector2 desiredDirection = new Vector2(Mathf.Cos(desiredAngleInRadians), Mathf.Sin(desiredAngleInRadians));

        // The dot product will be negative if the two vectors are more than 90 degrees apart.
        // This means the desired direction is into the planet (i.e., not a valid launch angle).
        return Vector2.Dot(desiredDirection, collisionNormal.Value) >= 0;
    }
}
