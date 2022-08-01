using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Dictionary<string, List<SkillBase>> skillList = new Dictionary<string, List<SkillBase>>();
    public RuntimeAnimatorController controller;
    public AnimatorOverrideController overrideController;
    AudioSource audioSource;
    Animator anim;
    //组件集合
    public List<SkillBase> currSkillComponets = new List<SkillBase>();
    public Transform effectsparent;
    public static Player Init(string path)
    {
        if (path != null)
        {
            string str = "Assets/aaa/" + path + ".prefab";
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(str);
            if (obj != null)
            {
                Player player = Instantiate(obj).AddComponent<Player>();
                player.overrideController = new AnimatorOverrideController();
                player.controller = Resources.Load<RuntimeAnimatorController>("Player");
                player.overrideController.runtimeAnimatorController = player.controller;
                player.anim.runtimeAnimatorController = player.overrideController;
                player.audioSource = player.gameObject.AddComponent<AudioSource>();
                player.gameObject.name = path;
                player.effectsparent = player.transform.Find("effectsparent");
                player.LoadAssetData();
                return player;
            }
        }
        return null;
    }
    public void InitPlayer()
    {
        overrideController = new AnimatorOverrideController();
        controller = Resources.Load<RuntimeAnimatorController>("Player");
        overrideController.runtimeAnimatorController = controller;
        anim.runtimeAnimatorController = overrideController;
        audioSource = gameObject.AddComponent<AudioSource>();
        effectsparent = transform.Find("effectsparent");
        LoadAssetData();
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        foreach (var item in currSkillComponets)
        {
            item.Update(Time.time);
        }
        if (RockerManger.isRun&&RockerManger.isAnmi)
        {
            anim.SetBool("Run", true);
            transform.LookAt(new Vector3(RockerManger.pos.x, 0, RockerManger.pos.y) + transform.position);
            transform.Translate(new Vector3(0, 0, Time.deltaTime * 3f));
        }
        else
        {
            anim.SetBool("Run", false);
        }
    }
    public void LoadAssetData()
    {
        if (File.Exists("Assets/" + gameObject.name + ".json"))
        {
            string str = File.ReadAllText("Assets/" + gameObject.name + ".json");
            List<SkillJson> list = JsonConvert.DeserializeObject<List<SkillJson>>(str);
            foreach (var item in list)
            {
                skillList.Add(item.name, new List<SkillBase>());
                List<SkillBase> lists = new List<SkillBase>();
                foreach (var ite in item.Components)
                {
                    foreach (var it in ite.Value)
                    {
                        if (ite.Key.Equals("技能"))
                        {
                            AnimationClip animationClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/GameDate/Anim/" + it.ComponentName + ".anim");
                            Skill_Anim _Anim = new Skill_Anim(this);
                            _Anim.SetAnimClip(animationClip);
                            _Anim.SetTrigger(it.trigger);
                            lists.Add(_Anim);
                        }
                        else if (ite.Key.Equals("声音"))
                        {
                            AudioClip audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/GameDate/Audio/" + it.ComponentName + ".mp3");
                            Skill_Audio _Audio = new Skill_Audio(this);
                            _Audio.SetAudioClip(audioClip);
                            _Audio.SetTrigger(it.trigger);
                            lists.Add(_Audio);
                        }
                        else if (ite.Key.Equals("特效"))
                        {
                            GameObject gameClip = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GameDate/Effect/Skill/" + it.ComponentName + ".prefab");
                            Skill_Effects _Effect = new Skill_Effects(this);
                            _Effect.SetGameClip(gameClip);
                            _Effect.SetTrigger(it.trigger);
                            lists.Add(_Effect);
                        }
                    }
                }
                skillList[item.name] = lists;
            }
        }
    }

    public void Baocun()
    {
        List<SkillJson> list = new List<SkillJson>();
        foreach (var item in skillList)
        {
            SkillJson skillJson = new SkillJson();
            skillJson.name = item.Key;
            foreach (var ite in item.Value)
            {
                if (ite is Skill_Anim)
                {
                    if (!skillJson.Components.ContainsKey("技能"))
                    {
                        skillJson.Components.Add("技能", new List<ComponentSkill>());
                    }
                    skillJson.Components["技能"].Add(new ComponentSkill(ite.trigger, ite.name));
                }
                else if (ite is Skill_Audio)
                {
                    if (!skillJson.Components.ContainsKey("声音"))
                    {
                        skillJson.Components.Add("声音", new List<ComponentSkill>());
                    }
                    skillJson.Components["声音"].Add(new ComponentSkill(ite.trigger, ite.name));
                }
                else if (ite is Skill_Effects)
                {
                    if (!skillJson.Components.ContainsKey("特效"))
                    {
                        skillJson.Components.Add("特效", new List<ComponentSkill>());
                    }
                    skillJson.Components["特效"].Add(new ComponentSkill(ite.trigger, ite.name));
                }
            }
            list.Add(skillJson);
        }
        string str = JsonConvert.SerializeObject(list);
        File.WriteAllText("Assets/" + gameObject.name + ".json", str);
    }
    public void Destory()
    {
        Destroy(gameObject);
    }

    public List<SkillBase> AddNewSkill(string newSkillName)
    {
        if (skillList.ContainsKey(newSkillName))
        {
            return skillList[newSkillName];
        }
        skillList.Add(newSkillName, new List<SkillBase>());
        return skillList[newSkillName];
    }
    public void RevSkill(string newSkillName)
    {
        if (skillList.ContainsKey(newSkillName))
        {
            skillList.Remove(newSkillName);
        }
    }

    public List<SkillBase> GetSkill(string key)
    {
        if (skillList.ContainsKey(key))
        {
            return skillList[key];
        }
        skillList.Add(key, new List<SkillBase>());
        return skillList[key];
    }

}
public class SkillJson
{
    public string name;
    public Dictionary<string, List<ComponentSkill>> Components = new Dictionary<string, List<ComponentSkill>>();
}
public class ComponentSkill
{
    public string ComponentName;
    public string trigger;
    public ComponentSkill(string tri, string name)
    {
        ComponentName = name;
        trigger = tri;
    }
}
