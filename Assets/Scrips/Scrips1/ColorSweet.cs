using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSweet : MonoBehaviour
{
    // Start is called before the first frame update
    public enum ColorType
    {
        YELLOW,
        PURPLE,
        RED,
        BLUE,
        GREEN,
        PINK,
        ANY,
        COUNT
    }

    [System.Serializable]
    public struct ColorSprite
    {
        public ColorType color;
        public Sprite sprite;
    }

    public ColorSprite[] ColorSprites;

    private Dictionary<ColorType, Sprite> colorSpriteDict;

    private SpriteRenderer sprite;

    //������ӵ�е���ɫ������
    public int NumColors
    {
        get { return ColorSprites.Length; }
    }

    public ColorType Color
    {
        get
        {
            return color;
        }

        set
        {
            SetColor(value);
        }
    }

    private ColorType color;


    private void Awake()
    {
        sprite = transform.Find("Sweet").GetComponent<SpriteRenderer>();

        colorSpriteDict = new Dictionary<ColorType, Sprite>();

        for (int i = 0; i < ColorSprites.Length; i++)
        {
            if (!colorSpriteDict.ContainsKey(ColorSprites[i].color))
            {
                colorSpriteDict.Add(ColorSprites[i].color, ColorSprites[i].sprite);
            }
        }
    }

    public void SetColor(ColorType newColor)
    {
        color = newColor;
        if (colorSpriteDict.ContainsKey(newColor))
        {
            sprite.sprite = colorSpriteDict[newColor];
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
