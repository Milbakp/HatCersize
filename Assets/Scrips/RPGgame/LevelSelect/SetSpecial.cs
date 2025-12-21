using UnityEngine;

public class SetSpecial : MonoBehaviour
{
    public int specialType;
    public void setSpecialType()
    {
        PlayerPrefs.SetInt("specialType", specialType);
    }
}
