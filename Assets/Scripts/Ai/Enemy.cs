using System;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.AI;

//Enemy Base Properties will be stored in a JSON so they can be adjusted even after building the game
[System.Serializable]
public class EnemyBaseStats
{
   #region Enemy Properties Json References
   [Header("Stats and Health")]
   public float baseHealth;
   public float baseSpeed;
   public float chaseSpeed;
  
   public float baseDamage;
   [Header("The range at which the enemy goes from idle to patrolling")]
   public float basePatrolRange;
   [Header("This is the range at which the enemy notices the player and tries to get close to them")]
   public float baseChaseRange;
   [Header("The range at which the enemy stops and attacks the player")]
   public float baseAttackRange;
   #endregion

}

public class Enemy : MonoBehaviour
{
   #region Animation Names to hash
   private static readonly int Death = Animator.StringToHash("Status_death");
   #endregion

   public enum State { Idle, Patrol, Chase, Attack, Dead };
   public State currentState;
   
   #region References
   [SerializeField] private EnemyBaseStats enemyBaseStats;
   
   [Header("References")]
   [SerializeField] private NavMeshAgent agent;
   //this animator uses ints because the package I got uses ints, so I didn't want to touch it
   [SerializeField] private Animator animator;
   [SerializeField] private Transform firePoint;
   //Later change this so this automatically gets assigned. maybe through the script that will manage object pooling?
   //Also, if multiplayer is ever added, make this more dynamic;
   //whichever player is the closest will get attacked, maybe.
   [SerializeField] private Transform player;
   #endregion
   
   #region Enemy Properties
   [Header("Enemy Properties")]
   
   [Space(10)]
   
   [Header("Stats and health")]
   [SerializeField]private float currentHealth;
   private float baseHealth => enemyBaseStats.baseHealth;

   [Header("Combat Properties")]
   [SerializeField] private float currentDamage;
   private float baseDamage => enemyBaseStats.baseDamage;
   [SerializeField] private float currentSpeed;
   private float baseSpeed => enemyBaseStats.baseSpeed;
   [SerializeField] private float currentChaseSpeed;
   private float baseChaseSpeed => enemyBaseStats.chaseSpeed;
   [Header("State properties")]
   private float basePatrolRange => enemyBaseStats.basePatrolRange;
   [SerializeField]private float currentPatrolRange;
   private float baseChaseRange => enemyBaseStats.baseChaseRange;
   [SerializeField]private float currentChaseRange;
   private float baseAttackRange => enemyBaseStats.baseAttackRange;
   [SerializeField]private float currentAttackRange;
   [SerializeField] private bool isAllowedToAttack;
  
   //Colliders
   private SphereCollider _headCollider;
   private CapsuleCollider _bodyCollider;

   #endregion


   private void Start()
   {
      IntialiseEnemy();
      
      _headCollider = this.GetComponent<SphereCollider>();
      _bodyCollider = this.GetComponent<CapsuleCollider>();
      
      ToggleColliders(true);
   }


   private void Update()
   {
      DetermineState();
      ExecuteCurrentState();
   }

   private void IntialiseEnemy()
   {
      enemyBaseStats = CentralDataManager.instance.ReturnEnemyBaseStats();
      currentHealth = baseHealth;
      currentDamage = baseDamage;
      currentSpeed = baseSpeed;
      currentChaseSpeed = baseChaseSpeed;
      currentPatrolRange = basePatrolRange;
      currentChaseRange = baseChaseRange;
      currentAttackRange = baseAttackRange;
   }
   
   private void DetermineState()
   {
      if (currentState == State.Dead)
      {
         return;
      }
      
      float distanceToPlayer = Vector3.Distance(transform.position, player.position);

      //Closest
      if (distanceToPlayer <= currentAttackRange && isAllowedToAttack)
      {
         currentState = State.Attack;
      }
      //Medium
      else if (distanceToPlayer <= currentChaseRange)
      {
         currentState = State.Chase;
         currentSpeed = currentChaseSpeed;
      }
      //Far
      else if (distanceToPlayer <= currentPatrolRange)
      {
         currentState = State.Patrol;
         currentSpeed = baseSpeed;
      }
      else
      {
         currentState = State.Idle;
         currentSpeed = baseSpeed;
      }
      
      agent.speed = currentSpeed;
   }

   private void ExecuteCurrentState()
   {
      switch (currentState)
      {
         case State.Idle:
            break;
         
         case State.Patrol:
            PatrolLogic();
            break;
         
         case State.Chase:
            ChaseLogic();
            break;
         
         case State.Attack:
            AttackLogic();
            break;
         
      }
   }
   private void PatrolLogic()
   {
      throw new NotImplementedException();
   }

   private void ChaseLogic()
   {
      agent.SetDestination(player.position);
   }

   private void AttackLogic()
   {
      agent.SetDestination(transform.position); //stop in place
      transform.LookAt(player);
      //Raycast of some sort
      
   }


   /// <summary>
   /// Used to take damage. If hit is passed, damage multiplier is applied for headshots
   /// </summary>
   /// <param name="damage"></param>
   /// <param name="hit"></param>
   public void TakeDamage(float damage, RaycastHit hit = default)
   {
      
      //Headshot multiplier
      if (hit.collider != null)
      {
         //Headshot collider is sphere
         if (hit.collider.GetType() == typeof(SphereCollider))
         {
            damage *= 4;
            print("Headshot!");
         }
      }
      
      currentHealth -= damage;
      if (currentHealth <= 0)
      {
         Die();
      }
   }

   private void Die()
   { 
      currentState = State.Dead;
      
      ResetAllAnimatorParameters(true);

      ToggleColliders(false);
      //Implement object pooling
      Destroy(gameObject, 5f);
   }

   private void ToggleColliders(bool _enabled)
   {
      _headCollider.enabled = _enabled;
      _bodyCollider.enabled = _enabled;
   }
   
   #region Animation Functions

   /// <summary>
   /// Finds and resets all animation params back to default.
   /// This is needed so the death animation can override all the others.
   /// If function is passed with bool argument as true, death anim will play after everything is reset.
   /// </summary>
   private void ResetAllAnimatorParameters(bool dead = false)
   {
      if (animator == null) return;

      foreach (var parameter in animator.parameters)
      {
         //Add more types if needed
         switch (parameter.type)
         {
            case AnimatorControllerParameterType.Int:
               animator.SetInteger(parameter.name, 0);
               break;
            case AnimatorControllerParameterType.Bool:
               animator.SetBool(parameter.name, false);
               break;
         }
      }
      
      if(!dead)return;

      animator.SetInteger(Death, 1);
   }

   #endregion

}
