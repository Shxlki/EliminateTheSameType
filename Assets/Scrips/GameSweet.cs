using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSweet : MonoBehaviour
{
    //甜品坐标
    private int x;
    private int y;
    [HideInInspector]
    public GameManger gameManger;
    public int X
    {
        get
        {
            return x;
        }

        set
        {
            if (CanMove())
            {
                x = value;
            }
        }
    }
    public int Y
    {
        get
        {
            return y;
        }

        set
        {
            if (CanMove())
            {
                y = value;
            }
        }
    }
    private GameManger.SweetType type;

    public GameManger.SweetType Type
    {
        get
        {
            return type;
        }
    }
    private MoveSweet moveComponent;
    public MoveSweet MoveComponent //糖果移动属性
    {
        get
        {
            return moveComponent;
        }
    }

    private SweetColor colorComponent;
    public SweetColor ColorComponent //糖果是否可以着色
    {
        get
        {
            return colorComponent;
        }
    }



    private SweetClear clearComponent;
    public SweetClear ClearComponent
    {
        get
        {
            return clearComponent;
        }
    }

    //判断糖果是否可以移动
    public bool CanMove()
    {
        return moveComponent != null;
    }
    //判断是否可以贴图  
    public bool CanColor()
    {
        return colorComponent != null;
    }

    public bool CanClear()
    {
        return clearComponent != null;
    }

    private void Awake()
    {
        moveComponent = GetComponent<MoveSweet>();
        colorComponent = GetComponent<SweetColor>();
        clearComponent = GetComponent<SweetClear>();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    //初始化
    public void Init(int _x, int _y, GameManger _gameManger, GameManger.SweetType _type)
    {
        x = _x;
        y = _y;
        gameManger = _gameManger;
        type = _type;
    }
    private void OnMouseEnter()
    {
        gameManger.EnterSweet(this);
    }

    private void OnMouseDown()
    {
        gameManger.PressSweet(this);
    }

    private void OnMouseUp()
    {
        gameManger.ReleaseSweet();
    }
}
