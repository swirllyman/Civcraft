using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum SelectionState { hover, selected, none}
public class PlayerController : NetworkBehaviour {

    public SelectionState selectionState = SelectionState.none;
    public List<GameObject> currentUnits = new List<GameObject>();
    bool selecting;
    Vector3 startingMousePos;
	
	// Update is called once per frame
	void Update () {

        if (!isLocalPlayer) return;
        Mycast();

        if (selecting)
        {
            var camera = Camera.main;
            var viewportBounds = Utils.GetViewportBounds(camera, startingMousePos, Input.mousePosition);
        }
    }

    public bool IsWithinSelectionBounds(GameObject gameObject)
    {
        if (!selecting)
            return false;

        var camera = Camera.main;
        var viewportBounds =
            Utils.GetViewportBounds(camera, startingMousePos, Input.mousePosition);

        return viewportBounds.Contains(
            camera.WorldToViewportPoint(gameObject.transform.position));
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
                if (selectionState == SelectionState.none)
                    selectionState = SelectionState.hover;
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
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    selecting = true;
                    startingMousePos = Input.mousePosition;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (selecting)
                    {
                        selecting = false;
                        if (selectionState == SelectionState.hover)
                        {
                            selectionState = SelectionState.none;
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
