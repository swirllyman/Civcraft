using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class BasicMovement : NetworkBehaviour
{

    float currentSpeed;
    public float maxSpeed = 1.0f;
    public float maxHeight = 100.0f;
    public float minHeight = 3.0f;
    float ogMax;
    float currentHeight;
    public float maxHeightSpeed = 100.0f;
    float currentHeightSpeed;
    float ogMaxHeightSpeed;
    public float heightSmoothing = 1.5f;
    float directionY = 0;
    float heightDesintation;

    void Start()
    {
        if (!isLocalPlayer)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
        }
        currentSpeed = maxSpeed;
        ogMax = maxSpeed;

        currentHeightSpeed = maxHeightSpeed;
        ogMaxHeightSpeed = maxHeightSpeed;
    }
    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            currentSpeed = maxSpeed * 2;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            currentSpeed = maxSpeed;
        }
        transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * currentSpeed, 0, Input.GetAxis("Vertical") * Time.deltaTime * currentSpeed);

        directionY = -Input.GetAxis("Mouse ScrollWheel");
        if (directionY != 0)
        {
            heightDesintation = transform.position.y + directionY * currentHeightSpeed; new Vector3(transform.position.x, transform.position.y + directionY * currentHeightSpeed, transform.position.z);
        }
        if (heightDesintation != 0 && ((heightDesintation > minHeight || directionY > 0) && (heightDesintation < maxHeight || directionY < 0)))
        {
            currentHeight = Mathf.Lerp(transform.position.y, heightDesintation, Time.deltaTime * heightSmoothing);
            transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

            float normalHeight = currentHeight / maxHeight;
            if (normalHeight > .5f)
            {
                if (maxSpeed != ogMax * 5)
                {
                    maxSpeed = ogMax * 5;
                    maxHeightSpeed = ogMaxHeightSpeed * 30;
                    currentSpeed = maxSpeed;
                }
            }
            else if (normalHeight > .25f)
            {
                if (maxSpeed != ogMax * 2)
                {
                    maxSpeed = ogMax * 2;
                    maxHeightSpeed = ogMaxHeightSpeed * 15;
                    currentSpeed = maxSpeed;
                }
            }
            else
            {
                if (maxSpeed != ogMax)
                {
                    maxSpeed = ogMax;
                    maxHeightSpeed = ogMaxHeightSpeed;
                    currentSpeed = maxSpeed;
                }
            }
        }
    }
}
