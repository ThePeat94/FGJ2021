
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRanged : MonoBehaviour
{
    [SerializeField] private NavMeshAgent m_agent;
    private Transform m_player;

    [SerializeField] private LayerMask m_whatIsGround, m_whatIsPlayer;

    [SerializeField] private float m_health;

    //Patroling
    private Vector3 m_walkPoint;
    bool walkPointSet;
    [SerializeField] private float m_walkPointRange;
    
    [SerializeField] private float m_timeBetweenAttacks;
    bool alreadyAttacked;
    [SerializeField] private GameObject m_projectile;

    //States
    [SerializeField] private float m_sightRange = 15, m_attackRange = 7;
    private bool m_playerInSightRange, m_playerInAttackRange;

    private void Awake()
    {
        m_player = PlayerMovementController.Instance.transform;
        m_agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //Check for sight and attack range
        m_playerInSightRange = Physics.CheckSphere(transform.position, m_sightRange, m_whatIsPlayer);
        m_playerInAttackRange = Physics.CheckSphere(transform.position, m_attackRange, m_whatIsPlayer);
        Debug.Log(m_playerInAttackRange);
        if (!m_playerInSightRange && !m_playerInAttackRange) Patroling();
        if (m_playerInSightRange && !m_playerInAttackRange) ChasePlayer();
        if (m_playerInAttackRange && m_playerInSightRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            m_agent.SetDestination(m_walkPoint);

        Vector3 distanceToWalkPoint = transform.position - m_walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-m_walkPointRange, m_walkPointRange);
        float randomX = Random.Range(-m_walkPointRange, m_walkPointRange);

        m_walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(m_walkPoint, -transform.up, 2f, m_whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        m_agent.SetDestination(m_player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        m_agent.SetDestination(transform.position);

        transform.LookAt(m_player);

        if (!alreadyAttacked)
        {
            var instantiatedProjectile = Instantiate(m_projectile);
            instantiatedProjectile.transform.position = this.transform.position + 2*this.transform.forward;
            instantiatedProjectile.GetComponent<Projectile>().ShootDirection = this.transform.forward;

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), m_timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        m_health -= damage;

        if (m_health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}