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
    }

    public AnimationPlaylistItem(string name, int loopCount, Action callBack = null)
    {
        this.name = name;
        this.loopCount = loopCount;
        this.finishCallback = callBack;
    }
}
