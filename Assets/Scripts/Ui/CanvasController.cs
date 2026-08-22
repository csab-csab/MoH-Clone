using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
  [Header("Script References")]
  [SerializeField]private PlayerInteract playerInteract;
  
  [Header("Atlas References")]
  [SerializeField]private TMP_Asset mkSpriteAtlas;
  
  
  [Header("Text References")]
  [Space(10)]
  [Header("Weapon UI References")]
  [SerializeField]private Image crosshair;
  [SerializeField]private TMP_Text weaponName;
  [SerializeField] private TMP_Text ammoText;
  
  [Header("Crosshair References")]
  [SerializeField]private Color standardColor;
  [SerializeField]private Color enemyColor;
  
  [Header("Interact UI References")]
  [SerializeField]private TextMeshProUGUI interactText;
 
  
  void Update()
  {
    UpdateCrosshair(playerInteract.ReturnIsLookingAtEnemy());
  }

  private void UpdateCrosshair(bool isLookingAtEnemy)
  {
    crosshair.color = isLookingAtEnemy ? enemyColor : standardColor;
  }

  public void UpdateWeaponNameText(string newName)
  {
    weaponName.text = newName; 
  }

  public void DisplayReloadText()
  {
    ammoText.color = Color.white;
    ammoText.text = "Reloading...";
  }
  
  public void UpdateAmmoText(int currentAmmo, int carriedAmmo)
  {
    if (currentAmmo <= 0)
    {
      ammoText.text = "Out Of Ammo!";
      ammoText.color = Color.red;
      return;
    }

    if (ammoText.color == Color.red)
    {
      ammoText.color = Color.white;
    }
    ammoText.text = $"{currentAmmo}/{carriedAmmo}";
  }

  
  //Maybe build a dictionary for sprite index lookups?
  //for example:
  //1. build array of sprite indexes (int)
  //2. build array of what button they represent (string)
  //3. assign in inspector
  //3. create dictionary in start()
  public void TestInteractText()
  {
    int index = 82;
    string spriteTag = $"<size=120%><sprite={index}></size>";

    interactText.text = $"Press {spriteTag} to open door";

    RectTransform rectTransform = interactText.GetComponentInChildren<RectTransform>();
    
    if (rectTransform != null)
    {
      rectTransform.localPosition = new Vector3(12, -12, 0);
    }

  }
}
