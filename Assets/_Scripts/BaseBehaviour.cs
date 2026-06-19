using UnityEngine;

public abstract class BaseBehaviour : MonoBehaviour
{
    protected virtual void Start()
    {
        this.LoadComponent();
    }

    protected virtual void Awake()
    {
        this.LoadComponent();
    }

    protected virtual void Reset()
    {
        this.ResetValue();
        this.LoadComponent();
    }
    protected virtual void OnEnable()
    {

    }
    protected virtual void OnDisable()
    {
    }
    protected virtual void LoadComponent()
    {

    }

    protected virtual void ResetValue()
    {

    }
}
