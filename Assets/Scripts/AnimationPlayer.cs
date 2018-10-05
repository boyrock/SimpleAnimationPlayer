using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(AnimationController))]
public class AnimationPlayer : MonoBehaviour {

    public List<AnimationPlaylistItem> playlist = new List<AnimationPlaylistItem>();

    public bool playOnStart = true;

    public bool isRepeatPlay = true;

    // Use this for initialization
    void Start ()
    {
        var controller = GetComponent<AnimationController>();

        if(playOnStart == true)
        {
            controller.Play(playlist.Where(p => p.isEnable == true).ToList(), false, isRepeatPlay);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetPlaylist(List<AnimationPlaylistItem> playlist)
    {
        this.playlist = playlist;
    }
}
