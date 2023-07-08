using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweetGame : MonoBehaviour
{
    // Start is called before the first frame update
    //��Ʒ����
    private int x;
    private int y;
    [HideInInspector]
    public GameBegin gameBegin;
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
    private GameBegin.SweetType type;

    public GameBegin.SweetType Type 
    {
        get
        {
            return type;
        }
    }
    private SweetMove moveComponent;
    public SweetMove MoveComponent //�ǹ��ƶ�����
    {
        get
        {
            return moveComponent;
        }
    }

    private ColorSweet colorComponent;
    public ColorSweet ColorComponent //�ǹ��Ƿ������ɫ
    {
        get
        {
            return colorComponent;
        }
    }

    

    private ClearSweet clearComponent;
    public ClearSweet ClearComponent 
    {
        get
        {
            return clearComponent;
        }
    }
   
    //�ж��ǹ��Ƿ�����ƶ�
    public bool CanMove()
    {
        return moveComponent != null;
    }
    //�ж��Ƿ������ͼ  
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
        moveComponent = GetComponent<SweetMove>();
        colorComponent = GetComponent<ColorSweet>();
        clearComponent = GetComponent<ClearSweet>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //��ʼ��
    public void Init(int _x,int _y,GameBegin _gameBegin,GameBegin.SweetType _type)
    {
        x = _x;
        y = _y;
        gameBegin = _gameBegin;
        type = _type;
    }
    private void OnMouseEnter()
    {
        gameBegin.EnterSweet(this);
    }

    private void OnMouseDown()
    {
        gameBegin.PressSweet(this);
    }

    private void OnMouseUp()
    {
        gameBegin.ReleaseSweet();
    }
}
