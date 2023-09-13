using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    private bool lauched = false;
    private float horizontalInput;
    public float turnSpeed = 45.0f; 
    private float forwardSpeed = 0f;
    private int bounceTime = 0;
    public int maxLandingTime = 3;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // speed control
        transform.Translate(Vector3.up * forwardSpeed * Time.deltaTime);

        // game start
        if (!lauched) {
            horizontalInput = Input.GetAxis("Horizontal");
            transform.Rotate(Vector3.forward, -turnSpeed * horizontalInput * Time.deltaTime);
        }

        // lauching the spaceship
        if (Input.GetKey(KeyCode.Space)) {
            lauched = true;
            forwardSpeed = 10.0f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {

        if (collision.gameObject.tag == "Finish") { // collision with the border
            Debug.Log("you lose");
            forwardSpeed = 0;
        } else { // collision with planets
            CollideWithPlanet(collision);
        }
    }

    private void CollideWithPlanet(Collision2D collision) {
        // reflection
        float angleZ = (transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad;
        Vector2 prevDir = new Vector2(Mathf.Cos(angleZ), Mathf.Sin(angleZ));
        Vector2 outDir = Vector2.Reflect(prevDir, collision.GetContact(0).normal);
        angleZ = Mathf.Atan2(outDir.y, outDir.x);
        transform.rotation = Quaternion.Euler(0, 0, angleZ * Mathf.Rad2Deg - 90);

        // deal with landing
        bounceTime++;
        if (bounceTime % 3 == 0) {
            forwardSpeed = 0f;
            lauched = false;
            if (bounceTime / 3 > maxLandingTime) {
                Debug.Log("you lose");
            }
        }
    } 
}
