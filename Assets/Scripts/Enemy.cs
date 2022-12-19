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

    IEnumerator getPushedCoroutine;
    Vector3 pushLerpStartPosition;
    Vector3 pushLerpEndPosition;
    float pushLerpTime;
    float pushLerpStartTime;
    WaitForEndOfFrame waitForEndOfFrame;

    #endregion

    #region private references

    GameManager gameManagerScript;

    #endregion

    private void Awake()
    {
        gameManagerScript = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        waitForEndOfFrame = new WaitForEndOfFrame();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"Enemy took {damage} damage");

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

    public void StartGettingPushed(bool isRight)
    {
        if (getPushedCoroutine != null)
        {
            StopCoroutine(getPushedCoroutine);
        }

        pushLerpStartPosition = transform.position;
        pushLerpEndPosition = isRight ? transform.position + Vector3.right : transform.position + Vector3.left;
        pushLerpStartTime = Time.time;
        pushLerpTime = 0f;

        getPushedCoroutine = GetPushed();
        StartCoroutine(getPushedCoroutine);
    }

    IEnumerator GetPushed()
    {
        while (Time.time <= pushLerpStartTime + 0.5f)
        {
            pushLerpTime += Time.deltaTime;
            transform.position = Vector3.Lerp(pushLerpStartPosition, pushLerpEndPosition, pushLerpTime);
            yield return waitForEndOfFrame;
        }
    }
}
