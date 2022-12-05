using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{

    #region protected variables

    protected bool isAttacking;
    protected bool isOnEnemy;
    protected bool isRightLastAttack;
    protected bool isAbilityQOnCooldown;
    protected bool isAbilityWOnCooldown;
    protected WaitForSeconds waitAttack;
    protected GameObject hitObject;
    protected GameObject target;

    #endregion

    #region protected references

    protected Animator animatorPlayer;
    protected NavMeshAgent navMeshAgent;

    #endregion

    #region public variables

    [Header("Character Stats")]
    [SerializeField] float health;
    [SerializeField] protected float attackDamage;
    [SerializeField] float attackRange;
    [SerializeField] protected float movementSpeed;
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float abilityQCooldown;
    [SerializeField] protected float abilityWCooldown;
    [SerializeField] protected float abilityECooldown;
    [SerializeField] protected float abilityRCooldown;

    #endregion

    #region public references

    [Header("References")]
    [SerializeField] GameObject arrow;

    #endregion

    #region private variables

    bool isWalking;
    bool isWalkingToTarget;
    MouseInfo crosshairInfo;
    WaitForSeconds waitArrow;
    WaitForSeconds waitAbilityQ;
    WaitForSeconds waitAbilityW;
    Vector3 mousePosition;
    IEnumerator playAttackAnimationCoroutine;

    #endregion

    #region private references



    #endregion

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animatorPlayer = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        waitArrow = new WaitForSeconds(0.42f);
        waitAttack = new WaitForSeconds(attackSpeed);
        waitAbilityQ = new WaitForSeconds(abilityQCooldown);
        waitAbilityW = new WaitForSeconds(abilityWCooldown);
        navMeshAgent.speed = movementSpeed;
    }

    protected virtual void Update()
    {
        ReadInput();
        DestinationCheck();
        AttackDistanceCheck();
    }

    void ReadInput()
    {
        //If Right Mouse Button is being pressed
        if (Input.GetMouseButton(1))
        {
            crosshairInfo = GetComponent<MyCamera>().GetMouseInfo();
            mousePosition = crosshairInfo.mousePosition;
            transform.LookAt(mousePosition, Vector3.up);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            isOnEnemy = crosshairInfo.isOnEnemy;
            hitObject = crosshairInfo.hitObject;

            if (!isOnEnemy)
            {
                if (isAttacking)
                {
                    StopCoroutine(playAttackAnimationCoroutine);
                    animatorPlayer.SetBool("isRightAttacking", false);
                    animatorPlayer.SetBool("isLeftAttacking", false);
                    isAttacking = false;
                }

                HandleMovement();
            }
        }

        //If Right Mouse Button is pressed
        if (Input.GetMouseButtonDown(1))
        {
            if (isOnEnemy && !isAttacking)
            {
                if (Vector3.Distance(transform.position, hitObject.transform.position) > attackRange)
                {
                    //Move towards target and then attack again
                    Vector3 direction = (hitObject.transform.position - transform.position).normalized;
                    Vector3 distanceToTargetPosition = direction * (Vector3.Distance(transform.position, hitObject.transform.position) - attackRange);
                    navMeshAgent.destination = transform.position + distanceToTargetPosition;
                    isWalkingToTarget = true;
                }
                else
                {
                    Attack();
                }
            }
            else if (!isOnEnemy)
            {
                StopCoroutine(SpawnArrow());
                StartCoroutine(SpawnArrow());
            }
        }
    }

    protected virtual void PerformAbilityQ()
    {
        //This Method will be filled when taking care of cooldowns and/or mana
        StartCoroutine(SetAbilityQOnCooldown());
    }

    protected virtual void PerformAbilityW()
    {
        //Take care of mana
        StartCoroutine(SetAbilityWOnCooldown());
    }

    protected virtual void PerformAbilityE()
    {
        //This Method will be filled when taking care of cooldowns and/or mana
    }

    protected virtual void PerformAbilityR()
    {
        //This Method will be filled when taking care of cooldowns and/or mana
    }

    IEnumerator SetAbilityQOnCooldown()
    {
        isAbilityQOnCooldown = true;
        yield return waitAbilityQ;
        isAbilityQOnCooldown = false;
    }

    IEnumerator SetAbilityWOnCooldown()
    {
        isAbilityWOnCooldown = true;
        yield return waitAbilityW;
        isAbilityWOnCooldown = false;
    }

    void HandleMovement()
    {
        navMeshAgent.SetDestination(mousePosition);
    }

    void Attack()
    {
        navMeshAgent.destination = transform.position;
        target = hitObject;

        if (isRightLastAttack)
        {
            StopCoroutine(playAttackAnimationCoroutine);
            playAttackAnimationCoroutine = PlayAttackAnimation(false);
            StartCoroutine(playAttackAnimationCoroutine);
        }
        else
        {
            if (playAttackAnimationCoroutine != null)
            {
                StopCoroutine(playAttackAnimationCoroutine);
            }
            
            playAttackAnimationCoroutine = PlayAttackAnimation(true);
            StartCoroutine(playAttackAnimationCoroutine);
        }
    }

    IEnumerator PlayAttackAnimation(bool isRight)
    {
        if (target == null)
        {
            StopCoroutine(playAttackAnimationCoroutine);
            yield return null;
        }

        if (isRight)
        {
            isRightLastAttack = true;
            isAttacking = true;
            animatorPlayer.SetBool("isRightAttacking", true);
            yield return waitAttack;
            animatorPlayer.SetBool("isRightAttacking", false);
            isAttacking = false;
        }
        else
        {
            isRightLastAttack = false;
            isAttacking = true;
            animatorPlayer.SetBool("isLeftAttacking", true);
            yield return waitAttack;
            animatorPlayer.SetBool("isLeftAttacking", false);
            isAttacking = false;
        }

        if (target == null)
        {
            StopCoroutine(playAttackAnimationCoroutine);
            yield return null;
        }


        if (Vector3.Distance(transform.position, target.transform.position) <= attackRange)
        {
            StopCoroutine(playAttackAnimationCoroutine);
            playAttackAnimationCoroutine = PlayAttackAnimation(!isRight);
            StartCoroutine(playAttackAnimationCoroutine);
        }
        else
        {
            //Move towards target and then attack again
            Vector3 direction = (target.transform.position - transform.position).normalized;
            Vector3 distanceToTargetPosition = direction * (Vector3.Distance(transform.position, target.transform.position) - attackRange);
            navMeshAgent.destination = transform.position + distanceToTargetPosition;
            isWalkingToTarget = true;
        }
    }

    IEnumerator SpawnArrow()
    {
        GameObject deletedArrow = Instantiate(arrow, mousePosition, Quaternion.identity);
        yield return waitArrow;
        Destroy(deletedArrow);
    }

    void DestinationCheck()
    {
        isWalking = navMeshAgent.destination != transform.position;
        animatorPlayer.SetBool("isWalking", isWalking);
    }

    void AttackDistanceCheck()
    {
        if (isWalkingToTarget)
        {
            if (transform.position == navMeshAgent.destination)
            {
                isWalkingToTarget = false;
                Attack();
            }
            else
            {
                transform.LookAt(hitObject.transform.position, Vector3.up);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
        }
    }

    public bool IsMoving()
    {
        return isWalking;
    }
}
