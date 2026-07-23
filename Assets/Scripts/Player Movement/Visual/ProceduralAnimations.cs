using UnityEngine;


public class ProceduralAnimations : MonoBehaviour
{
    [Header("References")]
    [SerializeField]private Vector3 originalPos;
    [SerializeField]private PlayerMovement movementScript;
    
    [Header("Breath Animation Settings")]
    [SerializeField]private float breathDepth = 0.01f;
    [SerializeField]private float breathSpeed = 1.5f;
    
    [Header("Walking Sway Settings")]
    public float walkAmountX = 0.05f; // Side-to-side distance
    public float walkAmountY = 0.03f; // Up-and-down distance
    
    private void Update() 
   {
       if (movementScript.ReturnIsMoving())
       {
           WalkEffect();
       }
       else
       {
           BreathEffect();
       }
       
   }

   //Adds a universal breathe in breathe out effect to arm rig
   private void BreathEffect()
   {
       // A simple Sine wave creates a smooth up-and-down "bob"
       var sway = Mathf.Sin(Time.time * breathSpeed) * breathDepth; 
       transform.localPosition = originalPos + new Vector3(0, sway, 0);
   }

   private void WalkEffect()
   {
       var walkSwayX = Mathf.Sin(Time.time * movementScript.ReturnCurrentMoveSpeed()) * walkAmountX;
       var walkSwayY = Mathf.Cos(Time.time * movementScript.ReturnCurrentMoveSpeed() * 2f) * walkAmountY; // Double speed for the "step"
       
       var targetWalkPos = new Vector3(walkSwayX, walkSwayY, 0);
       
       transform.localPosition = originalPos + targetWalkPos;
   }
}
