using System;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    
    [Header("Script References")]
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private CanvasController canvasController;

    [Header("Interaction Properties")]
    [Tooltip("Transform where raycast originates from")]
    [SerializeField] private Transform rayCastFrom;
    [SerializeField] private float rayCastRange;
    [SerializeField] private LayerMask raycastMask;

    [Header("Raycast out variables")] 
    private bool _isLookingAtEnemy;

    //Used for caching interactable we are looking at
    //Thought: maybe create an interactable base class all interactables derive from?
    private DoorScript _currentDoorScript;
    

    void Update()
    {
      CheckInteractionRaycast();
    }

    private void CheckInteractionRaycast()
    {  
        //Raycast with longer range for crosshair colour change
        if (Physics.Raycast(rayCastFrom.position, rayCastFrom.forward, out var raycastHit, Mathf.Infinity, raycastMask))
        {
            _isLookingAtEnemy = raycastHit.transform.TryGetComponent<Enemy>(out _); 
        }
        else
        {
            _isLookingAtEnemy = false;
        }
        
        if (Physics.Raycast(rayCastFrom.position, rayCastFrom.forward, out var hit, rayCastRange, raycastMask))
        {
            if (hit.transform.TryGetComponent<DoorScript>(out var doorScript))
            {
                if (_currentDoorScript != doorScript)
                {
                    _currentDoorScript = doorScript;
                    canvasController.ShowPrompt("Player", "Press", "Interact",
                        "Open/Close");
                } 
                
                if (inputHandler.interactPressed)
                {
                    doorScript.InteractWithDoor(); 
                }
                
                return;
            } 
        }

        // Raycast hit nothing or looked away from the door
        if (_currentDoorScript != null)
        {
            _currentDoorScript = null;
            canvasController.ClearPromptText(); // Only runs ONCE when looking away
        }
        
    }
    
    #region Getter Functions
    public bool ReturnIsLookingAtEnemy()
    {
        return _isLookingAtEnemy;
    }
    #endregion
}
