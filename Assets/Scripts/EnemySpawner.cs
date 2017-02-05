using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject enemyPrefab;
    public int numberOfEnemies;

    public float spawnTimer = 0.0f;

    int teamNum;
    Color teamColor;

    NetworkManagerHUDCustom hud;

    void Start()
    {
        hud = FindObjectOfType<NetworkManagerHUDCustom>();
    }

    [Server]
    public void Init(Color c, int team)
    {
        CmdInit(c, team);
        StartCoroutine(SpawnAfterTime());
    }

    [Command]
    void CmdInit(Color c, int team)
    {
        RpcInit(c, team);
    }

    [ClientRpc]
    void RpcInit(Color c, int team)
    {
        GetComponent<Renderer>().material.color = c;
        teamColor = c;
        teamNum = team;
    }

    IEnumerator SpawnAfterTime()
    {
        yield return new WaitForSeconds(spawnTimer);
        for (int i = 0; i < numberOfEnemies; i++)
        {
            var spawnPosition = gameObject.transform.position + new Vector3(Random.Range(-8.0f, 8.0f), 0.0f, Random.Range(-8.0f, 8.0f));

            var spawnRotation = Quaternion.Euler(0.0f, Random.Range(0, 180), 0.0f);

            var enemy = (GameObject)Instantiate(enemyPrefab, spawnPosition, spawnRotation);
            NetworkServer.Spawn(enemy);
            CmdSetupUnit(enemy);
        }
    }

    [Command]
    void CmdSetupUnit(GameObject g)
    {
        g.GetComponent<Unit>().CmdSetup(teamNum, teamColor);
        RpcSetupUnit();

    }

    [ClientRpc]
    void RpcSetupUnit()
    {
        hud.AddUnit();
    }
}