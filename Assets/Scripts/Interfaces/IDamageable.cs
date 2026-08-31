using UnityEngine;


//This interface is required by objects that are damageable by weapon raycasts
public interface IDamageable
{
    void TakeDamage(float damage, RaycastHit hitInfo);
}
