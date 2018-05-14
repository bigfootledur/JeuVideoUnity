using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(UnitMove))]
public class UnitAttack : UnitInteraction
{

    public Animator animator;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private GameObject projectile; // If ranged attack

    public float attackTimer = 0;
    public float animationSpeed = 1f;
    private AttackingUnit self;

    [SerializeField] private bool _attackStance = false;
    [SerializeField] private bool _isMoveAttack = false;
    [SerializeField] private RTSGameObject target = null;
    [SerializeField] private Vector3 _attackDestination;
    [SerializeField] private bool onTheMove = false;

    private UnitMove _unitMoveScript;

    private RTSGameObject _tmpEnemy;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        _unitMoveScript = GetComponent<UnitMove>();
        self = GetComponent<AttackingUnit>();
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        animator.speed = animationSpeed / self.AttackSpeed;

        if (target && _unitMoveScript.DestMoveReached)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position), 5f * Time.deltaTime);
            onTheMove = false;
        }

        if(attackTimer > 0)
            attackTimer -= Time.deltaTime;

        if (attackTimer < 0)
            attackTimer = 0;

        if (_attackStance)
        {
            if (target)
            {
                if (attackTimer <= 0)
                {
                    animator.SetTrigger("Attack");
                    audioSource.Play();

                    attackTimer = self.AttackSpeed;
                }
            }
        }

        //if (_isAttacking)
        //{
        if (target != null)
        {
            //_targettedAttack = true;

            if (!onTheMove)
            {
                _unitMoveScript.Move(target.transform.position);
                onTheMove = true;
            }

            // Move to the target
            if (RemainingTargetDistance() >= 0 && RemainingTargetDistance() < self.Range)
            {
                onTheMove = false;
                _unitMoveScript.StopAction();
                _attackStance = true;
            }
            else
            {
                if (!onTheMove)
                {
                    _unitMoveScript.Move(target.transform.position);
                    onTheMove = true;
                }

                _attackStance = false;
            }
        }
        else
        {
            //_targettedAttack = false;

            if (!onTheMove)
            {
                if (!_attackDestination.Equals(Vector3.zero))
                {
                    _unitMoveScript.Move(_attackDestination);
                    onTheMove = true;
                }
            }
        }
    }

    public float RemainingTargetDistance()
    {
        if (target != null)
            return Vector3.Distance(transform.position, target.transform.position);
        else
            return -1;
    }

    public void Attack(RTSGameObject target)
    {
        //_isAttacking = true;
        this.target = target;
    }

    public void MoveAttack(Vector3 position)
    {
        //_isAttacking = true;
        _isMoveAttack = true;
        _attackDestination = position;
    }

    public void Aggro(RTSGameObject enemy)
    {
        if ((_isMoveAttack || GetComponent<AttackingUnit>().doingNothing()) && target == null && enemy != null)
        {
            _tmpEnemy = enemy;
            Invoke("NextAggro", 0.04f);
        }
    }

    public void NextAggro()
    {
        //print("Unit attack : " + _isMoveAttack + " " + GetComponent<RTSGameObject>().doingNothing());
        //if (!_targettedAttack || target == null)
        if ((_isMoveAttack || GetComponent<AttackingUnit>().doingNothing()) && target == null)
        {
            //print("Target it");
            //_targettedAttack = true;
            target = _tmpEnemy;
            onTheMove = false;
            //_isAttacking = true;
        }
    }

    public void StopAction()
    {
        //print("Action stopped");
        //_isAttacking = false;
        _isMoveAttack = false;
        target = null;
        _attackStance = false;
        onTheMove = false;
        _unitMoveScript.StopAction();
        _attackDestination = Vector3.zero;
    }

    #region Getters/Setters
    public bool IsAttacking
    {
        get
        {
            return target != null || _isMoveAttack;
        }
    }
    #endregion

    public void RangeAttack()
    {
        if (target)
        {
            GameObject tmpProjectile = Instantiate(projectile, transform.position, projectile.transform.rotation);
            tmpProjectile.transform.LookAt(target.transform);
            tmpProjectile.GetComponentInChildren<Projectile>().Target = target.transform;
            tmpProjectile.GetComponentInChildren<Projectile>().Damage = self.Damage;
            tmpProjectile.GetComponentInChildren<Projectile>().Sender = this.GetComponent<AttackingUnit>();

            //AudioSource.PlayClipAtPoint(audioSource.clip, transform.position);

            //UnitHealthBar eh = (UnitHealthBar)target.GetComponent("UnitHealthBar");
            //eh.updateHealth(-10);
        }
    }

    public override bool isInAction()
    {
        return onTheMove && (target != null);
    }
}

//if (_isAttacking && target != null)
//{
//    // Add an if clause "isAttackable" (flying, phasing, cloaked, etc)

//    float distance = Vector3.Distance(target.transform.position, transform.position);
//    Vector3 dir = (target.transform.position - transform.position).normalized;

//    float direction = Vector3.Dot(dir, transform.forward);

//    if (distance > 2)
//        transform.position += transform.forward * moveSpeed * Time.deltaTime;

//    if (direction != 0)
//        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position), rotationSpeed * Time.deltaTime);

//    if (distance < 2)
//    {
//        if (attackTimer <= 0)
//        {
//            UnitHealthBar eh = (UnitHealthBar)target.GetComponent("UnitHealthBar");
//            eh.updateHealth(-10);

//            attackTimer = cooldown;
//        }
//    }
//    if (target.GetComponent<UnitHealthBar>().currentHealth == 0)
//        _isAttacking = false;
//}