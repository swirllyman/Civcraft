using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class BasicMovement : NetworkBehaviour {

    public float moveSpeed = 1.0f;
    Camera myCam;

    void Start()
    {
        if (isLocalPlayer)
        {
            myCam = GetComponentInChildren<Camera>();
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed, 0, Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed);
    }
}
