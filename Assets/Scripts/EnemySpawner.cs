using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject enemyPrefab;
    public int numberOfEnemies;

    public float spawnTimer = 5.0f;
    void Start()
    {
        if (!isServer) return;
        StartCoroutine(SpawnAfterTime());
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
        RpcSetupUnit(g);
    }

    [ClientRpc]
    void RpcSetupUnit(GameObject g)
    {
        g.GetComponent<Renderer>().material.color = GetComponent<Renderer>().material.color;
    }
}