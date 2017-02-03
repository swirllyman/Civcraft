using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            ColorChanger c = hit.transform.GetComponent<ColorChanger>();
            if (c != null && Input.GetMouseButtonDown(0))
            {
                CmdChangeObjectColor(c.gameObject);
            }
        }
    }

    [Command]
    void CmdChangeObjectColor(GameObject g)
    {
        g.GetComponent<ColorChanger>().CmdNextColor();
    }
}
