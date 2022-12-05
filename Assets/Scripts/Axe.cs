using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{

    #region public variables

    [HideInInspector] public bool isThrowing;
    [HideInInspector] public bool isSpinningAxeActive;
    [HideInInspector] public bool isLerpAxeCoroutineActive;
    [HideInInspector] public float damage;
    [SerializeField] float speed;
    [HideInInspector] public Vector3 direction;
    [HideInInspector] public GameObject target;

    #endregion

    #region public references

    [SerializeField] GameObject abilityQIndicatorRight;
    [SerializeField] GameObject abilityQIndicatorLeft;

    #endregion

    #region private variables

    bool isRotating;
    float slerpTime;
    Vector3 startPosition;
    Vector3 middlePosition;
    Vector3 endPosition;
    IEnumerator lerpAxeCoroutine;
    WaitForEndOfFrame waitForEndOfFrame;

    #endregion

    #region private references

    Transform rightHandTransform;
    Transform leftHandTransform;
    Draven dravenScript;

    #endregion

    private void Awake()
    {
        rightHandTransform = transform.root.GetComponent<Draven>().rightHandTransform;
        leftHandTransform = transform.root.GetComponent<Draven>().leftHandTransform;
        dravenScript = transform.root.GetComponent<Draven>();
    }

    private void Start()
    {
        slerpTime = 0f;
        waitForEndOfFrame = new WaitForEndOfFrame();
    }

    void Update()
    {
        ThrowAxe();
        RotateAxe();
    }

    IEnumerator LerpAxe()
    {
        isLerpAxeCoroutineActive = true;
        slerpTime = 0f;

        while (slerpTime <= 0.75f)
        {
            slerpTime += Time.deltaTime;
            transform.position = Vector3.Slerp(startPosition, middlePosition, slerpTime * 1/0.75f);
            isSpinningAxeActive = true;
            yield return waitForEndOfFrame;
        }

        slerpTime = 0f;

        while (slerpTime <= 0.75f)
        {
            slerpTime += Time.deltaTime;
            transform.position = Vector3.Slerp(middlePosition, endPosition, slerpTime * 1/0.75f);
            isSpinningAxeActive = true;
            yield return waitForEndOfFrame;
        }

        ResetAxe();
        dravenScript.StopRotatingAxe(transform.name == "RightAxe");

        if (transform.name == "RightAxe")
        {
            abilityQIndicatorRight.transform.SetParent(dravenScript.gameObject.transform);
            abilityQIndicatorRight.SetActive(false);
        }
        else
        {
            abilityQIndicatorLeft.transform.SetParent(dravenScript.gameObject.transform);
            abilityQIndicatorLeft.SetActive(false);
        }

        isLerpAxeCoroutineActive = false;
    }

    void ThrowAxe()
    {
        if (isThrowing && target.transform.CompareTag("Enemy"))
        {
            direction = (target.transform.position - transform.position).normalized;
            transform.Translate(direction * Time.deltaTime * speed, Space.World);
            isRotating = true;
        }
    }

    void RotateAxe()
    {
        if (isSpinningAxeActive || isRotating)
        {
            transform.Rotate(0, -2000 * Time.deltaTime, 0);
        }
    }

    void ResetAxe()
    {
        if (transform.name == "RightAxe")
        {
            isThrowing = false;
            transform.SetParent(rightHandTransform);
            transform.localPosition = new Vector3(0.109325863f, 0.128984153f, 0.0675434992f);
            transform.localEulerAngles = new Vector3(0.097950086f, 255.820236f, 90.128334f);
        }
        else
        {
            isThrowing = false;
            transform.SetParent(leftHandTransform);
            transform.localPosition = new Vector3(-0.108699843f, 0.15922907f, 0.0663153455f);
            transform.localEulerAngles = new Vector3(0.0978390947f, 104.179764f, 269.872131f);
        }
    }

    void FindReturningPoint()
    {
        isThrowing = false;
        startPosition = transform.position;

        if (dravenScript.IsMoving())
        {
            endPosition = dravenScript.gameObject.transform.position + (dravenScript.transform.forward * 4);
        }
        else
        {
            endPosition = dravenScript.gameObject.transform.position;
        }

        if (transform.name == "RightAxe")
        {
            abilityQIndicatorRight.transform.parent = null;
            abilityQIndicatorRight.transform.position = endPosition;
            abilityQIndicatorRight.GetComponent<DravenQIndicator>().isDravenInside = false;
            abilityQIndicatorRight.SetActive(true);
        }
        else
        {
            abilityQIndicatorLeft.transform.parent = null;
            abilityQIndicatorLeft.transform.position = endPosition;
            abilityQIndicatorLeft.GetComponent<DravenQIndicator>().isDravenInside = false;
            abilityQIndicatorLeft.SetActive(true);
        }

        

        middlePosition = Vector3.Lerp(transform.position, endPosition, 0.5f);
        middlePosition += new Vector3(0, 2, 0);
        lerpAxeCoroutine = LerpAxe();
        StartCoroutine(lerpAxeCoroutine);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && isThrowing)
        {
            other.GetComponent<Enemy>().TakeDamage(damage);
            isRotating = false;

            if (isSpinningAxeActive)
            {
                FindReturningPoint();
            }
            else
            {
                ResetAxe();
            }

        }
    }

    public void CatchAxe()
    {
        StopCoroutine(lerpAxeCoroutine);
        isLerpAxeCoroutineActive = false;
        ResetAxe();
        dravenScript.StartRotatingAxe();
        dravenScript.SetAbilityWActive();

        if (transform.name == "RightAxe")
        {
            abilityQIndicatorRight.transform.SetParent(dravenScript.gameObject.transform);
            abilityQIndicatorRight.SetActive(false);
        }
        else
        {
            abilityQIndicatorLeft.transform.SetParent(dravenScript.gameObject.transform);
            abilityQIndicatorLeft.SetActive(false);
        }
    }
}
