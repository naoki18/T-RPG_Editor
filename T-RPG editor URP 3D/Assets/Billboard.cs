using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] bool lockXZAxis = true;
    private void Update()
    {
        if(lockXZAxis)
        {
            transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
        }
        else
        {
            transform.rotation = Camera.main.transform.rotation;
        }

    }
}
