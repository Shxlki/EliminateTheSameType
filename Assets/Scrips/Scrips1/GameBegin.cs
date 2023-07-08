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
    public int x;//��
    public int y;//��
    /*��ϷUI��ʾ*/
    //���ʱ��
    public float fillTime;
    //ʱ����ʾ
    public Text timeText;

    private float gameTime = 60;

    private bool gameOver;//��Ϸ����

    public int playerScore;//��ҵĵ÷�

    public Text PlayerScore;//��ҷ�����ʾ

    private float addScoreTime;//����ÿһ֡ʱ��

    private float currentScore;//��ǰ�÷�

    public GameObject gameOverPanel;//���

    public Text finalScoreText;//���÷�

    public Text finalScoreText1;

    public GameObject gameNextPanel;//��һ�����





    //��ȡ����Ԥ����
    public GameObject gridPrefab;

    //��Ʒ����
    private SweetGame[,] sweets;

    //Ҫ������������Ʒ����
    private SweetGame pressedSweet;
    private SweetGame enteredSweet;

    //����
    public enum SweetType
    {
        EMPTY,//��
        NORMAL,//��ͨ
        BARRIER,//�ϰ�
        ROW_CLEAR,//������
        COLUMN_CLEAR,//������
        RAINBOWANDY,//�ʺ���
        COUNT//�������
    }
    //��ƷԤ������ֵ䣬ͨ����Ʒ���������õ���Ӧ��Ʒ��Ϸ����
    public Dictionary<SweetType, GameObject> sweetPrefabDict;

    [System.Serializable]
    public struct SweetPrefab//�ṹ��
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
        //�ֵ�ʵ����
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
            //��ʾʧ����壬����ʧ����嶯��
            gameOverPanel.SetActive(true);
            finalScoreText.text = playerScore.ToString();
            gameOver = true;
        }
        if(gameTime<=0&&playerScore>=100)
        {
            gameTime = 0;
            //��ʾ��һ�����
            gameNextPanel.SetActive(true);
            finalScoreText1.text = playerScore.ToString();
            gameOver = true;

        }
        timeText.text = gameTime.ToString("0");
        if(addScoreTime<=0.05f)
        {
            addScoreTime += Time.deltaTime;//��ʱ���ۼ�
        }
        else
        {
            if(currentScore<playerScore)//��ǰ�÷��ۼ�
            {
                currentScore++;
                PlayerScore.text = currentScore.ToString();
                addScoreTime = 0;
            }
        }
        



    }
    public Vector2 BlockPosition(int i,int j)//����λ��
    {
        return new Vector2(transform.position.x - x / 2 + i, transform.position.y + y / 2 - j);
    }

    //������Ʒ�ķ���
    public SweetGame CreateNewSweet(int x,int y,SweetType type)
    {
        GameObject newSweet = Instantiate(sweetPrefabDict[type], BlockPosition(x, y), Quaternion.identity);
        newSweet.transform.parent = transform;
        sweets[x, y] = newSweet.GetComponent<SweetGame>();
        sweets[x, y].Init(x, y, this, type);
        return sweets[x, y];

    }

    //ȫ�����
    public IEnumerator AllFill()
    {
        bool needRefill = true;//�������Ƿ���Ҫ���
        while(needRefill)
        {
            yield return new WaitForSeconds(fillTime);
            //���δ���һֱ���
            while(Fill())
            {
                yield return new WaitForSeconds(fillTime);
            }
            //���ƥ��õ�������Ʒ
            needRefill = ClearAllMatchedSweet();
        }
        
    }

    //�ֲ����
    public bool Fill()
    {
        bool isFill = false;//�жϱ�������Ƿ����
        for(int i=y-2;i>=0;i--)
        {
            for(int j=0;j<x; j++)
            {
                SweetGame sweet = sweets[j, i];//��ǰԪ��λ�õ���Ʒ����
                if(sweet.CanMove())//����޷��ƶ������޷��������
                {
                    SweetGame sweetBelow = sweets[j, i + 1];
                    if(sweetBelow.Type==SweetType.EMPTY)//��ֱ���
                    {
                        Destroy(sweetBelow.gameObject);
                        sweet.MoveComponent.Move(j, i + 1,fillTime);
                        sweets[j,i + 1] = sweet;
                        CreateNewSweet(j, i, SweetType.EMPTY);
                        isFill = true;
                    }
                    else//б�����
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
                                        bool isfill = true;//�Ƿ����б�����
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

        //������-1��
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

    private bool IsFriend(SweetGame sweet1,SweetGame sweet2)//�ж���Ʒ�Ƿ�����
    {
        return (sweet1.X == sweet2.X && Mathf.Abs(sweet1.Y - sweet2.Y) == 1) || (sweet1.Y == sweet2.Y && Mathf.Abs(sweet1.X - sweet2.X) == 1);
    }

    //����������Ʒ�ķ���
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
                if(sweet1.Type==SweetType.RAINBOWANDY&&sweet1.CanClear()&&sweet2.CanClear())//�ʺ�������ͬ����Ԫ��
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


                ClearAllMatchedSweet();//���
                StartCoroutine(AllFill());//����Э������ƶ�
            }
            else
            {
                sweets[sweet1.X, sweet1.Y] = sweet1;
                sweets[sweet2.X, sweet2.Y] = sweet2;
            }
            
        }
    }
    /// <summary>
    /// ��Ҷ���Ʒ���в����ķ���
    /// </summary>
    /// <param name="sweet"></param>
    #region
    public void PressSweet(SweetGame sweet)//��갴����Ʒ
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
    /// ���ƥ�䷽��
    /// </summary>
    /// <param name="sweet"></param>
    /// <param name="newX"></param>
    /// <param name="newY"></param>
    /// <returns></returns>
    #region
    //ƥ�䷽��
    public List<SweetGame> MatchSweets(SweetGame sweet,int newX,int newY)
    {
        if(sweet.CanColor())
        {
            ColorSweet.ColorType color = sweet.ColorComponent.Color;
            List<SweetGame> matchRowSweets = new List<SweetGame>();
            List<SweetGame> matchLineSweets = new List<SweetGame>();
            List<SweetGame> finishedMatchingSweets = new List<SweetGame>();

            //��ƥ��
            matchRowSweets.Add(sweet);
            //i=0����i=1����
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
            //L T��ƥ��
            //��鵱ǰ�б����б�Ԫ���Ƿ���ڵ���3
            if(matchRowSweets.Count>=3)
            {
                for(int i=0;i<matchRowSweets.Count;i++)
                {
                    //finishedMatchingSweets.Add(matchRowSweets[i]);
                    //��ƥ���б�������ƥ��������ÿ��Ԫ���������ν����б���
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

            //��ƥ��
            matchLineSweets.Add(sweet);
            //i=0����i=1����
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
            //��鵱ǰ�б����б�Ԫ���Ƿ���ڵ���3
            if (matchLineSweets.Count >= 3)
            {
                for (int i = 0; i < matchLineSweets.Count; i++)
                {
                    //finishedMatchingSweets.Add(matchRowSweets[i]);
                    //��ƥ���б�������ƥ��������ÿ��Ԫ���������ν����б���
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

    //�������
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

    //����ϰ������㷨
    private void ClearBarrier(int x1,int y1)//����Ϊ�������ı��ɶ��������
    {
        //�б�����x1-1��ߣ�x1+1�ұߣ�
        for(int friendX=x1-1;friendX<=x1+1;friendX++)
        {
            if(friendX!=x1&&friendX>=0&&friendX<x)
            {
                if(sweets[friendX,y1].Type==SweetType.BARRIER&&sweets[friendX,y1].CanClear())//��ǰλ�������ϰ�����
                {
                    sweets[friendX, y1].ClearComponent.Clear();
                    CreateNewSweet(friendX, y1, SweetType.EMPTY);//�����հ���Ʒ
                }
            }
        }
        //�б���
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
    //������ƥ���ȫ����Ʒ
    private bool ClearAllMatchedSweet()
    {
        bool needRefill = false;//�ж��Ƿ���Ҫ���
        for(int y1=0;y1<y;y1++)
        {
            for(int x1=0;x1<x;x1++)
            {
                if(sweets[x1,y1].CanClear())//�������
                {
                    List<SweetGame> matchList = MatchSweets(sweets[x1, y1], x1, y1);//�����ƥ��ɹ����б�
                    if(matchList!=null)
                    {
                        SweetType specialSweetsType = SweetType.COUNT;//�����Ƿ����������Ʒ

                        SweetGame randomSweet = matchList[Random.Range(0, matchList.Count)];//��ȡ�����������Ʒ����λ��
                        int specialSweetX = randomSweet.X;
                        int specialSweetY = randomSweet.Y;
                        if(matchList.Count==4)//����������������Ʒ
                        {
                            specialSweetsType = (SweetType)Random.Range((int)SweetType.ROW_CLEAR, (int)SweetType.COLUMN_CLEAR);//�������int����ǿתint����תö��
                        }
                        //5�������ʺ���

                        else if(matchList.Count>=5)
                        {
                            specialSweetsType = SweetType.RAINBOWANDY;
                        }

                        for(int i=0;i<matchList.Count;i++)//����ƥ���е�Ҫ��������Ʒ
                        {
                            if(ClearSweet(matchList[i].X,matchList[i].Y))
                            {
                                needRefill = true;
                            }
                        }
                        if(specialSweetsType!=SweetType.COUNT)//��ǰλ�ò��Ǳ�����͵���Ʒ
                        {
                            Destroy(sweets[specialSweetX, specialSweetY]);//�����ǰλ�õĿհ׶���
                            SweetGame newSweet = CreateNewSweet(specialSweetX, specialSweetY, specialSweetsType);//��������Ʒ
                            if(specialSweetsType==SweetType.ROW_CLEAR||specialSweetsType==SweetType.COLUMN_CLEAR&&newSweet.CanColor()&&matchList[0].CanColor())//�Ƿ���ɫ��ƥ���б��е�һ��Ԫ���Ƿ������ɫ����ǰƥ���б�����ɫ����һ���ģ�
                            {
                                newSweet.ColorComponent.SetColor(matchList[0].ColorComponent.Color);//��ƥ���б��һ��Ԫ�ص���ɫ��ֵ����ǰ��
                            }

                            //�ʺ��ǵĲ���
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

    //����еķ���
    public void ClearRow(int row)
    {
        for(int i=0;i<x;i++)
        {
            ClearSweet(i, row);
        }
    }

    //����еķ���
    public void ClearColumn(int column)
    {
        for(int j=0;j<y;j++)
        {
            ClearSweet(column, j);
        }
    }

    //�����ɫ�ķ�����ͬ���ͣ��ʺ�������ͬ������ɫ����������Ʒ������ʱ������
    public void ClearColor(ColorSweet.ColorType color)//��ȡ��ǰͬ���͵���ɫ
    {
        for(int i=0;i<x;i++)
        {
            for(int j=0;j<y;j++)
            {
                //1.��ǰ��Ʒ����ɫ������ɫ�ǵ�ǰ��ɫ����Ҫ�������ɫ2.��ǰ��Ʒ����ɫ���ҵ�ǰ���ǲʺ��ǣ��ʺ��ǺͲʺ��ǿ��Խ�������
                if(sweets[i,j].CanColor()&&(sweets[i,j].ColorComponent.Color==color||color==ColorSweet.ColorType.ANY))
                {
                    ClearSweet(i, j);
                }
            }
        }
    }
}
