using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ColorChanger : NetworkBehaviour {

    public Color[] colors;
    Renderer rend;

    [SyncVar]
    public int colorIdx = 0;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

	public void NextColor()
    {
        if (isServer)
        {
            CmdNextColor();
        }
    }

    [Command]
    public void CmdNextColor()
    {
        RpcNextColor();
    }

    [ClientRpc]
    void RpcNextColor()
    {
        int idx = (colorIdx++) % colors.Length;
        rend.material.color = colors[idx];
    }
}
