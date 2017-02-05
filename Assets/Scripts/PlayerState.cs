using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectionState { hover, selected, building, none }
public class PlayerState : MonoBehaviour {

    public SelectionState selectionState = SelectionState.none;
    
    public void SwitchState(SelectionState newState)
    {
        switch(newState)
        {
            case SelectionState.hover:
                selectionState = SelectionState.hover;
                break;
            case SelectionState.selected:
                selectionState = SelectionState.selected;
                break;
            case SelectionState.building:
                switch(selectionState)
                {
                    case SelectionState.hover:
                    case SelectionState.selected:
                    case SelectionState.none:
                        selectionState = SelectionState.building;
                        break;
                    case SelectionState.building:
                        selectionState = SelectionState.none;
                        break;
                }
                break;
            case SelectionState.none:
                selectionState = SelectionState.none;
                break;
        }
    }

    public SelectionState Get_State()
    {
        return selectionState;
    }
}
