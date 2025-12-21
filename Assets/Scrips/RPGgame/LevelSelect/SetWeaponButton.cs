using UnityEngine;

public class SetWeaponButton : MonoBehaviour
{
    public int weaponType;
    public void setWeaponType()
    {
        PlayerPrefs.SetInt("weaponType", weaponType);
    }
}
