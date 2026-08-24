using UnityEngine;
using System.Collections;

public class M1Garand : Weapon
{
    [Header("M1 Garand Visuals")]
    [Space(10)]
    [SerializeField] private GameObject m1ClipPrefab;

   

    [Header("Special Audio")] 
    [Space(10)]
    [SerializeField] private AudioSource m1PingSource;
 
    private void Awake()
    {
        this.weaponIdentity = WeaponManager.Weapons.M1Garand;
    }
    
    public override void Shoot(Transform raycastFrom)
    {
        base.Shoot(raycastFrom);

        //prevent us seeing reload prompt on interact script
        if (base.isReloadPromptActive)
        {
            base.isReloadPromptActive = false;
            base._canvasController.ClearPromptText();
        }
        
        //don't eject clip if mag is not empty, if we are reloading,
        //or if we are out of ammo
        if (CurrentAmmo > 0 || IsReloading() || isOutOfAmmo)
        {
            return;
        }
        
        EjectClip(raycastFrom);
    }

    public override IEnumerator Reload()
    {
        //Not allowed to reload m1 garand unless empty
        if (CurrentAmmo != 0)
        {
            yield break;
        }
        
        StartCoroutine(base.Reload());
        yield return null;
    }

    
    
    private void EjectClip(Transform raycastFrom)
    {
        //Play clip eject sound
        m1PingSource.Play();
        
        //Create visual effect for clip flying out
        var clip = Instantiate(m1ClipPrefab, ejectionPoint.transform.position, 
            ejectionPoint.transform.rotation);
        
        clip.GetComponent<Rigidbody>().AddForce((raycastFrom.up * ejectionForce) + raycastFrom.right, ForceMode.Impulse);
        
        Destroy(clip, 3);
    }
}
