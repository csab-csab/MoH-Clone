using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

//The base class for all weapons
public class Weapon : MonoBehaviour
{
    private static readonly int Fire = Animator.StringToHash("Base Layer.fire");
    private static readonly int ReloadAnim = Animator.StringToHash("Reload");
    private static readonly int ReloadSpeed = Animator.StringToHash("Reload Speed");

    private const string BaseEquipAnimName = "m1_equip";
    private const string BaseHolsterAnimName = "m1_holster";
    private const string BaseReloadAnimName = "m1_reload";
    

    [Header("Weapon Properties")]
    [Space(10)]
    [SerializeField] private WeaponData weaponData;
    protected int CurrentAmmo;
    private int _currentCarriedAmmo;
    public bool isEmpty => CurrentAmmo <= 0;
    public bool isFull => CurrentAmmo == weaponData.magSize;
    public bool isOutOfAmmo => _currentCarriedAmmo <= 0;
    private bool isReloading { get; set; }
    /// <summary>
    /// Use this value to enable or disable shooting
    /// </summary>
    private bool _allowedToShoot = true;
    
    //The identity of current weapon
    public WeaponManager.Weapons weaponIdentity;
    
    [Tooltip("Objects on these layers will be hit by the raycast")]
    public LayerMask raycastLayers;
    
    //Used for fire rate
    private float _timeSinceLastShot;

    public bool isReloadPromptActive = false;
  

    //Used for debugging the raycast
    private Transform _drawFrom;
    
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [Tooltip("Assign override controller here")]
    [SerializeField] private RuntimeAnimatorController animController;

    [Header("Audio")]
    [Space(10)]
    [Header("Audio Source")]
    [SerializeField] private AudioSource gunFireSource;
    [Header("Audio Clips")]
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioClip emptySound;
    
    [Header("Visuals")]
    [SerializeField] protected Transform ejectionPoint;
    [SerializeField] protected GameObject shellCasingPrefab;
    [Header("Ejection force applied to the clip and shell casing")]
    [SerializeField] protected float ejectionForce;
    
    //Automatically assigned by WeaponManager
    protected CanvasController _canvasController;
    
    private void Start()
    {
        CurrentAmmo = weaponData.magSize;
        _currentCarriedAmmo = weaponData.maxCarryCapacity;
        
        SetupOrSwapAnimator();
    }

    //separate this into spendAmmo(), trigger animation(), can shoot(), perform raycast
    //Virtual allows this method to be overwritten by derived class
    public virtual void Shoot(Transform raycastFrom)
    {
        if (!_allowedToShoot)
        {
            return;
        }
        
        if (CurrentAmmo <= 0)
        {
            PlayEmptyClick();
            Debug.Log("Playing empty click");
            return;
        }

        if (!CanShoot())
        {
            return;
        }

        var hit = CastRay(raycastFrom);

        if (hit.transform != null)
        {
            if (hit.transform.TryGetComponent<Enemy>(out var enemy))
            {
                try
                {
                    enemy.TakeDamage(weaponData.damage, hit);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            
                Debug.Log($"dealt damage{weaponData.damage}");
            } 
        }
        
        //Play animation
        //Play animation
        //This is better than set trigger because when spamming shoot set trigger can lag behind
        //making the animation look poo
        animator.Play(Fire, 0, 0f);

        //Shot sound
        PlayShootSound();
       
        //used to draw raycast gizmo
        _drawFrom = raycastFrom;
        
        EjectShellCasing(raycastFrom);
        
        SpendAmmo();
        
        //Update ui
        if (_canvasController != null)
        {
            _canvasController.UpdateAmmoText(CurrentAmmo, _currentCarriedAmmo);
        }
        
        print($"Current Ammo: {CurrentAmmo}/{_currentCarriedAmmo}");
    }

    public virtual IEnumerator Reload()
    {
        if (isReloading || isOutOfAmmo || CurrentAmmo >= weaponData.magSize)
        {
            yield break;
        }

        isReloading = true;
        var ammoDiff = weaponData.magSize -  CurrentAmmo;
        
        //This ensures that we only take as much ammo as we have
        //if we need 7 rounds for a full mag, but we only have 5 more rounds,
        //we only load 5 and take 5
        var ammoToTake = Mathf.Min(_currentCarriedAmmo, ammoDiff);

        //Used to calculate the speed multiplier. if anim is 2 secs long but the reload takes 4 seconds, it will play at
        //0.5 speed, for example.
        float animSpeedMultiplier = ReturnReloadAnimDuration() / weaponData.reloadTime;

        animator.SetFloat(ReloadAnim, weaponData.reloadTime);
        animator.SetFloat(ReloadSpeed, animSpeedMultiplier);
        animator.SetTrigger(ReloadAnim);
        
        //Update Ui
        if (_canvasController != null)
        {
            _canvasController.DisplayReloadText();
        }

        yield return new WaitForSeconds(weaponData.reloadTime);
        
        CurrentAmmo += ammoToTake;
        _currentCarriedAmmo -= ammoToTake;
        
        isReloading = false;
        
        //Update ui
        if (_canvasController != null)
        {
            _canvasController.UpdateAmmoText(CurrentAmmo, _currentCarriedAmmo);
            
            if (isReloadPromptActive)
            {
                _canvasController.ClearPromptText();
                isReloadPromptActive = false;
            }
        }
    }

    
    private bool CanShoot()
    {
        //calculate fire rate in Seconds Per Round
        var timeBetweenShots = 60f / weaponData.fireRate;

        if ((Time.time-_timeSinceLastShot) >= timeBetweenShots)
        {
            _timeSinceLastShot = Time.time;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SpendAmmo()
    {
        CurrentAmmo--;

        if ( !isReloadPromptActive &&CurrentAmmo <= weaponData.magSize/4)
        {
            isReloadPromptActive = true;
            _canvasController.ShowPrompt("Weapons","Press", "Reload", "reload");
        }
    }

    #region Raycast

    private RaycastHit CastRay(Transform raycastFrom)
    {
        Physics.Raycast(raycastFrom.position, raycastFrom.forward, out var hit, weaponData.range, raycastLayers);
        {
            return hit;
        }
    }

    #endregion
    
    
    #region Visuals
    
    private void EjectShellCasing(Transform raycastFrom)
    {
        //Shell casing ejection logic
        var shellCasing = Instantiate(shellCasingPrefab, ejectionPoint.transform.position, 
            Quaternion.Euler(0, 0, 0));
        
        var shellCasingRb = shellCasing.GetComponent<Rigidbody>();
        shellCasingRb.AddForce((raycastFrom.up * ejectionForce) + (raycastFrom.right * ejectionForce), ForceMode.Impulse);
        
        //Add some random spin so it doesn't look like a static brick
        shellCasingRb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
        
        Destroy(shellCasing, 3); 
    }

    #endregion
    
    #region Audio

    private void PlayShootSound()
    {
        //Randomize pitch between 0.9 and 1.1 for a subtle variation
        gunFireSource.pitch = Random.Range(0.9f, 1.1f);
        //Use one shot to ensure tail of the sound doesn't get cut off when firing multiple times
        gunFireSource.PlayOneShot(fireSound);
    }

    private void PlayEmptyClick()
    {
        gunFireSource.PlayOneShot(emptySound);
    }
    #endregion
    
    #region Getter Functions

    public string ReturnWeaponName()
    {
        return weaponData.weaponName;
    }
    
    public WeaponData.FireType ReturnTypeOfFire()
    {
        return weaponData.fireType;
    }

    public (int _currentAmmo, int _carriedAmmo) ReturnAmmo()
    {
        int currentAmmo = CurrentAmmo;
        int carriedAmmo = _currentCarriedAmmo;
        
        return (currentAmmo, carriedAmmo); 
    }

    public bool IsReloading()
    {
        return isReloading;
    }
    
    //Returns the time it takes to equip weapon
    public float ReturnEquipTime()
    {
        return weaponData.equipTime;
    }

    public float ReturnHolsterTime()
    {
        return weaponData.holsterTime;
    }

    public float ReturnEquipAnimDuration()
    {
        //if the runtime anim controller is an override control which it is guaranteed to be but good to be safe
        if (animator.runtimeAnimatorController is AnimatorOverrideController overrideController)
        {
            AnimationClip originalClip = overrideController[BaseEquipAnimName];

            if (originalClip != null)
            {
                return originalClip.length;
            }
            else
            {
                Debug.LogError("No equip clip was found.");
            }
        }
        
        return 0f;
    }
    
    public float ReturnHolsterAnimDuration()
    {
        //if the runtime anim controller is an override control which it is guaranteed to be but good to be safe
        if (animator.runtimeAnimatorController is AnimatorOverrideController overrideController)
        {
            AnimationClip originalClip = overrideController[BaseHolsterAnimName];

            if (originalClip != null)
            {
                return originalClip.length;
            }
            else
            {
                Debug.LogError("No equip clip was found.");
            }
        }
        
        return 0f;
    }

    private float ReturnReloadAnimDuration()
    {
        //if the runtime anim controller is an override control which it is guaranteed to be but good to be safe
        if (animator.runtimeAnimatorController is AnimatorOverrideController overrideController)
        {
            AnimationClip originalClip = overrideController[BaseReloadAnimName];

            if (originalClip != null)
            {
                return originalClip.length;
            }
            else
            {
                Debug.LogError("No equip clip was found.");
            }
        }
        
        return 0f;
    }
    #endregion

    #region Setter Functions
    /// <summary>
    /// Assigns private reference to the canvas controller in the weapon's script
    /// </summary>
    /// <param name="canvasController"></param>
    public void AssignCanvasControllerReference(CanvasController canvasController)
    {
        _canvasController = canvasController;
    }

    public void SetAllowedToShoot(bool canShoot)
    {
        _allowedToShoot = canShoot;
    }
    #endregion

    #region Setup
    
    /// <summary>
    /// This function either sets up (if it hasn't already)
    /// or changes the runtime animation controller (called on weapon swap)
    /// to the correct override when switching weapon
    /// </summary>
    public void SetupOrSwapAnimator()
    {
        if (animator != null && animController != null)
        {
            animator.runtimeAnimatorController = animController;
        }
        else
        {
            Debug.LogError("No animator or override controller component assigned");
        }
    }

    private void OnEnable()
    {
        if (_canvasController == null)
        {
            return;
        }
        
        if ( !isReloadPromptActive && CurrentAmmo <= weaponData.magSize/4)
        {
            isReloadPromptActive = true;
            _canvasController.ShowPrompt("Weapons","Press", "Reload");
        }
        else
        {
            isReloadPromptActive = false;
            _canvasController.ClearPromptText();
        }
    }
    
      private void OnDisable()
    {
        isReloadPromptActive = false;
        _canvasController.ClearPromptText();
    }

    #endregion
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (_drawFrom != null)
        {
            Gizmos.DrawRay(_drawFrom.position, _drawFrom.forward * weaponData.range);
        }
    }
}
