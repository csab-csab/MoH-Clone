using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] private InputHandler inputHandler;

    [Header("Interaction Properties")]
    [Tooltip("Transform where raycast originates from")]
    [SerializeField] private Transform rayCastFrom;
    [SerializeField] private float rayCastRange;
    [SerializeField] private LayerMask raycastMask;

    [Header("Raycast out variables")] 
    private bool _isLookingAtEnemy;

    void Update()
    {
        if (Physics.Raycast(rayCastFrom.position, rayCastFrom.forward, out var hit, rayCastRange, raycastMask))
        {
            if (hit.transform.TryGetComponent<DoorScript>(out var doorScript))
            {
                Debug.Log("Press E to open door");
                if (inputHandler.interactPressed)
                {
                    doorScript.InteractWithDoor(); 
                }
            } 
        }
        
        //Raycast with longer range for crosshair colour change
        if (Physics.Raycast(rayCastFrom.position, rayCastFrom.forward, out var raycastHit, 1000, raycastMask))
        {
            _isLookingAtEnemy = raycastHit.transform.TryGetComponent<Enemy>(out var enemy); 
        }
    }
    
    #region Getter Functions
    public bool ReturnIsLookingAtEnemy()
    {
        return _isLookingAtEnemy;
    }
    #endregion
}
