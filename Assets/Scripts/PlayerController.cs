using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum SelectionState { hover, selected, none}
public class PlayerController : NetworkBehaviour {

    public SelectionState selectionState = SelectionState.none;
    public List<Unit> currentUnits = new List<Unit>();
	
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
                CmdMoveUnits(hit.point);
            }
        }

        if (Input.GetMouseButtonDown(1) && currentUnits.Count != 0)
        {
            Deselect();
        }
    }

    void Deselect()
    {
        foreach(Unit u in currentUnits)
        {
            u.ToggleIndicator(false);
        }
        currentUnits.Clear();
        selectionState = SelectionState.none;
    }

    void SelectUnit(Unit u)
    {
        selectionState = SelectionState.selected;
        currentUnits.Add(u);
        u.ToggleIndicator(true);
    }

    [Command]
    void CmdMoveUnits(Vector3 newPos)
    {
        foreach(Unit u in currentUnits)
        {
            u.GetComponent<Unit>().CmdMoveTo(newPos);
        }
    }
}
