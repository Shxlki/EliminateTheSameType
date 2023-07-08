using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweetColorClear : SweetClear
{
    private SweetColor.ColorType clearColor;//当前要清除的颜色

    public SweetColor.ColorType ClearColor
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
        sweet.gameManger.ClearColor(clearColor);
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
