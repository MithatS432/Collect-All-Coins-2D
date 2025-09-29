using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 3.35f, -10f);

    private void LateUpdate()
    {
        transform.position = target.position + offset;
    }
}
