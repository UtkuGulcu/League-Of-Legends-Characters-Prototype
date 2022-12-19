using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draven : Character
{

    #region public variables

    public bool isRightAxeRotating { get; private set; }
    public bool isLeftAxeRotating { get; private set; }

#endregion

    #region public references

    [SerializeField] GameObject rightAxe;
    [SerializeField] GameObject leftAxe;
    public Transform rightHandTransform;
    public Transform leftHandTransform;
    public GameObject abilityEContainer;
    public DravenRContainer abilityRContainerScript;

    #endregion

    #region private variables

    WaitForSeconds waitSpeed;
    WaitForSeconds waitAbilityQ;
    WaitForSeconds waitAbilityE;
    WaitForSeconds waitAbilityR;
    float extraMovementSpeed;
    float extraAttackSpeed;
    float abilityWActivationTime;
    float abilityWLerpTime;
    float abilityWLerpStart;
    float abilityWLerpEnd;
    IEnumerator changeSpeedCoroutine;
    IEnumerator leftAxeCoroutine;
    IEnumerator rightAxeCoroutine;
    bool isChangeSpeedCoroutineRunning;
    bool isAbilityWActive;
    Vector3 abilityRDirection;

    #endregion

    #region private references

    Axe rightAxeScript;
    Axe leftAxeScript;

    #endregion

    protected override void Awake()
    {
        base.Awake();
        rightAxeScript = rightAxe.GetComponent<Axe>();
        leftAxeScript = leftAxe.GetComponent<Axe>();
        abilityEContainer = transform.GetChild(4).gameObject;
    }

    protected override void Start()
    {
        base.Start();
        waitSpeed = new WaitForSeconds(3);
        waitAbilityQ = new WaitForSeconds(5.8f);
        waitAbilityE = new WaitForSeconds(1.05f);
        waitAbilityR = new WaitForSeconds(0.567f);
        rightAxeScript.damage = attackDamage;
        leftAxeScript.damage = attackDamage;
    }

    protected override void Update()
    {
        base.Update();
        ReadSkillInput();
        HandleLerps();
    }

    void HandleLerps()
    {
        //Decaying movement speed bonus from Ability W
        if (isAbilityWActive && Time.time <= abilityWActivationTime + 3)
        {
            abilityWLerpTime += Time.deltaTime;
            movementSpeed = Mathf.Lerp(abilityWLerpStart, abilityWLerpEnd, abilityWLerpTime / 3);
            navMeshAgent.speed = movementSpeed;
        }
    }

    void ReadSkillInput()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isAbilityQOnCooldown)
        {
            PerformAbilityQ();
        }

        if (Input.GetKeyDown(KeyCode.W) && !isAbilityWOnCooldown)
        {
            PerformAbilityW();
        }

        if (Input.GetKeyDown(KeyCode.E) && !isAbilityEOnCooldown)
        {
            PerformAbilityE();
        }

        if (Input.GetKeyDown(KeyCode.R) && !isAbilityROnCooldown)
        {
            if (!abilityRContainerScript.isAbilityRGoing)
            {
                PerformAbilityR();
            }
            else
            {
                abilityRContainerScript.isAbilityRGoing = false;
                abilityRContainerScript.isAbilityRReturning= true;

                DeactivateAbiityR();
            }
            
        }
    }

    protected override void PerformAbilityQ()
    {
        base.PerformAbilityQ();

        // Both axes are not rotating
        if (!isLeftAxeRotating && !isRightAxeRotating)
        {
            //Rotate right axe
            isRightLastAttack = false;
            rightAxeCoroutine = RotateRightAxe();
            StartCoroutine(rightAxeCoroutine);
            return;
        }

        //Only right axe is rotating
        if (isRightAxeRotating && !isLeftAxeRotating)
        {
            //Rotate left axe
            leftAxeCoroutine = RotateLeftAxe();
            StartCoroutine(leftAxeCoroutine);

            //Reset the rotation of right axe
            StopCoroutine(rightAxeCoroutine);
            rightAxeCoroutine = RotateRightAxe();
            StartCoroutine(rightAxeCoroutine);

            return;
        }

        //Only left axe is rotating
        if (isLeftAxeRotating && !isRightAxeRotating)
        {
            //Rotate right axe
            rightAxeCoroutine = RotateRightAxe();
            StartCoroutine(rightAxeCoroutine);

            //Reset the rotation of left axe
            StopCoroutine(leftAxeCoroutine);
            leftAxeCoroutine = RotateLeftAxe();
            StartCoroutine(leftAxeCoroutine);
        }

        //Both axes are rotating
        if (isLeftAxeRotating && isRightAxeRotating)
        {
            //Reset the rotation of both axes 
            StopCoroutine(leftAxeCoroutine);
            leftAxeCoroutine = RotateLeftAxe();
            StartCoroutine(leftAxeCoroutine);

            StopCoroutine(rightAxeCoroutine);
            rightAxeCoroutine = RotateRightAxe();
            StartCoroutine(rightAxeCoroutine);

            return;
        }
    }

    protected override void PerformAbilityW()
    {
        base.PerformAbilityW();

        if (isChangeSpeedCoroutineRunning)
        {
            StopCoroutine(changeSpeedCoroutine);
            isAbilityWActive = false;
            movementSpeed = abilityWLerpEnd;
            navMeshAgent.speed = movementSpeed;
            attackSpeed -= extraAttackSpeed;
            waitAttack = new WaitForSeconds(1 / attackSpeed);
            animatorPlayer.SetFloat("attackAnimationMultiplier", attackSpeed);
        }

        changeSpeedCoroutine = ChangeSpeed();
        StartCoroutine(changeSpeedCoroutine);
    }

    protected override void PerformAbilityE()
    {
        base.PerformAbilityE();
        StartCoroutine(PlayAbilityEAnimation());
    }

    protected override void PerformAbilityR()
    {
        //base.PerformAbilityR();
        StartCoroutine(PlayAbilityRAnimation());
    }

    IEnumerator RotateLeftAxe()
    {
        leftAxeScript.isSpinningAxeActive = true;
        leftAxeScript.damage = attackDamage + (attackDamage * 0.4f);
        isLeftAxeRotating = true;
        yield return waitAbilityQ;
        isLeftAxeRotating = false;
        leftAxeScript.isSpinningAxeActive = false;
        leftAxeScript.damage = attackDamage;
        leftAxe.transform.localPosition = new Vector3(-0.108699843f, 0.15922907f, 0.0663153455f);
        leftAxe.transform.localEulerAngles = new Vector3(0.0978390947f, 104.179764f, 269.872131f);
    }

    IEnumerator RotateRightAxe()
    {
        rightAxeScript.isSpinningAxeActive = true;
        rightAxeScript.damage = attackDamage + (attackDamage * 0.4f);
        isRightAxeRotating = true;
        yield return waitAbilityQ;
        isRightAxeRotating = false;
        rightAxeScript.isSpinningAxeActive = false;
        rightAxeScript.damage = attackDamage;
        rightAxe.transform.localPosition = new Vector3(0.109325863f, 0.128984153f, 0.0675434992f);
        rightAxe.transform.localEulerAngles = new Vector3(0.097950086f, 255.820236f, 90.128334f);
    }

    IEnumerator ChangeSpeed()
    {
        isChangeSpeedCoroutineRunning = true;
        abilityWActivationTime = Time.time;
        abilityWLerpTime = 0;
        extraMovementSpeed = movementSpeed * 0.5f;
        movementSpeed += extraMovementSpeed;
        navMeshAgent.speed = movementSpeed;
        abilityWLerpStart = movementSpeed;
        abilityWLerpEnd = movementSpeed - extraMovementSpeed;
        isAbilityWActive = true;

        extraAttackSpeed = attackSpeed * 0.2f;
        attackSpeed += extraAttackSpeed;
        waitAttack = new WaitForSeconds(1 / attackSpeed);
        animatorPlayer.SetFloat("attackAnimationMultiplier", attackSpeed);

        yield return waitSpeed;

        isChangeSpeedCoroutineRunning = false;
        isAbilityWActive = false;
        attackSpeed -= extraAttackSpeed;
        waitAttack = new WaitForSeconds(1 / attackSpeed);
        animatorPlayer.SetFloat("attackAnimationMultiplier", attackSpeed);
    }

    IEnumerator PlayAbilityEAnimation()
    {
        navMeshAgent.destination = transform.position;
        crosshairInfo = cameraScript.GetMouseInfo();
        transform.LookAt(crosshairInfo.mousePosition, Vector3.up);

        abilityEContainer.GetComponent<DravenEContainer>().lerpStartPosition = transform.position;
        Vector3 direction = Vector3.zero;

        if (crosshairInfo.hitObject.CompareTag("Ground"))
        {
            //abilityEContainer.GetComponent<DravenEContainer>().lerpEndPosition = (crosshairInfo.mousePosition - transform.position).normalized * 6.4f;
            direction = (crosshairInfo.mousePosition - transform.position).normalized * 6.4f;
        }
        else
        {
            if (Physics.Raycast(crosshairInfo.mousePosition, Vector3.down, out RaycastHit hit, 10f, ~LayerMask.GetMask("Enemy")))
            {
                //abilityEContainer.GetComponent<DravenEContainer>().lerpEndPosition = (hit.point - transform.position).normalized * 6.4f;
                direction = (hit.point - transform.position).normalized * 6.4f;
            }
        }

        abilityEContainer.GetComponent<DravenEContainer>().lerpEndPosition = transform.position + direction;

        abilityEContainer.GetComponent<DravenEContainer>().lerpEndPosition += new Vector3(0f, 0.35f, 0f);
        animatorPlayer.SetBool("isPerformingE", true);
        yield return waitAbilityE;
        animatorPlayer.SetBool("isPerformingE", false);
    }

    IEnumerator PlayAbilityRAnimation()
    {
        navMeshAgent.destination = transform.position;
        crosshairInfo = cameraScript.GetMouseInfo();
        transform.LookAt(crosshairInfo.mousePosition, Vector3.up);

        if (crosshairInfo.hitObject.CompareTag("Ground"))
        {
            abilityRDirection = (crosshairInfo.mousePosition - transform.position).normalized;
        }
        else
        {
            if (Physics.Raycast(crosshairInfo.mousePosition, Vector3.down, out RaycastHit hit, 100f, ~LayerMask.GetMask("Enemy")))
            {
                abilityRDirection = (hit.point - transform.position).normalized;
            }
        }

        abilityRContainerScript.direction = abilityRDirection;

        leftAxeScript.isRotating = true;
        rightAxeScript.isRotating = true;

        animatorPlayer.SetBool("isPerformingR", true);
        yield return waitAbilityR;
        animatorPlayer.SetBool("isPerformingR", false);
    }

    //Animation event for auto attack
    void ThrowAxeAnimEvent()
    {
        if (target == null)
        {
            return;
        }

        if (isRightLastAttack)
        {
            rightAxe.transform.parent = null;
            rightAxe.transform.rotation = Quaternion.LookRotation((target.transform.position - rightAxe.transform.position).normalized, Vector3.up);
            rightAxe.transform.eulerAngles += new Vector3(0, 0, 90);
            StartThrowingAxe(true);
        }
        else
        {
            leftAxe.transform.parent = null;
            leftAxe.transform.rotation = Quaternion.LookRotation((target.transform.position - leftAxe.transform.position).normalized, Vector3.up);
            leftAxe.transform.eulerAngles += new Vector3(0, 0, 90);
            StartThrowingAxe(false);
        }
    }

    void StartThrowingAxe(bool isRight)
    {
        if (isRight && target.CompareTag("Enemy"))
        {
            rightAxeScript.target = target;
            rightAxeScript.isThrowing = true;
        }
        else if (!isRight && target.CompareTag("Enemy"))
        {
            leftAxeScript.target = target;
            leftAxeScript.isThrowing = true;
        }
    }

    //Animation event for Ability E
    void ThrowAxesAnimEvent()
    {
        abilityEContainer.SetActive(true);
        abilityEContainer.transform.parent = null;
        abilityEContainer.transform.position = transform.position;
        abilityEContainer.transform.position += new Vector3(0, 0.45f, 0);
        abilityEContainer.GetComponent<DravenEContainer>().activationTime = Time.time;
        abilityEContainer.GetComponent<DravenEContainer>().lerpTime = 0f;

        leftAxe.transform.parent = abilityEContainer.transform;
        leftAxe.transform.localPosition += new Vector3(0.24f, 0, 0);

        rightAxe.transform.parent = abilityEContainer.transform;
        rightAxe.transform.localPosition -= new Vector3(0.24f, 0, 0);

        StartCoroutine(abilityEContainer.GetComponent<DravenEContainer>().LerpAxes());
    }

    //Animation event for Ability R
    void AbilityRAnimEvent()
    {
        abilityRContainerScript.transform.SetParent(null);
        leftAxe.transform.SetParent(abilityRContainerScript.transform);
        rightAxe.transform.SetParent(abilityRContainerScript.transform);
        abilityRContainerScript.gameObject.SetActive(true);

        if (Physics.Raycast(leftAxe.transform.position, Vector3.down, out RaycastHit hitLeft, 10f))
        {
            leftAxe.transform.position = hitLeft.point;
            leftAxe.transform.forward = hitLeft.normal;
        }

        if (Physics.Raycast(rightAxe.transform.position, Vector3.down, out RaycastHit hitRight, 10f))
        {
            rightAxe.transform.position = hitRight.point;
            rightAxe.transform.forward = hitRight.normal;
        }

        abilityRContainerScript.isAbilityRGoing = true;
    }


    public float GetAttackDamage()
    {
        return attackDamage;
    }

    public void StartRotatingAxe()
    {
        if (isRightAxeRotating)
        {
            StopCoroutine(rightAxeCoroutine);
            rightAxeCoroutine = RotateRightAxe();
            StartCoroutine(rightAxeCoroutine);
        }

        if (isLeftAxeRotating)
        {
            StopCoroutine(leftAxeCoroutine);
            leftAxeCoroutine = RotateLeftAxe();
            StartCoroutine(leftAxeCoroutine);
        }
    }

    public void StopRotatingAxe(bool isRight)
    {
        if (isRight)
        {
            StopCoroutine(rightAxeCoroutine);
            isRightAxeRotating = false;
            rightAxeScript.isSpinningAxeActive = false;
            rightAxeScript.damage = attackDamage;
        }
        else
        {
            StopCoroutine(leftAxeCoroutine);
            isLeftAxeRotating = false;
            leftAxeScript.isSpinningAxeActive = false;
            leftAxeScript.damage = attackDamage;
        }
    }

    public void DeactivateAbiityR()
    {
        StartCoroutine(SetAbilityROnCooldown());
    }

    public void SetAbilityWActive()
    {
        isAbilityWOnCooldown = false;
    }
}
