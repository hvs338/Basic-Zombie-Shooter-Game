using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    public int Health = 100;
     public NavMeshAgent agent;

    public Transform player;
    public GunController player_with_components;

    public LayerMask whatIsGround, whatIsPlayer;


    public float speed;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public bool isWalking;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;
    public bool isAttacking;
    
    public Animator Zombie_anim;
    

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        player_with_components = GameObject.Find("Player").GetComponent<GunController>();;
        
        agent = GetComponent<NavMeshAgent>();
       
        Zombie_anim =GetComponent<Animator>();

        Zombie_anim.SetBool("idle",true);

        

        
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange){ 
            Patroling();
            Zombie_anim.SetBool("walking",true);
        }
        if (playerInSightRange && !playerInAttackRange){
         ChasePlayer();
         Zombie_anim.SetBool("running",true);
         Zombie_anim.SetBool("attacking",false);

        }

        if (playerInAttackRange && playerInSightRange){ 
        AttackPlayer();
        Zombie_anim.SetBool("attacking",true);
        }
    }

    private void Patroling()

    { 

        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()

    {   
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        
        agent.SetDestination(transform.position);

        transform.LookAt(player);
        
        //Debug.Log("already attacked outside coroutine" + alreadyAttacked.ToString());
        if (alreadyAttacked == false)
        {
            ///Attack code here\


            player_with_components.currentHealth -= 10;
            Debug.Log("hiting");
            player_with_components.Health_bar.SetHealth(player_with_components.currentHealth);

            alreadyAttacked = true;
            StartCoroutine(ResetAttack(2f));
        }
    }
    public IEnumerator ResetAttack(float seconds)
    {   Debug.Log("Waiting");
        
        yield return new WaitForSeconds(seconds);
        alreadyAttacked = !alreadyAttacked;
        Debug.Log(alreadyAttacked);
        

    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        Debug.Log(Health);

        if (Health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }
    private void DestroyEnemy()

    {
        GunController points;
        points = player.GetComponent<GunController>();

        points.playerPoints += 60;
        Destroy(gameObject);
        Debug.Log("Enemy Dead");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
            

        
}
