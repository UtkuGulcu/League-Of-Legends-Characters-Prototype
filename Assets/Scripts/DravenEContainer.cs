using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DravenEContainer : MonoBehaviour
{

    #region public variables

    [HideInInspector] public Vector3 lerpStartPosition;
    [HideInInspector] public Vector3 lerpEndPosition;
    [HideInInspector] public float lerpTime;
    [HideInInspector] public float activationTime;
    [SerializeField] GameObject draven;

    #endregion

    #region public references

    [SerializeField] GameObject leftAxe;
    [SerializeField] GameObject rightAxe;

    #endregion

    #region private variables

    WaitForEndOfFrame waitForEndOfFrame;

    #endregion

    #region private references



    #endregion

    private void Start()
    {
        waitForEndOfFrame = new WaitForEndOfFrame();
    }

    public IEnumerator LerpAxes()
    {
        leftAxe.GetComponent<Axe>().isAbilityEActive = true;
        rightAxe.GetComponent<Axe>().isAbilityEActive = true;

        while (Time.time <= activationTime + 1)
        {
            lerpTime += Time.deltaTime;
            transform.position = Vector3.Lerp(lerpStartPosition, lerpEndPosition, lerpTime);
            yield return waitForEndOfFrame;
        }

        gameObject.SetActive(false);
        transform.SetParent(draven.transform);
        leftAxe.GetComponent<Axe>().ResetAxe();
        rightAxe.GetComponent<Axe>().ResetAxe();
        leftAxe.GetComponent<Axe>().isAbilityEActive = false;
        rightAxe.GetComponent<Axe>().isAbilityEActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().StartGettingPushed(!((transform.position.x - other.transform.position.x) > 0));
            other.GetComponent<Enemy>().TakeDamage(175);
        }
    }
}
