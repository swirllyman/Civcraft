using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectionState { hover, selected, building, none }
public class PlayerState : MonoBehaviour {

    public  SelectionState selectionState = SelectionState.none;
    
    public void SwitchState(SelectionState newState)
    {
        switch(selectionState)
        {
            case SelectionState.hover:
                newState = SelectionState.hover;
                break;
            case SelectionState.selected:
                newState = SelectionState.selected;
                break;
            case SelectionState.building:
                switch(newState)
                {
                    case SelectionState.hover:
                    case SelectionState.selected:
                    case SelectionState.none:
                        newState = SelectionState.building;
                        break;
                    case SelectionState.building:
                        newState = SelectionState.none;
                        break;
                }
                break;
            case SelectionState.none:
                newState = SelectionState.none;
                break;
        }
        selectionState = newState;
    }

    public SelectionState Get_State()
    {
        return selectionState;
    }
}
