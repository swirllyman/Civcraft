using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class Unit : NetworkBehaviour {
    NavMeshAgent agent;

    public GameObject selectedIndicator;
	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
	}
	
    [Command]
    public void CmdMoveTo(Vector3 pos)
    {
        RpcMoveTo(pos);
    }

    [ClientRpc]
    void RpcMoveTo(Vector3 pos)
    {
        agent.SetDestination(pos);
    }

    public void ToggleIndicator(bool toggle)
    {
        selectedIndicator.SetActive(toggle);
    }

}
