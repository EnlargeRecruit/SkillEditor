using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBase
{
    public string name = string.Empty;
    //≤•∑≈—” ±
    public string trigger = "0";
    public float starttime = 0f;
    public bool isBegin = false;
    public virtual void Play()
    {

    }
    public virtual void Init()
    {

    }
    public virtual void Stop()
    {

    }
    public virtual void SetTrigger(string tri)
    {
        trigger = tri;
    }
    public virtual void Update(float times)
    {

    }
}
