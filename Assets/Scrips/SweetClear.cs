using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SweetClear : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public AnimationClip clearAnimation;

    private bool isClearing;

    public AudioClip destoryAudio;

    public bool IsClearing
    {
        get
        {
            return isClearing;
        }

    }

    protected GameSweet sweet;

    private void Awake()
    {
        sweet = GetComponent<GameSweet>();
    }

    public virtual void Clear()//虚方法
    {
        isClearing = true;
        StartCoroutine(ClearCoroutine());
    }

    private IEnumerator ClearCoroutine()//消除协程
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play(clearAnimation.name);
            //玩家得分,播放音效
            GameManger.Instance.playerScore++;
            AudioSource.PlayClipAtPoint(destoryAudio, transform.position);//在当前位置进行音乐播放
            yield return new WaitForSeconds(clearAnimation.length);//释放
            Destroy(gameObject);
        }
    }
}
