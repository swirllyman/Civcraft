using UnityEngine.Networking;
using UnityEngine;

public class Mouse_Behavior : NetworkBehaviour
{
    public GameObject[] buildings;

    public GameObject ghost_2_2;
    public GameObject ghost_1_1;

    private Vector3 building_size;
    private GameObject ghost_object;
    private int choiceNum;

    private Collider ghost_collider;
    private Ray ray;
    private RaycastHit hit;
    private int layer_mask = ((1 << 8) | Physics.IgnoreRaycastLayer);

    PlayerState my_state;
    PlayerController myPlayer;

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
                        CmdSpawnThatShit(hit.point, choiceNum);
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
        Physics.Raycast(ray, out hit, layer_mask);
        float x_position = building_size.x % 2 == 0 ? Mathf.RoundToInt(hit.point.x) : (Mathf.Floor(hit.point.x) + .5f);
        float z_position = building_size.z % 2 == 0 ? Mathf.RoundToInt(hit.point.z) : (Mathf.Floor(hit.point.z) + .5f);
        ghost_object.transform.position = new Vector3(x_position, building_size.y / 2.0f, z_position);
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
        return true;
    }

    [Command]
    void CmdSpawnThatShit(Vector3 spawnSpot, int choice)
    {
        float x_position = building_size.x % 2 == 0 ? Mathf.RoundToInt(spawnSpot.x) : (Mathf.Floor(spawnSpot.x) + .5f);
        float z_position = building_size.z % 2 == 0 ? Mathf.RoundToInt(spawnSpot.z) : (Mathf.Floor(spawnSpot.z) + .5f);
        var spawnPosition = new Vector3 (x_position, building_size.y / 2.0f, z_position);
        var spawnRotation = Quaternion.Euler(0, 0, 0);

        GameObject enemy = Instantiate(buildings[choice], spawnPosition, spawnRotation);
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
        if (GUI.Button(new Rect(Screen.width * .078f, Screen.height * .175f, Screen.width * .04f, Screen.height * .07f), "2x2"))
        {
            my_state.SwitchState(SelectionState.building);
            if (my_state.Get_State() == SelectionState.building)
            {
                choiceNum = 2;
                building_size = new Vector3(2, 2, 2);
                ghost_object = (GameObject)Instantiate(ghost_2_2, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                ghost_collider = ghost_object.GetComponent<Collider>();
            }
            else
            {
                Destroy(ghost_object);
            }
        }
        if (GUI.Button(new Rect(Screen.width * .078f, Screen.height * (.175f * 2), Screen.width * .04f, Screen.height * .07f), "1x1"))
        {
            my_state.SwitchState(SelectionState.building);
            if (my_state.Get_State() == SelectionState.building)
            {
                choiceNum = 1;
                building_size = new Vector3(1, 1, 1);
                ghost_object = (GameObject)Instantiate(ghost_1_1, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                ghost_collider = ghost_object.GetComponent<Collider>();
            }
            else
            {
                Destroy(ghost_object);
            }
        }
    }
}
