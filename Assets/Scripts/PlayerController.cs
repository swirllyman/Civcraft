using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum SelectionState { hover, selected, none}
public class PlayerController : NetworkBehaviour {

    public SelectionState selectionState = SelectionState.none;
    public List<GameObject> currentUnits = new List<GameObject>();
	
	// Update is called once per frame
	void Update () {

        if (!isLocalPlayer) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Unit u = hit.transform.GetComponent<Unit>();
            if (u != null)
            {
                if(selectionState == SelectionState.none)
                    selectionState = SelectionState.hover;

                if (Input.GetMouseButtonDown(0))
                {
                    SelectUnit(u);
                }
            }
            else if(selectionState == SelectionState.hover)
            {
                selectionState = SelectionState.none;
            }

            if(currentUnits.Count != 0 && hit.transform.name == "Ground" && Input.GetMouseButtonDown(0))
            {
                foreach(GameObject g in currentUnits)
                {
                    CmdMoveUnit(g, hit.point);
                }
                //CmdMoveUnits(currentUnits, hit.point);
            }
        }

        if (Input.GetMouseButtonDown(1) && currentUnits.Count != 0)
        {
            Deselect();
        }
    }

    void Deselect()
    {
        foreach(GameObject u in currentUnits)
        {
            u.GetComponent<Unit>().ToggleIndicator(false);
        }
        currentUnits.Clear();
        selectionState = SelectionState.none;
    }

    void SelectUnit(Unit u)
    {
        selectionState = SelectionState.selected;
        currentUnits.Add(u.gameObject);
        u.ToggleIndicator(true);
    }

    [Command]
    void CmdMoveUnit(GameObject unit, Vector3 newPos)
    {
        unit.GetComponent<Unit>().CmdMoveTo(newPos);
    }
}
