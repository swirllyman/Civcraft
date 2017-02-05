using UnityEngine.Networking;
using UnityEngine;
using System.Collections.Generic;

public class Mouse_Behavior : NetworkBehaviour
{
    public GameObject[] buildings;

    private GameObject ghost_2_2;
    private GameObject ghost_1_1;
    private NetworkHash128 building_2_2;
    private NetworkHash128 building_1_1;

    private Vector3 building_size;
    private GameObject ghost_object;
    private NetworkHash128 chosen_building;

    private Collider ghost_collider;
    private Ray ray;
    private RaycastHit hit;
    private int layer_mask = ((1 << 8) | Physics.IgnoreRaycastLayer);

    private Dictionary<NetworkHash128, GameObject> hash_codes;

    PlayerState my_state;
    PlayerController myPlayer;

    void Start ()
    {
        my_state = GetComponent<PlayerState>();
        myPlayer = GetComponent<PlayerController>();
        building_2_2 = (Resources.Load("Buildings/Spawn_Big") as GameObject).GetComponent<NetworkIdentity>().assetId;
        building_1_1 = (Resources.Load("Buildings/Spawn_Small") as GameObject).GetComponent<NetworkIdentity>().assetId;
        ghost_2_2 = Resources.Load("Buildings/Ghost_Big") as GameObject;
        ghost_1_1 = Resources.Load("Buildings/Ghost_Small") as GameObject;
        hash_codes = ClientScene.prefabs;
        buildings = new GameObject[] { Resources.Load("Buildings/Spawn_Big") as GameObject };
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
                        float x_position = building_size.x % 2 == 0 ? Mathf.RoundToInt(hit.point.x) : (Mathf.Floor(hit.point.x) + .5f);
                        float z_position = building_size.z % 2 == 0 ? Mathf.RoundToInt(hit.point.z) : (Mathf.Floor(hit.point.z) + .5f);
                        var spawnPosition = new Vector3(x_position, building_size.y / 2.0f, z_position);
                        print("Spawning -- Time 1: " + Time.time);
                        CmdSpawnThatShit(spawnPosition);//, chosen_building);
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Destroy(ghost_object);
                my_state.SwitchState(SelectionState.building);
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
    void CmdSpawnThatShit(Vector3 spawnSpot)//, NetworkHash128 chosen_building)
    {
        var spawnRotation = Quaternion.Euler(0, 0, 0);
        GameObject game_o;
        //hash_codes.TryGetValue(chosen_building, out game_o);
        game_o = buildings[0];
        GameObject enemy = Instantiate(game_o, spawnSpot, spawnRotation);
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
                chosen_building = building_2_2;
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
                chosen_building = building_1_1;
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
