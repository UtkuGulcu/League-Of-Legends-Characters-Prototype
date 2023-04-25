using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    protected bool isAbilityQOnCooldown;
    protected bool isAbilityWOnCooldown;
    protected bool isAbilityEOnCooldown;
    protected bool isAbilityROnCooldown;
    protected WaitForSeconds waitAttack;
    protected GameObject target;

    [SerializeField] protected Animator animatorPlayer;
    protected NavMeshAgent navMeshAgent;
    protected MouseInfo crosshairInfo;
    protected CameraFollow cameraScript;

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

    [HideInInspector] public bool isRightLastAttack;

    [Header("References")]
    [SerializeField] private GameObject arrow;

    private bool isWalking;
    private bool isWalkingToTarget;
    private bool isAttacking;
    private WaitForSeconds waitArrow;
    private WaitForSeconds waitAbilityQ;
    private WaitForSeconds waitAbilityW;
    private WaitForSeconds waitAbilityE;
    private WaitForSeconds waitAbilityR;
    private IEnumerator playAttackAnimationCoroutine;


    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        //animatorPlayer = GetComponent<Animator>();
        cameraScript = GetComponent<CameraFollow>();
    }

    protected virtual void Start()
    {
        waitArrow = new WaitForSeconds(0.42f);
        waitAttack = new WaitForSeconds(1 / attackSpeed);
        waitAbilityQ = new WaitForSeconds(abilityQCooldown);
        waitAbilityW = new WaitForSeconds(abilityWCooldown);
        waitAbilityE = new WaitForSeconds(abilityECooldown);
        waitAbilityR = new WaitForSeconds(abilityRCooldown);
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
        //If Right Mouse Button is pressed
        if (Input.GetMouseButtonDown(1))
        {
            crosshairInfo = cameraScript.GetMouseInfo();

            if (crosshairInfo.isOnEnemy && !isAttacking)
            {
                if (Vector3.Distance(transform.position, crosshairInfo.hitObject.transform.position) > attackRange)
                {
                    //Move towards target and then attack again
                    Vector3 direction = (crosshairInfo.hitObject.transform.position - transform.position).normalized;
                    Vector3 distanceToTargetPosition = direction * (Vector3.Distance(transform.position, crosshairInfo.hitObject.transform.position) - attackRange);
                    navMeshAgent.destination = transform.position + distanceToTargetPosition;
                    isWalkingToTarget = true;
                }
                else
                {
                    Attack();
                }
            }
            else if (!crosshairInfo.isOnEnemy)
            {
                StopCoroutine(SpawnArrow());
                StartCoroutine(SpawnArrow());
            }
        }

        //If Right Mouse Button is being pressed
        if (Input.GetMouseButton(1))
        {
            crosshairInfo = cameraScript.GetMouseInfo();
            transform.LookAt(crosshairInfo.mousePosition, Vector3.up);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

            if (!crosshairInfo.isOnEnemy)
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
    }

    protected virtual void PerformAbilityQ()
    {
        //Handle mana consumption
        StartCoroutine(SetAbilityQOnCooldown());
    }

    protected virtual void PerformAbilityW()
    {
        //Handle mana consumption
        StartCoroutine(SetAbilityWOnCooldown());
    }

    protected virtual void PerformAbilityE()
    {
        //Handle mana consumption
        StartCoroutine(SetAbilityEOnCooldown());
    }

    protected virtual void PerformAbilityR()
    {
        //Handle mana consumption
        StartCoroutine(SetAbilityROnCooldown());
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

    IEnumerator SetAbilityEOnCooldown()
    {
        isAbilityEOnCooldown = true;
        yield return waitAbilityE;
        isAbilityEOnCooldown = false;
    }

    protected IEnumerator SetAbilityROnCooldown()
    {
        isAbilityROnCooldown = true;
        yield return waitAbilityR;
        isAbilityROnCooldown = false;
    }

    void HandleMovement()
    {
        navMeshAgent.SetDestination(crosshairInfo.mousePosition);
    }

    void Attack()
    {
        navMeshAgent.destination = transform.position;
        target = crosshairInfo.hitObject;

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


        if (Vector3.Distance(transform.position, target.transform.position) <= attackRange + 0.1f)
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
        GameObject deletedArrow = Instantiate(arrow, crosshairInfo.mousePosition, Quaternion.identity);
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
                transform.LookAt(crosshairInfo.hitObject.transform.position, Vector3.up);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
        }
    }

    public bool IsMoving()
    {
        return isWalking;
    }
}
