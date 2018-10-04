using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour {

    Animator animator;

    Queue<AnimationPlaylistItem> queue;

    AnimationPlaylistItem item;

    AnimatorStateInfo currentState;

    int currentHashName;

    float floorNormalizedTime = 0;

    bool isImmeditely = false;

    bool isRepeatPlay;

    List<AnimationPlaylistItem> playList;

    [Range(0,2f)]
    [SerializeField]
    float animationSpeed = 1;

    // Use this for initialization
    void Awake()
    {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {

        if (queue == null)
            return;

        animator.speed = animationSpeed;

        currentState = animator.GetCurrentAnimatorStateInfo(0);

        if(isImmeditely == false)
        {
            if (currentState.shortNameHash != currentHashName)
            {
                if (queue.Count > 0)
                    PlayNextAnimation();
            }
            else
            {
                if (item != null)
                {
                    if ((item.loopCount > 0 && currentState.normalizedTime >= item.loopCount) || (floorNormalizedTime > 0 && currentState.normalizedTime - floorNormalizedTime >= 1.0f))
                    {
                        if (floorNormalizedTime > 0)
                            floorNormalizedTime = 0;

                        currentHashName = -1;

                        if (item.finishCallback != null)
                            item.finishCallback();

                        if(isRepeatPlay == true)
                        {
                            InsertQueueItems(playList);
                        }
                    }
                }
            }
        }
    }

    void PlayNextAnimation()
    {
        item = queue.Dequeue();

        if (item == null)
            return;

        animator.Play(item.hashName, -1, 0);

        currentHashName = item.hashName;
    }

    public void PlayTransitionAnimation(string animationPrefixName, bool hasExitTime = false, int loopCount = 1, Action finishCallback = null)
    {
        if (hasExitTime == false)
        {
            currentHashName = -1;
            queue = new Queue<AnimationPlaylistItem>(3);
        }
        else
        {
            floorNormalizedTime = Mathf.Floor(currentState.normalizedTime);
        }

        queue.Enqueue(new AnimationPlaylistItem(string.Format("{0}_start", animationPrefixName)));
        queue.Enqueue(new AnimationPlaylistItem(string.Format("{0}_loop", animationPrefixName), loopCount));
        queue.Enqueue(new AnimationPlaylistItem(string.Format("{0}_end", animationPrefixName), finishCallback));
    }

    public void InterceptionPlay(string animationName, float transitionDuration = 1f, float loopCycle = 0.5f)
    {
        isImmeditely = true;

        var currAnim = animator.GetCurrentAnimatorStateInfo(0);

        var animElasped = currAnim.normalizedTime % 1f;

        if (animElasped >= loopCycle)
            animElasped -= loopCycle;

        var transitionDurationFixedTime = transitionDuration / currAnim.length;

        animator.CrossFade(Animator.StringToHash(animationName), transitionDurationFixedTime, 0, animElasped);

        StartCoroutine(ReleaseImmediatelyAnimation(currAnim.length * (1 - animElasped)));
    }


    public void Play(List<AnimationPlaylistItem> playList, bool hasExitTime = false, bool isRepeat = false)
    {
        this.isRepeatPlay = isRepeat;
        this.playList = playList;

        if (hasExitTime == false)
        {
            currentHashName = -1;
            queue = new Queue<AnimationPlaylistItem>(playList.Count);
        }
        else
        {
            floorNormalizedTime = Mathf.Floor(currentState.normalizedTime);
        }

        InsertQueueItems(playList);
    }

    void InsertQueueItems(List<AnimationPlaylistItem> playList)
    {
        for (int i = 0; i < playList.Count; i++)
        {
            queue.Enqueue(playList[i]);
        }
    }

    public void Play(string animationName, bool hasExitTime = false, int loopCount = 0, Action finishCallback = null)
    {
        if(hasExitTime == false)
        {
            currentHashName = -1;
            queue = new Queue<AnimationPlaylistItem>(1);
        }
        else
        {
            floorNormalizedTime = Mathf.Floor(currentState.normalizedTime);
        }

        queue.Enqueue(new AnimationPlaylistItem(animationName, loopCount, finishCallback));
    }

    public void Play(string[] playlist, Action finishCallback = null)
    {
        queue = new Queue<AnimationPlaylistItem>();

        for (int i = 0; i < playlist.Length; i++)
        {
            queue.Enqueue(new AnimationPlaylistItem(playlist[i], 1, (i == playlist.Length - 1) ? finishCallback : null));
        }
    }

    IEnumerator ReleaseImmediatelyAnimation(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        animator.Play(currentHashName);
        isImmeditely = false;
    }
}
