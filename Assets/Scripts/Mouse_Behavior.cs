using UnityEngine.Networking;
using UnityEngine;

public class Mouse_Behavior : NetworkBehaviour
{
    public GameObject enemyPrefab;
    public GameObject ghost_building;
    public GameObject ghost_object;
    public Vector2 building_size;

    private Collider ghost_collider;
    private Ray ray;
    private RaycastHit hit;
    private int layer_mask = ((1 << 8) | Physics.IgnoreRaycastLayer);

    PlayerState my_state;
    PlayerController myPlayer;

    // Use this for initialization
    void Start ()
    {
        my_state = GetComponent<PlayerState>();
        myPlayer = GetComponent<PlayerController>();
    }
	
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if(my_state.Get_State() == SelectionState.building)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out hit, layer_mask))
                {
                    if (Valid_Placement(hit))
                    {
                        CmdSpawnThatShit(hit.point);
                    }
                }
            }
            else
            {
                Move_Ghost();
            }
        }

    }

    void Move_Ghost()
    {
        Physics.Raycast(ray, out hit);
        ghost_object.transform.position = new Vector3(Mathf.RoundToInt(hit.point.x), 2, Mathf.RoundToInt(hit.point.z));
    }

    bool Valid_Placement(RaycastHit hit_object)
    {
        Vector3 box_dimensions = new Vector3(building_size.x / 2.1f, building_size.y / 2.1f);
        Collider[] col = Physics.OverlapBox(ghost_collider.bounds.center, box_dimensions, transform.rotation);
        foreach(Collider coll in col)
        {
            if(coll.tag.Equals("Building"))
            {
                return false;
            }
        }
        if(!hit_object.transform.gameObject.tag.Equals("Building"))
        {

        }
        return true;
    }

    [Command]
    void CmdSpawnThatShit(Vector3 spawnSpot)
    {
        var spawnPosition = new Vector3 (Mathf.RoundToInt(spawnSpot.x), 2, Mathf.RoundToInt(spawnSpot.z));
        var spawnRotation = Quaternion.Euler(0, 0, 0);

        var enemy = (GameObject)Instantiate(enemyPrefab, spawnPosition, spawnRotation);
        NetworkServer.Spawn(enemy);
        enemy.GetComponent<EnemySpawner>().Init(myPlayer.playerColor, myPlayer.playerNumber);
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
            my_state.SwitchState(SelectionState.building);
            ghost_object = (GameObject)Instantiate(ghost_building, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
            ghost_collider = ghost_object.GetComponent<Collider>();
        }
    }
}
