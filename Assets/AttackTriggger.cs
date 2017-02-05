using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTriggger : MonoBehaviour {

    Unit myUnit;

    //float checkTimer = 1.5f;

    SphereCollider myCol;

    void Start()
    {
        myUnit = GetComponentInParent<Unit>();
        myCol = GetComponent<SphereCollider>();
    }

    void Update()
    {
        //if(checkTimer <= 0.0f)
        //{
        //    checkTimer = 1.5f;
        //    checkTimer -= Time.deltaTime;
        //    CheckArea();
        //}
        //else
        //{
        //    checkTimer -= Time.deltaTime;
        //}
    }

    public void CheckArea()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, myCol.radius);
        foreach (Collider c in cols)
        {
            Unit u = c.GetComponent<Unit>();
            if (u != null && u.team != myUnit.team && myUnit.target == null)
            {
                myUnit.Attack(c.gameObject);
            }
        }
    }

	void OnTriggerEnter(Collider c)
    {
        Unit u = c.GetComponent<Unit>();
        if(u != null && u.team != myUnit.team && myUnit.target == null)
        {
            myUnit.Attack(c.gameObject);
        }
    }
}
