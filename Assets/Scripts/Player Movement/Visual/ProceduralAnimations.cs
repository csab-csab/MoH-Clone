using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;


public class ProceduralAnimations : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private Vector3 originalPos;

    [SerializeField] private PlayerMovement movementScript;

    [Header("Breath Animation Settings")] [SerializeField]
    private float breathDepth = 0.01f;

    [SerializeField] private float breathSpeed = 1.5f;

    [Header("Walking Sway Settings")] public float walkAmountX = 0.05f; // Side-to-side distance
    public float walkAmountY = 0.03f; // Up-and-down distance

    [Header("Death Animation Settings")]
    [SerializeField]private float collapseSpeed = 4f;
    [Header("In degrees per second")]
    [SerializeField] private float headRotationSpeed = 180f;
    

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
        var walkSwayY =
            Mathf.Cos(Time.time * movementScript.ReturnCurrentMoveSpeed() * 2f) *
            walkAmountY; // Double speed for the "step"

        var targetWalkPos = new Vector3(walkSwayX, walkSwayY, 0);

        transform.localPosition = originalPos + targetWalkPos;
    }
    
    public void TriggerDeathAnimation(Transform cameraPivot, bool isCrouching)
    {
        StartCoroutine(DeathAnimation(cameraPivot, isCrouching));
    }

    private IEnumerator DeathAnimation(Transform cameraPivot, bool isCrouching)
    {
        float targetCollapsePos;
        
        yield return new WaitForSeconds(0.5f);
        
        //Stage 1: Collapse into kneeling position if not crouching
        if (!isCrouching)
        {
            targetCollapsePos = cameraPivot.localPosition.y / 2;
            
            while (!Mathf.Approximately(cameraPivot.localPosition.y, targetCollapsePos))
            {
                cameraPivot.localPosition = new Vector3(cameraPivot.localPosition.x,
                    Mathf.MoveTowards(cameraPivot.localPosition.y, targetCollapsePos, 
                        collapseSpeed * Time.deltaTime),
                    cameraPivot.localPosition.z);
                //Adds delay, with the exact duration depending on the frame rate
                //E.g.: at 60fps its 16ms
                //This makes this essentially the same as running in update
                yield return null;
            }

        }
        
        yield return new WaitForSeconds(1);
     
        //Stage 2: Collapse onto the ground and turn head to the side
        
        targetCollapsePos = 0;
        Quaternion targetRot = Quaternion.Euler(cameraPivot.localEulerAngles.x, cameraPivot.localEulerAngles.y, 90f);
        
        //Slight x offset to make it look like we toppled over
        float xOffset = -1f;

        while (Quaternion.Angle(cameraPivot.localRotation, targetRot) > 0.05f ||
               !Mathf.Approximately(cameraPivot.localPosition.y, targetCollapsePos))
        {
            //Added x and y pos because as a one-liner it looked super uglyy
            float xPos = Mathf.MoveTowards(cameraPivot.localPosition.x, xOffset,
                collapseSpeed * Time.deltaTime);

            float yPos = Mathf.MoveTowards(cameraPivot.localPosition.y, targetCollapsePos,
                collapseSpeed * Time.deltaTime);

            cameraPivot.localPosition = new Vector3(xPos, yPos, cameraPivot.localPosition.z);
            
            //Tilt camera 90 degrees to make it look like player collapsed
            cameraPivot.localRotation = Quaternion.RotateTowards(cameraPivot.localRotation, targetRot, 
                headRotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
}