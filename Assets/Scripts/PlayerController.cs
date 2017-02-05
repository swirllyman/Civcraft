using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    public List<GameObject> currentUnits = new List<GameObject>();
    public Color[] colorChoices;

    [SyncVar]
    public int playerNumber;
    [SyncVar]
    public int teamNumber;
    [SyncVar]
    public Color playerColor;

    bool selecting;
    Vector3 startingMousePos;
    Vector3 mouseStartDrag;
    Vector3 mouseEndDrag;
    PlayerController[] allPlayers;
    PlayerState myState;
	
    void Start()
    {
        if (isLocalPlayer)
        {
            myState = GetComponent<PlayerState>();
            allPlayers = FindObjectsOfType<PlayerController>();
            CmdSetPlayer(allPlayers.Length);
        }
        else
        {
            Init();
        }
    }

    void Init()
    {
        SetPlayer(playerNumber);
    }

    [Command]
    void CmdSetPlayer(int playerNum)
    {
        RpcSetPlayer(playerNum);
    }

    [ClientRpc]
    void RpcSetPlayer(int playerNum)
    {
        SetPlayer(playerNum);
    }

    void SetPlayer(int playerNum)
    {
        if(playerNum != 0)
        {
            name = "Player " + playerNum;
            playerNumber = playerNum;
            playerColor = colorChoices[playerNumber - 1];
        }
    }
   

	// Update is called once per frame
	void Update () {

        if (!isLocalPlayer || myState.Get_State() == SelectionState.building) return;
        Mycast();
    }

    void Mycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Unit u = hit.transform.GetComponent<Unit>();
            if (u != null)
            {
                if (myState.Get_State() == SelectionState.none)
                    myState.SwitchState(SelectionState.hover);
                if (Input.GetMouseButtonDown(0))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        SelectUnit(u);
                    }
                    else
                    {
                        Deselect();
                        SelectUnit(u);
                    }
                }

                if (Input.GetMouseButtonDown(1) && currentUnits.Count != 0)
                {
                    foreach (GameObject g in currentUnits)
                    {
                        CmdAttack(g, u.gameObject);
                    }
                }

            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    selecting = true;
                    startingMousePos = Input.mousePosition;
                    mouseStartDrag = hit.point;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (selecting)
                    {
                        selecting = false;
                        if (!BoxSelect(hit.point) && myState.Get_State() == SelectionState.hover)
                        {
                            myState.SwitchState(SelectionState.none);
                        }
                    }
                }
            }

            if (hit.transform.name == "Ground" && currentUnits.Count != 0)
            {
                if (Input.GetMouseButtonDown(1) && hit.point.y == 0)
                {
                    foreach (GameObject g in currentUnits)
                    {
                        CmdMoveUnit(g, hit.point);
                    }
                }

                if (Input.GetMouseButtonDown(0))
                {
                    Deselect();
                }
            }
        }
    }

    bool BoxSelect(Vector3 hit)
    {
        bool pickedUpUnits = false;
        mouseEndDrag = hit;
        Vector3 midpoint = (mouseStartDrag + mouseEndDrag) / 2;
        float halfX, halfZ;
        halfX = Mathf.Abs(mouseEndDrag.x - midpoint.x);
        halfZ = Mathf.Abs(mouseEndDrag.z - midpoint.z);
        Vector3 halfBox = new Vector3(halfX, 5, halfZ);
        Collider[] cols = Physics.OverlapBox(midpoint, halfBox);

        foreach (Collider c in cols)
        {
            Unit un = c.GetComponent<Unit>();
            if (un != null)
            {
                SelectUnit(un);
                pickedUpUnits = true;
            }
        }

        return pickedUpUnits;
    }

    void Deselect()
    {
        foreach(GameObject u in currentUnits)
        {
            u.GetComponent<Unit>().ToggleIndicator(false);
        }

        currentUnits.Clear();
        myState.SwitchState(SelectionState.none);
    }

    void SelectUnit(Unit u)
    {

        myState.SwitchState(SelectionState.selected);
        currentUnits.Add(u.gameObject);
        u.ToggleIndicator(true);
        u.onDied += OnUnitDied;
    }

    public void OnUnitDied(Unit unit)
    {
        if (currentUnits.Contains(unit.gameObject))
        {
            currentUnits.Remove(unit.gameObject);
        }
        unit.onDied -= OnUnitDied;
    }

    #region Network Commands

    [Command]
    void CmdMoveUnit(GameObject unit, Vector3 newPos)
    {
        unit.GetComponent<Unit>().CmdMoveTo(newPos);
    }

    [Command]
    void CmdAttack(GameObject unit, GameObject target)
    {
        unit.GetComponent<Unit>().CmdAttack(target);
    }
    #endregion

    void OnGUI()
    {
        if (selecting)
        {
            // Create a rect from both mouse positions
            var rect = Utils.GetScreenRect(startingMousePos, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }
}
