using UnityEngine;

public class Enemy : MonoBehaviour
{
   #region Animation Names to hash
   private static readonly int Death = Animator.StringToHash("Status_death");
   #endregion
  
   
   public float health = 100;

   //this animator uses ints because the package I got uses ints, so I didn't want to touch it
   [SerializeField]Animator animator;
   
   public void TakeDamage(float damage)
   {
      health -= damage;
      if (health <= 0)
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
