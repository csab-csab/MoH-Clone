using System;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.AI;

//Enemy Base Properties will be stored in a json so they can be adjusted even after building the game
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

   public enum State { Idle, Patrol, Chase, Attack };
   public State currentState;
   
   #region References
   [SerializeField] private EnemyBaseStats enemyBaseStats;
   
   [Header("References")]
   [SerializeField] private NavMeshAgent agent;
   //this animator uses ints because the package I got uses ints, so I didn't want to touch it
   [SerializeField] private Animator animator;
   [SerializeField] private Transform firePoint;
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
   #endregion


   private void Start()
   {
      enemyBaseStats = CentralDataManager.instance.ReturnEnemyBaseStats();
   }
   

   public void TakeDamage(float damage)
   {
      currentHealth -= damage;
      if (currentHealth <= 0)
      {
         Die();
      }
   }
   
   private void Die()
   { 
      ResetAllAnimatorParameters(true);
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
