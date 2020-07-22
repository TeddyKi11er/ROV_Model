using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestMove : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI speedometerText;
    [SerializeField] float velocity;

    private Rigidbody playerRb;
    private GameObject centerOfMass;
    public float speed = 5f;
    public float turnSpeed = 25;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        centerOfMass = GameObject.Find("Center Of Mass");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(centerOfMass.transform.forward * speed * forwardInput);
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up * Time.deltaTime * turnSpeed * horizontalInput);
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * turnSpeed);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(- Vector3.forward * Time.deltaTime * turnSpeed);
        }
          if (Input.GetKey(KeyCode.Z))
        {
            playerRb.AddForce(centerOfMass.transform.up * speed);
        }
        if (Input.GetKey(KeyCode.X))
        {
            playerRb.AddForce(- centerOfMass.transform.up * speed);
        }

        // spedometer
        velocity = Mathf.Round(playerRb.velocity.magnitude * 3.6f);
        speedometerText.SetText("Velocity: " + velocity + "kph");

    }
}
