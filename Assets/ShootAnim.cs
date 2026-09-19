using UnityEngine;

public class ShootAnim : MonoBehaviour
{
    [SerializeField] private Transform targetBone; // The hand bone
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;

    private void LateUpdate()
    {
        if (targetBone == null) return;

        // Force position and rotation strictly AFTER the Animator finishes its frame
        transform.position = targetBone.TransformPoint(positionOffset);
        transform.rotation = targetBone.rotation * Quaternion.Euler(rotationOffset);
    }

}
