using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    AnimationController animationController;
    string aa = "0004_NouminA_AO_A_INT1_04";

    // Use this for initialization
    void Start () {
        animationController = this.GetComponent<AnimationController>();

        //string[] playlist = { "0001_NouminA_AO_A_WLK-start_04", "0002_NouminA_AO_A_WLK", "0003_NouminA_AO_A_WLK-end_04" };
        //PlayAnimation(playlist);

        PlayAnimation();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.I))
            Interaction();
    }

    void PlayAnimation(string[] playlist)
    {

        Action callback = delegate
        {
            GameObject.Destroy(this.gameObject);
        };

        animationController.Play(playlist, callback);
    }

    void PlayAnimation()
    {
        Action callback = delegate
        {
            animationController.Play("0003_NouminA_AO_A_WLK-end_04");
        };
        animationController.Play("0002_NouminA_AO_A_WLK", false, 10, callback);
    }

    void Interaction()
    {
        animationController.InterceptionPlay("0004_NouminA_AO_A_INT1_04", 1, 0.5f);
    }
}
