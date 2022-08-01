using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Audio : SkillBase
{
    Player player;
    AudioSource audioSource;
    public AudioClip audioClip;
    public Skill_Audio(Player _player)
    {
        player = _player;
        audioSource = player.gameObject.GetComponent<AudioSource>();
    }
    public override void Play()
    {
        base.Play();
        starttime = Time.time;
        isBegin = true;
    }
    public override void Init()
    {
        audioSource.clip = audioClip;
    }
    public override void Stop()
    {
        base.Stop();
        audioSource.Stop();
    }
    public override void Update(float times)
    {
        if ((times - starttime) > float.Parse(trigger) && isBegin)
        {
            isBegin = false;
            Begin();
        }
    }

    private void Begin()
    {
        audioSource.Play();
    }

    public void SetAudioClip(AudioClip _audioClip)
    {
        audioClip = _audioClip;
        name = audioClip.name;
        audioSource.clip = audioClip;
        audioSource.Stop();
    }
}
