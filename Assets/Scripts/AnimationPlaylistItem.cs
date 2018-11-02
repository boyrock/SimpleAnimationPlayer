using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[System.Serializable]
public class AnimationPlaylistItem
{
    public string name;
    public int loopCount;
    public Action finishCallback;
    public bool isEnable;

    public int hashName
    {
        get
        {
            return Animator.StringToHash(name);
        }
    }

    public AnimationPlaylistItem(string name, Action finishCallback = null)
    {
        this.name = name;
        this.loopCount = 1;
        this.finishCallback = finishCallback;
        this.isEnable = true;
    }

    public AnimationPlaylistItem(string name, int loopCount, Action callBack = null)
    {
        this.name = name;
        this.loopCount = loopCount;
        this.finishCallback = callBack;
        this.isEnable = true;
    }

    public void CopyTo(AnimationPlaylistItem state)
    {
        state.name = this.name;
        state.loopCount = this.loopCount;
        state.finishCallback = this.finishCallback;
        state.isEnable = this.isEnable;
    }
}
