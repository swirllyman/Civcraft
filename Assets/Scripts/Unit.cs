using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;

interface SubscriptionInterface
{
    void Death();
}

public class Unit : NetworkBehaviour {

    public enum AIState { idle, attack, chase, move, flee, none }
    public delegate void OnDied(Unit unit);
    public event OnDied onDied;
    public AIState currentState = AIState.idle;

    [HideInInspector]
    public GameObject target;
    NavMeshAgent agent;
    Health myHealth;

    public GameObject selectedIndicator;
    public float speed = 3.5f;

    [Header("Attack Info")]
    public float attackRange = 5;
    public int attackDamage = 1;
    public float attackSpeed = 1.0f;
    float attackTimer;
    bool attacking = true;
    bool dead = false;
    public int team;

    AttackTriggger attackTrigger;

    // Use this for initialization
    void Start() {
        attackTrigger = GetComponentInChildren<AttackTriggger>();
        agent = GetComponent<NavMeshAgent>();
        selectedIndicator.SetActive(false);
        agent.speed = speed;
        myHealth = GetComponent<Health>();
    }

    void Update()
    {
        if (dead) return;
        if (target != null || (currentState == AIState.idle || currentState == AIState.move))
        {
            CheckState();
        }
        else
        {
            currentState = AIState.idle;
        }
    }

    void CheckState()
    {
        switch (currentState)
        {
            case AIState.idle:
                break;
            case AIState.move:
                if (Vector3.Distance(transform.position, agent.destination) < 5)
                {
                    agent.ResetPath();
                    currentState = AIState.idle;
                }
                break;
            case AIState.chase:
                if (target != null && Vector3.Distance(transform.position, target.transform.position) > attackRange)
                {
                    agent.SetDestination(target.transform.position);
                }
                else
                {
                    if (isServer)
                    {
                        CmdAttack(target);
                    }
                }
                break;
            case AIState.attack:
                if (target != null && Vector3.Distance(transform.position, target.transform.position) <= attackRange)
                {
                    attacking = true;
                    if (attackTimer <= 0 && isServer)
                    {
                        target.GetComponent<Unit>().CmdTakeDamage(attackDamage);
                        if(target.GetComponent<Health>().currentHealth <= attackDamage)
                        {
                            CmdKB();
                        }
                        attackTimer = attackSpeed;
                    }
                    else if(isServer)
                    {
                        attackTimer -= Time.deltaTime;
                    }
                }
                else
                {
                    currentState = AIState.chase;
                }
                break;
        }
    }

    #region Networking
    [Command]
    public void CmdMoveTo(Vector3 pos)
    {
        RpcMoveTo(pos);
    }

    [ClientRpc]
    void RpcMoveTo(Vector3 pos)
    {
        agent.ResetPath();
        agent.SetDestination(pos);
        currentState = AIState.move;
    }

    //This is called from non networked objects
    public void Attack(GameObject g)
    {
        if (isServer)
        {
            CmdAttack(g);
        }
    }

    [Command]
    public void CmdAttack(GameObject g)
    {
        RpcAttack(g);
    }

    [ClientRpc]
    void RpcAttack(GameObject g)
    {
        g.GetComponent<Unit>().onDied += OnTargetDied;
        agent.ResetPath();
        currentState = AIState.attack;
        target = g;
    }

    [Command]
    public void CmdTakeDamage(int damage)
    {
        RpcTakeDamage(damage);
    }

    [ClientRpc]
    void RpcTakeDamage(int damage)
    {
        bool KB = myHealth.TakeDamage(damage);
        if (KB)
        {
            dead = true;
            if (isServer) 
                CmdDeath();
        }
    }

    [Command]
    public void CmdSetup(int teamNum, Color teamColor)
    {
        RpcSetup(teamNum, teamColor);
    }

    [ClientRpc]
    void RpcSetup(int teamNum, Color teamColor)
    {
        GetComponent<Renderer>().material.color = teamColor;
        team = teamNum;
    }

    [Command]
    void CmdKB()
    {
        RpcKB();
    }

    [ClientRpc]
    void RpcKB()
    {
        target = null;
    }

    [Command]
    void CmdDeath()
    {
        RpcDeath();
    }

    [ClientRpc]
    void RpcDeath()
    {
        OnDeath();
    }
    #endregion

    public void OnTargetDied(Unit unit)
    {
        if (dead) return;
        attackTrigger.CheckArea();
        unit.onDied -= OnTargetDied;
    }

    public void OnDeath()
    {
        dead = true;
        if (onDied != null)
            onDied(this);

        StartCoroutine(DieAfterTime());
    }

    IEnumerator DieAfterTime()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }

    public void ToggleIndicator(bool toggle)
    {
        selectedIndicator.SetActive(toggle);
    }

}
