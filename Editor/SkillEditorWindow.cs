using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SkillEditorWindow : EditorWindow
{
    class PlayerEditor
    {
        public int _characterIndex = 0;//预制体下标
        public int _folderIndex = 0;//文件下标
        public string characterName = string.Empty;//预制体名字
        public string folderName = string.Empty;//文件名
        public List<string> characteList = new List<string>();//预制体集合
        public Player player = null;
    }
    PlayerEditor m_player = new PlayerEditor();
    //文件夹名字
    public List<string> m_folderList = new List<string>();
    //所有预制体名字
    public List<string> m__characterList = new List<string>();
    //文件夹名为键，预制体名字集合为值
    public Dictionary<string, List<string>> m_folderPrefabs = new Dictionary<string, List<string>>();
    //创建的技能名字
    string newSkillName = string.Empty;
    // 技能详情窗口
    SkillWindow skillWindow;
    [MenuItem("Tools/技能编译器")]
    public static void Init()
    {
        SkillEditorWindow skillEditor = EditorWindow.GetWindow<SkillEditorWindow>("SkillEditor");
        if (skillEditor != null)
        {
            skillEditor.Show();
        }
    }
    private void OnEnable()
    {
        DoSearchFolder();
        DoSearchCharacter();
    }
    //查找所有文件
    void DoSearchFolder()
    {
        string[] folders = Directory.GetDirectories(GetCharacterPath());
        m_folderList.Clear();
        m_folderList.Add("All");
        for (int i = 0; i < folders.Length; i++)
        {
            m_folderList.Add(Path.GetFileName(folders[i]));
        }
    }
    void DoSearchCharacter()
    {
        string[] files = Directory.GetFiles(GetCharacterPath(), "*.prefab", SearchOption.AllDirectories);
        m__characterList.Clear();
        for (int i = 0; i < files.Length; i++)
        {
            m__characterList.Add(Path.GetFileNameWithoutExtension(files[i]));
        }
        m__characterList.Sort();
        m__characterList.Insert(0, "Null");
        m_player.characteList = m__characterList;
    }
    //文件路径
    string GetCharacterPath()
    {
        return Application.dataPath + "/GameDate/Model";
    }
    private void OnGUI()
    {
        int folderIndex = EditorGUILayout.Popup(m_player._folderIndex, m_folderList.ToArray());
        if (folderIndex != m_player._folderIndex)
        {
            m_player._folderIndex = folderIndex;
            m_player._characterIndex = -1;
            List<string> list;//存放选择文件夹下的所有预制体
            string folderName = m_folderList[m_player._folderIndex];
            if (folderName.Equals("All"))
            {
                list = m__characterList;
            }
            else
            {
                if (!m_folderPrefabs.TryGetValue(folderName, out list))
                {
                    list = new List<string>();
                    string[] files = Directory.GetFiles(GetCharacterPath() + "/" + folderName, "*.prefab", SearchOption.AllDirectories);
                    foreach (var item in files)
                    {
                        list.Add(Path.GetFileNameWithoutExtension(item));
                    }
                    m_folderPrefabs[folderName] = list;
                }
            }
            m_player.characteList.Clear();
            m_player.characteList.AddRange(list);
        }

        int characteIndex = EditorGUILayout.Popup(m_player._characterIndex, m_player.characteList.ToArray());
        if (characteIndex != m_player._characterIndex)
        {
            m_player._characterIndex = characteIndex;
            string charcterName = m__characterList[m_player._characterIndex];
            if (m_player.characterName != charcterName)
            {
                m_player.characterName = charcterName;
                if (!string.IsNullOrEmpty(m_player.characterName))
                {
                    if (m_player.player != null)
                    {
                        m_player.player.Destory();
                    }
                    m_player.player = Player.Init(charcterName);
                }
            }
        }
        newSkillName = GUILayout.TextField(newSkillName);
        if (GUILayout.Button("创建新的技能"))
        {
            if (!string.IsNullOrEmpty(newSkillName) && m_player.player != null)
            {
                List<SkillBase> skills = m_player.player.AddNewSkill(newSkillName);
                //打开另一个窗口
                OpenSkillWindow(newSkillName, skills);
                newSkillName = "";
            }
        }
        if (m_player.player != null)
        {
            ScrollViewPos = GUILayout.BeginScrollView(ScrollViewPos, false, true);
            foreach (var item in m_player.player.skillList)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(item.Key))
                {
                    List<SkillBase> skillComponents = m_player.player.GetSkill(item.Key);
                    foreach (var ite in skillComponents)
                    {
                        ite.Init();
                    }
                    OpenSkillWindow(item.Key, skillComponents);
                }
                GUILayoutOption[] option = new GUILayoutOption[]
                {
                GUILayout.Width(60),
                GUILayout.Height(19)
                };
                if (GUILayout.Button("删除技能", option))
                {
                    m_player.player.RevSkill(item.Key);
                    break;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
    }
    Vector2 ScrollViewPos = new Vector2(0, 0);
    private void OpenSkillWindow(string newSkillName, List<SkillBase> skillComponents)
    {
        if (skillComponents != null)
        {
            if (skillWindow == null)
            {
                skillWindow = EditorWindow.GetWindow<SkillWindow>("");
            }
            //设置名字
            skillWindow.titleContent = new GUIContent(newSkillName);
            skillWindow.SetInitSkill(skillComponents, m_player.player);
            skillWindow.Show();
            skillWindow.Repaint();
        }
    }
}
