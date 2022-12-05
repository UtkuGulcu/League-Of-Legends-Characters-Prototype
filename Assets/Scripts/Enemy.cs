using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    #region public variables

    public float health;

    #endregion

    #region public references



    #endregion

    #region private variables



    #endregion

    #region private references

    GameManager gameManagerScript;

    #endregion

    private void Awake()
    {
        gameManagerScript = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        gameManagerScript.SpawnEnemy();
        Destroy(gameObject);
    }
}
