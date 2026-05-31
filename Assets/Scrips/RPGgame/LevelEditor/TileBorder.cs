using UnityEngine;

public class TileBorder : MonoBehaviour
{
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("TileBorder"))
        {
            Destroy(this.gameObject);
        }
    }
}
