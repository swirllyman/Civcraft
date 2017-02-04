using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectionState { hover, selected, building, none }
public class PlayerState : MonoBehaviour {

    public SelectionState selectionState = SelectionState.none;
    
    public void SwitchState(SelectionState newState)
    {

    }
}
