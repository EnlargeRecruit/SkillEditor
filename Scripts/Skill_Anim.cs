using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Anim : SkillBase
{
    Player player;
    Animator animator;
    public AnimationClip animationClip;
    AnimatorOverrideController controller;
    public Skill_Anim(Player _player)
    {
        player = _player;
        animator = player.gameObject.GetComponent<Animator>();
        controller = player.overrideController;
    }
    public override void Play()
    {
        base.Play();
        starttime = Time.time;
        isBegin = true;
    }
    public override void Init()
    {
        controller["Start"] = animationClip;
    }
    public override void Stop()
    {
        base.Stop();
        animator.StartPlayback();
    }
    public override void Update(float times)
    {
        if (isBegin && (times - starttime) > float.Parse(trigger))
        {
            isBegin = false;
            Begin();
        }
    }

    private void Begin()
    {
        animator.StopPlayback();
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Idle1"))
        {
            animator.SetTrigger("Play");
        }
    }

    public void SetAnimClip(AnimationClip _animClip)
    {
        animationClip = _animClip;
        name = animationClip.name;
        controller["Start"] = animationClip;
    }
}
