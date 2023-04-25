using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garen : Character
{
    private WaitForSeconds waitMovementSpeedIncrease;
    private IEnumerator changeMovementSpeedCoroutine;
    private bool isChangeMovementSpeedCoroutineRunning;
    private float extraMovementSpeed;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        waitMovementSpeedIncrease = new WaitForSeconds(1f);
    }

    protected override void Update()
    {
        base.Update();
        ReadSkillInput();
    }

    private void ReadSkillInput()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isAbilityQOnCooldown)
        {
            PerformAbilityQ();
        }
    }

    protected override void PerformAbilityQ()
    {
        //base.PerformAbilityQ();
        if (isChangeMovementSpeedCoroutineRunning)
        {
            StopCoroutine(changeMovementSpeedCoroutine);
            movementSpeed -= extraMovementSpeed;
        }
        
        changeMovementSpeedCoroutine = ChangeMovementSpeed();
        StartCoroutine(changeMovementSpeedCoroutine);
    }

    private IEnumerator ChangeMovementSpeed()
    {
        isChangeMovementSpeedCoroutineRunning = true;
        extraMovementSpeed = movementSpeed * 0.35f;
        movementSpeed += extraMovementSpeed;
        yield return waitMovementSpeedIncrease;
        isChangeMovementSpeedCoroutineRunning = false;
        movementSpeed -= extraMovementSpeed;
    }
}
