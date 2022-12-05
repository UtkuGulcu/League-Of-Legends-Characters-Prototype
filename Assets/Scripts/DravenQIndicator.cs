using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DravenQIndicator : MonoBehaviour
{

    #region public variables

    [HideInInspector] public bool isDravenInside;

    #endregion

    #region public references

    [SerializeField] Axe leftAxeScript;
    [SerializeField] Axe rightAxeScript;

    #endregion

    #region private variables



    #endregion

    #region private references



    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Draven"))
        {
            isDravenInside = true;
        }

        if ((other.CompareTag("RightAxe") && rightAxeScript.isLerpAxeCoroutineActive) && isDravenInside)
        {
            rightAxeScript.CatchAxe();
        }

        if ((other.CompareTag("LeftAxe") && leftAxeScript.isLerpAxeCoroutineActive) && isDravenInside)
        {
            leftAxeScript.CatchAxe();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Draven"))
        {
            isDravenInside = false;
        }
    }
}
