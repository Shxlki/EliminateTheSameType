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

    public virtual void Clear()//�鷽��
    {
        isClearing = true;
        StartCoroutine(ClearCoroutine());
    }

    private IEnumerator ClearCoroutine()//����Э��
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play(clearAnimation.name);
            //��ҵ÷�,������Ч
            GameManger.Instance.playerScore++;
            AudioSource.PlayClipAtPoint(destoryAudio, transform.position);//�ڵ�ǰλ�ý������ֲ���
            yield return new WaitForSeconds(clearAnimation.length);//�ͷ�
            Destroy(gameObject);
        }
    }
}
