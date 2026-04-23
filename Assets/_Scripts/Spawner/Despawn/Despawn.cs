using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Despawn<T> : DespawnBase where T : PoolObj
{
    [SerializeField] protected T parent;
    [SerializeField] protected Spawner<T> spawner;
    // [SerializeField] protected float lifeTime = 5f;
    // [SerializeField] protected float currentTime;
    // [SerializeField] protected bool isNeedDespawn = true;
    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadSpawner();
        this.LoadParent();
        // this.currentTime = this.lifeTime;
    }
    // protected void Update()
    // {
    //     if (!this.isNeedDespawn) return;
    //     this.DespawnByTime();
    // }
    // public void ResetCurrentTime()
    // {
    //     this.currentTime = this.lifeTime;
    // }
    protected void LoadSpawner()
    {
        if (this.spawner != null) return;
        this.spawner = FindAnyObjectByType<Spawner<T>>();
        Debug.Log(transform.name + ": LoadSpawner", gameObject);
    }
    protected virtual void LoadParent()
    {
        if (this.parent != null) return;
        this.parent = transform.parent.GetComponent<T>();
        Debug.Log(transform.name + ": LoadParent", gameObject);
    }
    // protected void DespawnByTime()
    // {
    //     this.currentTime -= Time.deltaTime;
    //     if (this.currentTime > 0) return;

    //     this.currentTime = this.lifeTime;
    //     this.DoDespawn();
    // }

    public override void DoDespawn()
    {
        this.spawner.Despawn(this.parent);
    }
}
