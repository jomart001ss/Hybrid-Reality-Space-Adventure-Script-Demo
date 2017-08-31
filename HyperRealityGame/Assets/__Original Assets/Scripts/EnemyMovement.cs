using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    [HideInInspector]
    public Transform target;

    public float attackRange;
    public float minAttSpeed, maxAttSpeed;

    protected Vector3 tempTarget; //when the actual target isn't in sight
    public float attInvertval;
    protected float counter;
    protected EnemyAttack attackComp;

    public float roamRange;
    public float minRoamSpeed, maxRoamSpeed;
    protected float speed;
    public float roamInterval;

    protected delegate void stateHandler();
    protected stateHandler state;

    public void Init()
    {
        attackComp = gameObject.GetComponent<EnemyAttack>();
        speed = Random.Range(minRoamSpeed, maxRoamSpeed);
        target = Player.instance.positionTransform;
        SwitchToRoaming();
    }

    void Update()
    {
        attackComp.target = Player.instance.position;
        state();
    }

    protected void Roaming()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackRange)
        {
            SwitchToAttacking();
        }
        else
        {
            counter -= Time.deltaTime;
            Move(speed, tempTarget);
            if (counter <= 0 || transform.position == tempTarget)
            {
                counter = roamInterval;
                tempTarget = GetAPosNearTarget(target, roamRange);
                speed = Random.Range(minRoamSpeed, maxRoamSpeed);
            }
        }
    }

    public void SwitchToAttacking()
    {
        UpdateAttackStats();
        attackComp.state = attackComp.attacking;
        state = Attacking;
    }

    public void SwitchToRoaming()
    {
        tempTarget = GetAPosNearTarget(target, roamRange);
        speed = Random.Range(minRoamSpeed, maxRoamSpeed);
        counter = roamInterval;
        attackComp.state = attackComp.idle;
        state = Roaming;
    }

    protected void Attacking()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > attackRange)
        {
            SwitchToRoaming();
            return;
        }

        Move(speed, tempTarget);
        counter -= Time.deltaTime;
        if (counter <= 0)
        {
            UpdateAttackStats();
        }
    }

    protected void UpdateAttackStats()
    {
        counter = attInvertval;
        speed = Random.Range(minAttSpeed, maxAttSpeed);
        tempTarget = GetAPosNearTarget(target, attackRange);
    }

    protected void Move(float spd, Vector3 position)
    {
        Vector3 newPos = Vector3.MoveTowards(transform.position, position, spd * Time.deltaTime);
        transform.position = newPos;
        transform.LookAt(target);
    }

    protected Vector3 GetAPosNearTarget(Transform target, float _range)
    {
        Vector3 randomPositionInSphere = Vector3.Normalize(Random.insideUnitSphere) * _range + target.position;
        Vector3 shift = target.forward * ((_range/2) + 5f);
        shift = Vector3.zero; //debug
        Vector3 position = randomPositionInSphere + shift;
        return  position;
    }
}
