using UnityEngine.Networking;
using UnityEngine;

public class Mouse_Behavior : NetworkBehaviour
{
    public GameObject enemyPrefab;

    private Ray ray;
    private RaycastHit hit;
    bool build = false;

    // Use this for initialization
    void Start ()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    }
	
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (Input.GetMouseButton(0) && build)
        {
            if (Physics.Raycast(ray, out hit))
            {
                CmdSpawnThatShit(hit.point);
            }
        }
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    [Command]
    void CmdSpawnThatShit(Vector3 spawnSpot)
    {
        var spawnPosition = spawnSpot;
        Debug.Log(spawnPosition);
        var spawnRotation = Quaternion.Euler(0, 0, 0);

        var enemy = (GameObject)Instantiate(enemyPrefab, spawnPosition, spawnRotation);
        NetworkServer.Spawn(enemy);
    }

    void OnGUI()
    {
        if(!isLocalPlayer)
        {
            return;
        }
        GUI.Box(new Rect(Screen.width * .05f, Screen.height * .15f, Screen.width * .14f, Screen.height * .48f), "");
        if (GUI.Button(new Rect(Screen.width * .078f, Screen.height * .175f, Screen.width * .04f, Screen.height * .07f), "Click"))
        {
            build = !build;
        }
    }
}
