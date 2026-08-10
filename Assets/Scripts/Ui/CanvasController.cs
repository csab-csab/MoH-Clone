using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
  [Header("Script References")]
  [SerializeField]private PlayerInteract playerInteract;
  
  [Header("Text References")]
  [Space(10)]
  [Header("Weapon UI References")]
  [SerializeField]private Image crosshair;
  [SerializeField]private TMP_Text weaponName;
  [SerializeField] private TMP_Text ammoText;
  
  [Header("Crosshair References")]
  [SerializeField]private Color standardColor;
  [SerializeField]private Color enemyColor;
  
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
  
}
