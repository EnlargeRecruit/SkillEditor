using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SkillWindow : EditorWindow
{
    Player player;
    List<SkillBase> skillComponents;
    float currSpeed = 1;
    internal void SetInitSkill(List<SkillBase> _skillComponents, Player _player)
    {
        player = _player;
        currSpeed = 1;
        skillComponents = _skillComponents;
        player.currSkillComponets = skillComponents;
    }
    string[] skillComponent = new string[] { "null", "动画", "声音", "特效" };
    int skillComponentIndex = 0;

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("播放"))
        {
            foreach (var item in skillComponents)
            {
                item.Play();
            }
        }
        if (GUILayout.Button("暂停"))
        {
            foreach (var item in skillComponents)
            {
                item.Stop();
            }
        }
        if (GUILayout.Button("保存"))
        {
            player.Baocun();
        }
        GUILayout.EndHorizontal();
        GUILayout.Label("速度");
        float speed = EditorGUILayout.Slider(currSpeed, 0, 5);
        if (speed != currSpeed)
        {
            currSpeed = speed;
            Time.timeScale = currSpeed;
        }
        GUILayout.BeginHorizontal();
        skillComponentIndex = EditorGUILayout.Popup(skillComponentIndex, skillComponent);
        if (GUILayout.Button("添加"))
        {
            switch (skillComponentIndex)
            {
                case 1:
                    {
                        skillComponents.Add(new Skill_Anim(player));
                    }
                    break;
                case 2:
                    {
                        skillComponents.Add(new Skill_Audio(player));
                    }
                    break;
                case 3:
                    {
                        skillComponents.Add(new Skill_Effects(player));
                    }
                    break;
            }
        }
        GUILayout.EndHorizontal();
        ScrollView = GUILayout.BeginScrollView(ScrollView, false, true);
        foreach (var item in skillComponents)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(item.name);
            if (GUILayout.Button("删除"))
            {
                skillComponents.Remove(item);
                player.Baocun();
                break;
            }
            GUILayout.EndHorizontal();
            if (item is Skill_Anim)
            {
                ShowSkill_Anim(item as Skill_Anim);
            }
            else if (item is Skill_Audio)
            {
                Skill_Audio(item as Skill_Audio);
            }
            else if (item is Skill_Effects)
            {
                Skill_Effects(item as Skill_Effects);
            }
            GUILayout.Space(0.5f);
        }
        GUILayout.EndScrollView();
    }
    Vector2 ScrollView = new Vector2(0, 0);
    private void Skill_Effects(Skill_Effects _Effects)
    {
        _Effects.trigger = EditorGUILayout.TextField(_Effects.trigger);
        GameObject gameClip = EditorGUILayout.ObjectField(_Effects.gameClip, typeof(GameObject), false) as GameObject;
        if (_Effects.gameClip != gameClip)
        {
            _Effects.SetGameClip(gameClip);
        }
    }

    private void Skill_Audio(Skill_Audio _Audio)
    {
        _Audio.trigger = EditorGUILayout.TextField(_Audio.trigger);
        AudioClip audioClip = EditorGUILayout.ObjectField(_Audio.audioClip, typeof(AudioClip), false) as AudioClip;
        if (_Audio.audioClip != audioClip)
        {
            _Audio.SetAudioClip(audioClip);
        }
    }

    private void ShowSkill_Anim(Skill_Anim _Anim)
    {
        _Anim.trigger = EditorGUILayout.TextField(_Anim.trigger);
        AnimationClip animationClip = EditorGUILayout.ObjectField(_Anim.animationClip, typeof(AnimationClip), false) as AnimationClip;
        if (_Anim.animationClip != animationClip)
        {
            _Anim.SetAnimClip(animationClip);
        }
    }
}
