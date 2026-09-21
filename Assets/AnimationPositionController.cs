using UnityEngine;

/// <summary>
/// This script ensures weapon transform is glued to arm as well as returning the weapon specific muzzle flash
/// </summary>
public class AnimationPositionController : MonoBehaviour
{
    [SerializeField] private Transform targetBone; // The hand bone
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private Light muzzleFlashLight;

    private void LateUpdate()
    {
        if (targetBone == null) return;

        // Force position and rotation strictly AFTER the Animator finishes its frame
        transform.position = targetBone.TransformPoint(positionOffset);
        transform.rotation = targetBone.rotation * Quaternion.Euler(rotationOffset);
    }
    
    public ParticleSystem ReturnMuzzleFlash()
    {
        return muzzleFlash;
    }

    public Light ReturnMuzzleFlashLight()
    {
        return muzzleFlashLight;
    }
}
