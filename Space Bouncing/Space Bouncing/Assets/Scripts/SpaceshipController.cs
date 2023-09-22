using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpaceshipController : MonoBehaviour
{
    private bool launched;
    private bool endGame;
    private float horizontalInput;
    public float turnSpeed = 45f;
    public float forwardSpeed = 10f;
    private int bounceTime;
    private int landingTime;
    public int maxBounceTime = 3;
    public int maxLandingTime = 5;
    private int currentMaxLandTime;
    public int badTime = -1;
    public int goodTime = 1;
    private Vector2? collisionNormal;
    public GameObject arrow; // Reference to the arrow GameObject
    public TextMeshProUGUI endGameText;
    public TextMeshProUGUI lifeText;
    private Vector2 originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;

        endGameText.text = "Use Left/Right or A/D to rotate the Spaceship,\nPress SPACE to Launch!\n"
                         + "\nSpaceship bounces twice before landing.\nReach the Yellow planet within remaining Launches.\n"
                         + "\nTouching the Blue planet earns bonus steps,\nTouching the Red planet charges penalty steps.\n"
                         + "\nPress ENTER to Start!";
        endGameText.gameObject.SetActive(true); // Show gameplay instructions

        lifeText.gameObject.SetActive(false);

        launched = false;
        endGame = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (endGame)
        {
            // Restart the game
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                transform.position = originalPosition;
                transform.rotation = Quaternion.Euler(0, 0, 0);
                collisionNormal = null;

                endGame = false;
                endGameText.gameObject.SetActive(false); // Hide the ending screen

                bounceTime = 0;
                landingTime = 0;
                currentMaxLandTime = maxLandingTime;

                arrow.SetActive(true); // Show the arrow when the spaceship is to be launched

                lifeText.text = "Launches: " + Math.Max(currentMaxLandTime - landingTime, 0);
                lifeText.gameObject.SetActive(true);
            }
        }

        // game starts
        else
        {
            // speed control
            if (launched)
            {
                transform.Translate(Vector2.up * forwardSpeed * Time.deltaTime);
            }

            // change direction
            else
            {
                horizontalInput = Input.GetAxis("Horizontal");
                float rotatedAngle = turnSpeed * horizontalInput * Time.deltaTime;

                if (IsValidLaunchAngle(transform.rotation.eulerAngles.z - rotatedAngle))
                {
                    transform.Rotate(Vector3.back, rotatedAngle);
                }

                // launching the spaceship
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    launched = true;
                    landingTime++;
                    arrow.SetActive(false); // Hide the arrow when the spaceship is launched

                    lifeText.text = "Launches: " + Math.Max(currentMaxLandTime - landingTime, 0);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Border")
        {
            EndGame(false);
        }
        else if (collision.gameObject.tag == "Finish")
        {
            EndGame(true);
        }

        // collision with planets
        else if (collision.gameObject.tag == "badPlanet")
        {
            CollideWithPlanet(collision, badTime);
        }
        else if (collision.gameObject.tag == "goodPlanet")
        {
            CollideWithPlanet(collision, goodTime);
        }
    }

    private void CollideWithPlanet(Collision2D collision, int timeChange)
    {
        // reflection

        // Get the contact normal of the first contact point
        Vector2 contactNormal = collision.contacts[0].normal;
        collisionNormal = contactNormal;

        // Get the current rotation angle of the object (assuming the object's rotation is in Z-axis)
        float rotationAngle = transform.rotation.eulerAngles.z + 90;

        // Convert the rotation angle to a direction vector
        Vector2 rotationDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * rotationAngle), Mathf.Sin(Mathf.Deg2Rad * rotationAngle));

        // Reflect the rotation direction vector against the contact normal
        Vector2 reflectedDirection = Vector2.Reflect(rotationDirection, contactNormal);

        // Calculate the new rotation angle from the reflected direction
        float newRotationAngle = Mathf.Atan2(reflectedDirection.y, reflectedDirection.x) * Mathf.Rad2Deg;

        // Set the new rotation angle to the object
        transform.rotation = Quaternion.Euler(0f, 0f, newRotationAngle - 90);

        // deal with landing
        bounceTime++;
        currentMaxLandTime += timeChange;
        lifeText.text = "Launches: " + Math.Max(currentMaxLandTime - landingTime, 0);

        if (bounceTime >= maxBounceTime)
        {
            launched = false;
            bounceTime = 0;
            arrow.SetActive(true); // Show the arrow when the spaceship is to be launched

            if (landingTime >= currentMaxLandTime)
            {
                EndGame(false);
            }
        }
    }

    private bool IsValidLaunchAngle(float desiredAngle)
    {
        if (collisionNormal.HasValue)
        {
            // Calculate the forward direction of the spaceship for the desired angle
            float desiredAngleInRadians = (desiredAngle + 90) * Mathf.Deg2Rad;
            Vector2 desiredDirection = new Vector2(Mathf.Cos(desiredAngleInRadians), Mathf.Sin(desiredAngleInRadians));

            // The dot product will be negative if the two vectors are more than 90 degrees apart.
            // This means the desired direction is into the planet (i.e., not a valid launch angle).
            return Vector2.Dot(desiredDirection, collisionNormal.Value) > 0;
        }

        else
        {
            // If there's no collision normal (i.e., the spaceship hasn't hit a planet yet), any angle is valid.
            return true;
        }
    }

    private void EndGame(bool isWin)
    {
        launched = false;
        endGame = true;

        lifeText.gameObject.SetActive(false);

        if (isWin)
        {
            endGameText.text = "You Win! ^_^\n\nPress ENTER to Replay!";
        }
        else
        {
            endGameText.text = "You Lose! >_<\n\nPress ENTER to Replay!";
        }
        endGameText.gameObject.SetActive(true);
    }
}
