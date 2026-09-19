using UnityEngine;

//Implement default player stats in central data manager
public class PlayerVitals : MonoBehaviour
{
    [Header("References")]
    [SerializeField]CanvasController canvasController;
    
    [Header("Player Stats")]
    [SerializeField]private float baseHealth = 100;
    [SerializeField]private float currentHealth = 100;

    private void Start()
    {
        currentHealth = baseHealth;
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        canvasController.UpdateHealthBar(currentHealth,  baseHealth);
        
        if (currentHealth <= 0)
        {
           Die(); 
        }
    }

    private void Die()
    {
        Debug.Log("Player is dead");
    }
}
