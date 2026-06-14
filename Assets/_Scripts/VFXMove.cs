using Unity.VisualScripting;
using UnityEngine;

public class VFXMove : BaseBehaviour
{
    [SerializeField] protected Vector3 direction = Vector3.up;
    [SerializeField] protected float speed = 5f;
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
    public void SetDirection(Vector3 dir)
    {
        this.direction = dir;
    }

    public void SetRotation(Vector3 rotation)
    {
        transform.rotation = Quaternion.Euler(rotation);
    }
}
