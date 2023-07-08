using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSweet : MonoBehaviour
{
    // Start is called before the first frame update
    private GameSweet sweet;

    private IEnumerator moveCoroutine;//得到其他指令的时候可以终止这个协程

    private void Awake()
    {
        sweet = GetComponent<GameSweet>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //开启或结束一个协程
    public void Move(int newX, int newY, float time)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);

    }
    private IEnumerator MoveCoroutine(int newX, int newY, float time)
    {
        sweet.X = newX;
        sweet.Y = newY;
        //每一帧移动一点
        Vector3 startPos = transform.position;
        Vector3 endPos = sweet.gameManger.BlockPosition(newX, newY);
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            sweet.transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0;
        }
        sweet.transform.position = endPos;
    }
}
