using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DravenRContainer : MonoBehaviour
{

    #region public variables

    [HideInInspector] public bool isAbilityRGoing;
    [HideInInspector] public bool isAbilityRReturning;
    [HideInInspector] public Vector3 direction;

    #endregion

    #region public references

    [SerializeField] Draven dravenScript;

    #endregion

    #region private variables

    IEnumerator returnAxesCoroutine;
    WaitForSeconds waitReturn;

    #endregion

    #region private references

    Axe leftAxeScript;
    Axe rightAxeScript;

    #endregion


    private void Awake()
    {
        leftAxeScript = transform.GetChild(0).GetComponent<Axe>();
        rightAxeScript = transform.GetChild(1).GetComponent<Axe>();
    }

    private void Start()
    {
        waitReturn = new WaitForSeconds(0.5f);
    }

    private void Update()
    {
        if (isAbilityRGoing)
        {
            transform.Translate(6 * Time.deltaTime * direction, Space.World);
        }

        if (isAbilityRReturning)
        {
            transform.Translate(6 * Time.deltaTime * (dravenScript.transform.position - transform.position).normalized, Space.World);
        }
    }

    IEnumerator ReturnAxes()
    {
        dravenScript.DeactivateAbiityR();
        yield return waitReturn;
        isAbilityRGoing = false;
        isAbilityRReturning = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && isAbilityRGoing)
        {
            other.GetComponent<Enemy>().TakeDamage(175);

            //if (returnAxesCoroutine == null)
            //{
            //    returnAxesCoroutine = ReturnAxes();
            //    StartCoroutine(returnAxesCoroutine);
            //}

            StartCoroutine(ReturnAxes());
        }

        if (other.CompareTag("Enemy") && isAbilityRReturning)
        {
            other.GetComponent<Enemy>().TakeDamage(175);
        }

        if (other.CompareTag("Draven") && isAbilityRReturning)
        {
            transform.SetParent(dravenScript.transform);
            
            leftAxeScript.isRotating = false;
            leftAxeScript.ResetAxe();
            
            rightAxeScript.isRotating = false;
            rightAxeScript.ResetAxe();
            
            gameObject.SetActive(false);
        }
    }

}
