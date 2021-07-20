﻿using System.Collections;
using System.Collections.Generic;
using TapTap.Moment;
using UnityEngine;

public class MomentScene : MonoBehaviour
{
    // Start is called before the first frame update

    private string label = "";

    void Start()
    {
        TapMoment.SetCallback((code, msg) =>
        {
            label = "---- moment 回调  code: " + code + " msg: " + msg + "----";
            Debug.Log("---- moment 回调  code: " + code + " msg: " + msg + "----");
            if (code == 20100)
            {
            }
            else if (code == 20000)
            {
            }
        });
    }

    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(6);
        TapMoment.Close();
    }

    private string sceneId = "taprl0194610001";

    private string userId = "7KfeZgtnLAZvJG8JZUnYVw==";


    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.fontSize = 40;

        GUIStyle inputStyle = new GUIStyle(GUI.skin.textArea);
        inputStyle.fontSize = 35;

        sceneId = GUI.TextArea(new Rect(60, 450, 250, 100), sceneId, inputStyle);

        userId = GUI.TextArea(new Rect(60, 600, 250, 100), sceneId, inputStyle);

        var labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 20
        };

        GUI.Label(new Rect(400, 100, 400, 300), label, labelStyle);

        GUI.depth = 0;

        if (GUI.Button(new Rect(60, 150, 180, 100), "打开动态", style))
        {
            TapMoment.Open(Orientation.ORIENTATION_DEFAULT);
        }

        if (GUI.Button(new Rect(60, 300, 180, 100), "动态红点", style))
        {
            TapMoment.FetchNotification();
        }

        if (GUI.Button(new Rect(360, 450, 245, 100), "场景化入口", style))
        {
            TapMoment.DirectlyOpen(Orientation.ORIENTATION_DEFAULT, TapMomentConstants.TapMomentPageShortCut,
                new Dictionary<string, object> {{TapMomentConstants.TapMomentPageShortCutKey, sceneId}});
        }

        if (GUI.Button(new Rect(360, 600, 245, 100), "用户中心入口", style))
        {
            TapMoment.DirectlyOpen(Orientation.ORIENTATION_DEFAULT, TapMomentConstants.TapMomentPageUser,
                new Dictionary<string, object> {{TapMomentConstants.TapMomentPageUserKey, userId}});
        }

        if (GUI.Button(new Rect(60, 750, 180, 100), "初始化", style))
        {
            TapMoment.Init("0RiAlMny7jiz086FaU");
        }
        
        if (GUI.Button(new Rect(60, 900, 180, 100), "返回", style))
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0);

        }
    }

    private void OnApplicationPause(bool focus)
    {
        Debug.Log($"Moment Scene On Application:{focus}");
        //进入程序状态更改为前台
        if (focus)
        {
        }
        else
        {
            TapMoment.Close();
            //离开程序进入到后台状态
        }
    }
}