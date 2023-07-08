using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameBegin : MonoBehaviour
{
    // Start is called before the first frame update
    private static GameBegin _instance;
    public static GameBegin Instance
    {
        get
        {
            return _instance;
        }

        set
        {
            _instance = value;

        }
    }
    public int x;//列
    public int y;//行
    /*游戏UI显示*/
    //填充时间
    public float fillTime;
    //时间显示
    public Text timeText;

    private float gameTime = 60;

    private bool gameOver;//游戏结束

    public int playerScore;//玩家的得分

    public Text PlayerScore;//玩家分数显示

    private float addScoreTime;//控制每一帧时间

    private float currentScore;//当前得分

    public GameObject gameOverPanel;//面板

    public Text finalScoreText;//最后得分

    public Text finalScoreText1;

    public GameObject gameNextPanel;//下一关面板





    //获取格子预制体
    public GameObject gridPrefab;

    //甜品数组
    private SweetGame[,] sweets;

    //要交换的两个甜品对象
    private SweetGame pressedSweet;
    private SweetGame enteredSweet;

    //种类
    public enum SweetType
    {
        EMPTY,//空
        NORMAL,//普通
        BARRIER,//障碍
        ROW_CLEAR,//列消除
        COLUMN_CLEAR,//行消除
        RAINBOWANDY,//彩虹糖
        COUNT//标记类型
    }
    //甜品预置体的字典，通过甜品的类型来得到对应甜品游戏物体
    public Dictionary<SweetType, GameObject> sweetPrefabDict;

    [System.Serializable]
    public struct SweetPrefab//结构体
    {
        public SweetType type;
        public GameObject prefab;
    }
    public SweetPrefab[] sweetPrefabs;
    
    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        //字典实例化
        sweetPrefabDict = new Dictionary<SweetType, GameObject>();
        for(int i=0;i<sweetPrefabs.Length;i++)
        {
            if(!sweetPrefabDict.ContainsKey(sweetPrefabs[i].type))
            {
                sweetPrefabDict.Add(sweetPrefabs[i].type, sweetPrefabs[i].prefab);
            }
        }
        for(int i=0;i<x;i++)
        {
            for(int j=0;j<y;j++)
            {//BlockPosition(i,j)
                GameObject block = Instantiate(gridPrefab, BlockPosition(i,j), Quaternion.identity);
                block.transform.SetParent(transform);
            }
        }
        //sw = new GameObject[x, y];

        sweets = new SweetGame[x, y];
        for (int i=0;i<x;i++)
        {
            for(int j=0;j<y;j++)
            {
                CreateNewSweet(i, j, SweetType.EMPTY);
            }
        }

        Destroy(sweets[4, 4].gameObject);
        CreateNewSweet(4, 4, SweetType.BARRIER);
        
        StartCoroutine(AllFill());
    }

    // Update is called once per frame
    void Update()
    {
        
        gameTime -= Time.deltaTime;
        if(gameTime<=0&& playerScore<100)
        {
            gameTime = 0;
            //显示失败面板，播放失败面板动画
            gameOverPanel.SetActive(true);
            finalScoreText.text = playerScore.ToString();
            gameOver = true;
        }
        if(gameTime<=0&&playerScore>=100)
        {
            gameTime = 0;
            //显示下一关面板
            gameNextPanel.SetActive(true);
            finalScoreText1.text = playerScore.ToString();
            gameOver = true;

        }
        timeText.text = gameTime.ToString("0");
        if(addScoreTime<=0.05f)
        {
            addScoreTime += Time.deltaTime;//计时器累加
        }
        else
        {
            if(currentScore<playerScore)//当前得分累加
            {
                currentScore++;
                PlayerScore.text = currentScore.ToString();
                addScoreTime = 0;
            }
        }
        



    }
    public Vector2 BlockPosition(int i,int j)//格子位置
    {
        return new Vector2(transform.position.x - x / 2 + i, transform.position.y + y / 2 - j);
    }

    //产生甜品的方法
    public SweetGame CreateNewSweet(int x,int y,SweetType type)
    {
        GameObject newSweet = Instantiate(sweetPrefabDict[type], BlockPosition(x, y), Quaternion.identity);
        newSweet.transform.parent = transform;
        sweets[x, y] = newSweet.GetComponent<SweetGame>();
        sweets[x, y].Init(x, y, this, type);
        return sweets[x, y];

    }

    //全部填充
    public IEnumerator AllFill()
    {
        bool needRefill = true;//消除后是否需要填充
        while(needRefill)
        {
            yield return new WaitForSeconds(fillTime);
            //填充未完成一直填充
            while(Fill())
            {
                yield return new WaitForSeconds(fillTime);
            }
            //清除匹配好的所有甜品
            needRefill = ClearAllMatchedSweet();
        }
        
    }

    //分步填充
    public bool Fill()
    {
        bool isFill = false;//判断本次填充是否完成
        for(int i=y-2;i>=0;i--)
        {
            for(int j=0;j<x; j++)
            {
                SweetGame sweet = sweets[j, i];//当前元素位置的甜品对象
                if(sweet.CanMove())//如果无法移动，则无法向下填充
                {
                    SweetGame sweetBelow = sweets[j, i + 1];
                    if(sweetBelow.Type==SweetType.EMPTY)//垂直填充
                    {
                        Destroy(sweetBelow.gameObject);
                        sweet.MoveComponent.Move(j, i + 1,fillTime);
                        sweets[j,i + 1] = sweet;
                        CreateNewSweet(j, i, SweetType.EMPTY);
                        isFill = true;
                    }
                    else//斜向填充
                    {
                        for (int down = 0; down <= 1; down++)
                        {
                            if (down != 0)
                            {
                                int downX = j + down;
                                if (downX >= 0 && downX < x)
                                {
                                    SweetGame downSweet = sweets[downX, i + 1];
                                    if (downSweet.Type == SweetType.EMPTY)
                                    {
                                        bool isfill = true;//是否可以斜下填充
                                        for (int upY = i; upY >= 0; upY--)
                                        {
                                            SweetGame sweetUp = sweets[downX, upY];
                                            if (sweetUp.CanMove())
                                            {
                                                break;
                                            }
                                            else if (!sweetUp.CanMove() && sweetUp.Type != SweetType.EMPTY)
                                            {
                                                isfill = false;
                                                break;
                                            }
                                        }
                                        if (!isfill)
                                        {
                                            Destroy(downSweet.gameObject);
                                            sweet.MoveComponent.Move(downX, i + 1, fillTime);
                                            sweets[downX, i + 1] = sweet;
                                            CreateNewSweet(j, i, SweetType.EMPTY);
                                            isFill = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
            }
        }

        //最上排-1行
        for(int i=0;i<x;i++)
        {
            SweetGame sweet = sweets[i, 0];
            if(sweet.Type==SweetType.EMPTY)
            {
                GameObject newSweet= Instantiate(sweetPrefabDict[SweetType.NORMAL], BlockPosition(i,-1), Quaternion.identity);
                newSweet.transform.parent = transform;
                sweets[i, 0] = newSweet.GetComponent<SweetGame>();
                sweets[i, 0].Init(i, -1, this, SweetType.NORMAL);
                sweets[i, 0].MoveComponent.Move(i, 0, fillTime);
                sweets[i, 0].ColorComponent.SetColor((ColorSweet.ColorType)Random.Range(0, sweets[i, 0].ColorComponent.NumColors));
                isFill = true;
            }
        }

        return isFill;
    }

    private bool IsFriend(SweetGame sweet1,SweetGame sweet2)//判断甜品是否相邻
    {
        return (sweet1.X == sweet2.X && Mathf.Abs(sweet1.Y - sweet2.Y) == 1) || (sweet1.Y == sweet2.Y && Mathf.Abs(sweet1.X - sweet2.X) == 1);
    }

    //交换两个甜品的方法
    private void ExchangeSweets(SweetGame sweet1,SweetGame sweet2)
    {
        if(sweet1.CanMove()&&sweet2.CanMove())
        {
            sweets[sweet1.X, sweet1.Y] = sweet2;
            sweets[sweet2.X, sweet2.Y] = sweet1;


            if(MatchSweets(sweet1,sweet2.X,sweet2.Y)!=null||MatchSweets(sweet2,sweet1.X,sweet1.Y)!=null||sweet1.Type==SweetType.RAINBOWANDY||sweet2.Type==SweetType.RAINBOWANDY)
            {
                int tempX = sweet1.X;
                int tempY = sweet1.Y;

                sweet1.MoveComponent.Move(sweet2.X, sweet2.Y, fillTime);
                sweet2.MoveComponent.Move(tempX, tempY, fillTime);
                if(sweet1.Type==SweetType.RAINBOWANDY&&sweet1.CanClear()&&sweet2.CanClear())//彩虹糖消除同类型元素
                {
                    ClearColorSweet clearColor = sweet1.GetComponent<ClearColorSweet>();
                    if(clearColor!=null)
                    {
                        clearColor.ClearColor = sweet2.ColorComponent.Color;
                    }
                    ClearSweet(sweet1.X, sweet1.Y);

                }

                if(sweet2.Type==SweetType.RAINBOWANDY&&sweet2.CanClear()&&sweet1.CanClear())
                {
                    ClearColorSweet clearColor = sweet2.GetComponent<ClearColorSweet>();
                    if(clearColor!=null)
                    {
                        clearColor.ClearColor = sweet1.ColorComponent.Color;
                    }
                    ClearSweet(sweet2.X, sweet2.Y);
                }


                ClearAllMatchedSweet();//清除
                StartCoroutine(AllFill());//开启协程填充移动
            }
            else
            {
                sweets[sweet1.X, sweet1.Y] = sweet1;
                sweets[sweet2.X, sweet2.Y] = sweet2;
            }
            
        }
    }
    /// <summary>
    /// 玩家对甜品进行操作的方法
    /// </summary>
    /// <param name="sweet"></param>
    #region
    public void PressSweet(SweetGame sweet)//鼠标按下甜品
    {
        if(gameOver)
        {
            return;
        }
        pressedSweet = sweet;
    }

    public void EnterSweet(SweetGame sweet)
    {
        if (gameOver)
        {
            return;
        }
        enteredSweet = sweet;
    }

    public void ReleaseSweet()
    {
        if (gameOver)
        {
            return;
        }
        if (IsFriend(pressedSweet,enteredSweet))
        {
            ExchangeSweets(pressedSweet, enteredSweet);
        }
    }
    #endregion

    /// <summary>
    /// 清除匹配方法
    /// </summary>
    /// <param name="sweet"></param>
    /// <param name="newX"></param>
    /// <param name="newY"></param>
    /// <returns></returns>
    #region
    //匹配方法
    public List<SweetGame> MatchSweets(SweetGame sweet,int newX,int newY)
    {
        if(sweet.CanColor())
        {
            ColorSweet.ColorType color = sweet.ColorComponent.Color;
            List<SweetGame> matchRowSweets = new List<SweetGame>();
            List<SweetGame> matchLineSweets = new List<SweetGame>();
            List<SweetGame> finishedMatchingSweets = new List<SweetGame>();

            //行匹配
            matchRowSweets.Add(sweet);
            //i=0往左，i=1往右
            for(int i=0;i<=1;i++)
            {
                for(int xDistance=1;xDistance<x; xDistance++)
                {
                    int xx;
                    if(i==0)
                    {
                        xx = newX - xDistance;
                    }
                    else
                    {
                        xx = newX + xDistance;
                    }
                    if(xx<0||xx>=x)
                    {
                        break;
                    }
                    if(sweets[xx,newY].CanColor()&&sweets[xx,newY].ColorComponent.Color==color)
                    {
                        matchRowSweets.Add(sweets[xx, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if(matchRowSweets.Count>=3)
            {
                for(int i=0;i<matchRowSweets.Count;i++)
                {
                    finishedMatchingSweets.Add(matchRowSweets[i]);
                }
            }
            //L T形匹配
            //检查当前行遍历列表元素是否大于等于3
            if(matchRowSweets.Count>=3)
            {
                for(int i=0;i<matchRowSweets.Count;i++)
                {
                    //finishedMatchingSweets.Add(matchRowSweets[i]);
                    //行匹配列表中满足匹配条件的每个元素上下依次进行列遍历
                    for(int j=0;j<=1;j++)
                    {
                        for(int yDistance=1;yDistance<y;yDistance++)
                        {
                            int y1;
                            if(j==0)
                            {
                                y1 = newY - yDistance;
                            }
                            else
                            {
                                y1 = newY + yDistance;
                            }
                            if(y1<0||y1>=y)
                            {
                                break;
                            }
                            if (sweets[matchRowSweets[i].X, y1].CanColor() && sweets[matchRowSweets[i].X,y1].ColorComponent.Color==color)
                            {
                                matchLineSweets.Add(sweets[matchRowSweets[i].X, y1]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if(matchLineSweets.Count<2)
                    {
                        matchLineSweets.Clear();
                    }
                    else
                    {
                        for(int j=0;j<matchLineSweets.Count;j++)
                        {
                            finishedMatchingSweets.Add(matchLineSweets[j]);
                        }
                    }
                }
            }
            if(finishedMatchingSweets.Count>=3)
            {
                return finishedMatchingSweets;
            }

            matchRowSweets.Clear();
            matchLineSweets.Clear();

            //列匹配
            matchLineSweets.Add(sweet);
            //i=0往左，i=1往右
            for (int i = 0; i <= 1; i++)
            {
                for (int yDistance = 1; yDistance < y; yDistance++)
                {
                    int yy;
                    if (i == 0)
                    {
                        yy = newY - yDistance;
                    }
                    else
                    {
                        yy = newY + yDistance;
                    }
                    if (yy < 0 || yy >= x)
                    {
                        break;
                    }
                    if (sweets[newX, yy].CanColor() && sweets[newX, yy].ColorComponent.Color == color)
                    {
                        matchLineSweets.Add(sweets[newX, yy]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (matchLineSweets.Count >= 3)
            {
                for (int i = 0; i < matchLineSweets.Count; i++)
                {
                    finishedMatchingSweets.Add(matchLineSweets[i]);
                }
            }
            //检查当前行遍历列表元素是否大于等于3
            if (matchLineSweets.Count >= 3)
            {
                for (int i = 0; i < matchLineSweets.Count; i++)
                {
                    //finishedMatchingSweets.Add(matchRowSweets[i]);
                    //行匹配列表中满足匹配条件的每个元素上下依次进行列遍历
                    for (int j = 0; j <= 1; j++)
                    {
                        for (int xDistance = 1; xDistance < x; xDistance++)
                        {
                            int x1;
                            if (j == 0)
                            {
                                x1 = newX - xDistance;
                            }
                            else
                            {
                                x1 = newX + xDistance;
                            }
                            if (x1 < 0 || x1 >= x)
                            {
                                break;
                            }
                            if (sweets[x1,matchLineSweets[i].Y].CanColor() && sweets[x1, matchLineSweets[i].Y].ColorComponent.Color == color)
                            {
                                matchRowSweets.Add(sweets[x1, matchLineSweets[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (matchRowSweets.Count < 2)
                    {
                        matchRowSweets.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < matchRowSweets.Count; j++)
                        {
                            finishedMatchingSweets.Add(matchRowSweets[j]);
                        }
                    }
                }
            }
            if (finishedMatchingSweets.Count >= 3)
            {
                return finishedMatchingSweets;
            }
        }
        return null;
    }

    //清除方法
    public bool ClearSweet(int x,int y)
    {
        if(sweets[x,y].CanClear()&&!sweets[x,y].ClearComponent.IsClearing)
        {
            sweets[x, y].ClearComponent.Clear();
            CreateNewSweet(x, y, SweetType.EMPTY);
            ClearBarrier(x, y);
            return true;
        }

        return false;
    }

    //清除障碍饼干算法
    private void ClearBarrier(int x1,int y1)//坐标为被消除的饼干对象的坐标
    {
        //行遍历（x1-1左边，x1+1右边）
        for(int friendX=x1-1;friendX<=x1+1;friendX++)
        {
            if(friendX!=x1&&friendX>=0&&friendX<x)
            {
                if(sweets[friendX,y1].Type==SweetType.BARRIER&&sweets[friendX,y1].CanClear())//当前位置上是障碍饼干
                {
                    sweets[friendX, y1].ClearComponent.Clear();
                    CreateNewSweet(friendX, y1, SweetType.EMPTY);//创建空白甜品
                }
            }
        }
        //列遍历
        for (int friendY = y1 - 1; friendY <= y1 + 1; friendY++)
        {
            if (friendY != y1 && friendY >= 0 && friendY < y)
            {
                if (sweets[x1,friendY].Type == SweetType.BARRIER && sweets[x1,friendY].CanClear())
                {
                    sweets[x1, friendY].ClearComponent.Clear();
                    CreateNewSweet(x1, friendY, SweetType.EMPTY);
                }
            }
        }
    }
    //清除完成匹配的全部甜品
    private bool ClearAllMatchedSweet()
    {
        bool needRefill = false;//判断是否需要填充
        for(int y1=0;y1<y;y1++)
        {
            for(int x1=0;x1<x;x1++)
            {
                if(sweets[x1,y1].CanClear())//可以清除
                {
                    List<SweetGame> matchList = MatchSweets(sweets[x1, y1], x1, y1);//拿完成匹配成功的列表
                    if(matchList!=null)
                    {
                        SweetType specialSweetsType = SweetType.COUNT;//决定是否产生特殊甜品

                        SweetGame randomSweet = matchList[Random.Range(0, matchList.Count)];//获取随机产生的甜品坐标位置
                        int specialSweetX = randomSweet.X;
                        int specialSweetY = randomSweet.Y;
                        if(matchList.Count==4)//产生行列消除的甜品
                        {
                            specialSweetsType = (SweetType)Random.Range((int)SweetType.ROW_CLEAR, (int)SweetType.COLUMN_CLEAR);//随机数是int，先强转int，在转枚举
                        }
                        //5个产生彩虹糖

                        else if(matchList.Count>=5)
                        {
                            specialSweetsType = SweetType.RAINBOWANDY;
                        }

                        for(int i=0;i<matchList.Count;i++)//遍历匹配中的要消除的甜品
                        {
                            if(ClearSweet(matchList[i].X,matchList[i].Y))
                            {
                                needRefill = true;
                            }
                        }
                        if(specialSweetsType!=SweetType.COUNT)//当前位置不是标记类型的甜品
                        {
                            Destroy(sweets[specialSweetX, specialSweetY]);//清除当前位置的空白对象
                            SweetGame newSweet = CreateNewSweet(specialSweetX, specialSweetY, specialSweetsType);//产生新甜品
                            if(specialSweetsType==SweetType.ROW_CLEAR||specialSweetsType==SweetType.COLUMN_CLEAR&&newSweet.CanColor()&&matchList[0].CanColor())//是否着色，匹配列表中第一个元素是否可以着色（当前匹配列表中颜色都是一样的）
                            {
                                newSweet.ColorComponent.SetColor(matchList[0].ColorComponent.Color);//吧匹配列表第一个元素的颜色赋值给当前的
                            }

                            //彩虹糖的产生
                            else if(specialSweetsType==SweetType.RAINBOWANDY&&newSweet.CanColor())
                            {
                                newSweet.ColorComponent.SetColor(ColorSweet.ColorType.ANY);
                            }
                        }
                    }
                }
            }
        }
        return needRefill;
    }
    #endregion

    public void ReturnMain()
    {
        SceneManager.LoadScene(0);
    }

    public void Replay()
    {
        SceneManager.LoadScene(1);
    }

    public void NextGame()
    {
        SceneManager.LoadScene(2);
    }

    //清除行的方法
    public void ClearRow(int row)
    {
        for(int i=0;i<x;i++)
        {
            ClearSweet(i, row);
        }
    }

    //清除列的方法
    public void ClearColumn(int column)
    {
        for(int j=0;j<y;j++)
        {
            ClearSweet(column, j);
        }
    }

    //清除颜色的方法（同类型）彩虹糖消除同类型颜色，在两个甜品交换的时候消除
    public void ClearColor(ColorSweet.ColorType color)//获取当前同类型的颜色
    {
        for(int i=0;i<x;i++)
        {
            for(int j=0;j<y;j++)
            {
                //1.当前甜品有颜色并且颜色是当前颜色是想要清除的颜色2.当前甜品有颜色并且当前的是彩虹糖，彩虹糖和彩虹糖可以进行消除
                if(sweets[i,j].CanColor()&&(sweets[i,j].ColorComponent.Color==color||color==ColorSweet.ColorType.ANY))
                {
                    ClearSweet(i, j);
                }
            }
        }
    }
}
