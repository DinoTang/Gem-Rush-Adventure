using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardAbstract : BaseBehaviour
{
    [Header("BoardAbstract")]
    [SerializeField] protected BoardManager boardManager;

    protected override void LoadComponent()
    {
        base.LoadComponent();
        this.LoadBoardManager();
    }
    protected void LoadBoardManager()
    {
        if (this.boardManager != null) return;
        this.boardManager = transform.parent.GetComponent<BoardManager>();
        Debug.Log(transform.name + ": LoadBoardManager");
    }
}
