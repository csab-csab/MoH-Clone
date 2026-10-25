using System;
using UnityEngine;

//Implement default player stats in central data manager
public class PlayerVitals : MonoBehaviour
{
    [Header("References")]
    [SerializeField]CanvasController canvasController;
    private Vector3 _recoil;
    
    //References for death
    private InputHandler _inputHandler;
    private PlayerMovement _playerMovement;
    private ProceduralAnimations _proceduralAnimation;
    private PlayerLook _playerLook;
    
    [SerializeField] private GameObject armRig;
    [SerializeField] private Transform cameraPivot;
    

    [Header("Player Stats")]
    [SerializeField]private float baseHealth = 100;
    [SerializeField]private float currentHealth = 100;

    
    private void Start()
    {
        currentHealth = baseHealth;
        
        _inputHandler = GetComponent<InputHandler>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerLook = GetComponent<PlayerLook>();

        if (armRig != null)
        {
            _proceduralAnimation = armRig.GetComponent<ProceduralAnimations>();
        }
        else
        {
            print("ProceduralAnimations script cannot be found. Make sure arm rig is assigned as a ref in " +
                  "this script.");
        }
    }

    private void Update()
    {
        if(_inputHandler.interactPressed == true)
        {
         Die();   
        }
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
        //disable input handler so player can't do anything
        _inputHandler.enabled = false; 
        
        //disable weapons and etc
        foreach (Transform child in armRig.transform)
        {
            child.gameObject.SetActive(false);
        }
        
        //Disable player look so it doesn't overwrite anim
        _playerLook.enabled = false;

        //disable hud
        
        //slight black tint

        _proceduralAnimation.TriggerDeathAnimation(cameraPivot, _playerMovement.ReturnIsCrouching());
    }
}
