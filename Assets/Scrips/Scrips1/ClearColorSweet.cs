using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearColorSweet : ClearSweet
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private ColorSweet.ColorType clearColor;//当前要清除的颜色

    public ColorSweet.ColorType ClearColor
    {
        get
        {
            return clearColor;
        }
        set
        {
            clearColor = value;

        }
    }

    public override void Clear()
    {
        base.Clear();
        sweet.gameBegin.ClearColor(clearColor);
    }
}
