using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearLineSweet : ClearSweet
{
    public bool isRow;
    public override void Clear()//重写清除方法
    {
        base.Clear();
        if(isRow)
        {
            sweet.gameBegin.ClearRow(sweet.Y);
        }
        else
        {
            sweet.gameBegin.ClearColumn(sweet.X);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
