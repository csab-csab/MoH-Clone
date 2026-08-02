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

    void Update()
    {
        if (!Physics.Raycast(rayCastFrom.position, rayCastFrom.forward, out var hit, rayCastRange, raycastMask)) return;
        
        DoorScript doorScript;
        if (hit.transform.TryGetComponent<DoorScript>(out doorScript))
        {
            Debug.Log("Press E to open door");
            if (inputHandler.interactPressed)
            {
                doorScript.InteractWithDoor(); 
            }
        }
    }
}
