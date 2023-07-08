using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweetClearLine : SweetClear
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool isRow;
    public override void Clear()//重写清除方法
    {
        base.Clear();
        if (isRow)
        {
            sweet.gameManger.ClearRow(sweet.Y);
        }
        else
        {
            sweet.gameManger.ClearColumn(sweet.X);
        }
    }
}
