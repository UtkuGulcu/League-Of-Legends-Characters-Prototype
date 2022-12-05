using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    #region public variables



    #endregion

    #region public references

    [SerializeField] GameObject enemyPrefab;

    #endregion

    #region private variables

    WaitForSeconds waitEnemySpawn;
    Vector3 enemyPositon;

    #endregion

    #region private references



    #endregion

    private void Start()
    {
        waitEnemySpawn = new WaitForSeconds(3);
        enemyPositon = new Vector3(-4.55999994f, 0.448000014f, 0.59799999f);
    }

    public void SpawnEnemy()
    {
        StartCoroutine(SpawnEnemyCoroutine());
    }

    IEnumerator SpawnEnemyCoroutine()
    {
        yield return waitEnemySpawn;
        Instantiate(enemyPrefab, enemyPositon, Quaternion.identity);
    }
}
