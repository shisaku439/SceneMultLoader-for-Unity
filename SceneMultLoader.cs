using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System;

public class SceneMultLoader
{
    //シングルトン
    public static SceneMultLoader instance;

    //遷移待機中のシーン
    private Dictionary<string, AsyncOperation> standbyScene;

    //シーンリスト（GetStandbyScene用）
    private List<string> sceneList = new List<string>();

    //インスタンスの取得
    private static SceneMultLoader GetInstance()
    {
        if (instance == null)
        {
            //初期化
            instance = new SceneMultLoader();
            instance.standbyScene = new Dictionary<string, AsyncOperation>();
        }
        return instance;
    }

    /// <summary>
    /// シーンの読み込み
    /// </summary>
    /// <param name="sceneName">シーン名</param>
    /// <param name="mode">single : 通常、Additive : 追加</param>
    /// <returns>true : 成功 | false : 失敗</returns>
    public static bool SetScene(string sceneName,int mode=0)
    {
        return GetInstance()._setScene(sceneName,mode);
    }

    /// <summary>
    /// シーンの読み込み
    /// </summary>
    /// <param name="scene">シーン</param>
    /// <param name="mode">true : 成功 | false : 失敗</param>
    /// <returns></returns>
    public static bool SetScene(Scene scene, int mode=0)
    {
        return GetInstance()._setScene(scene.name, mode);
    }

    private bool _setScene(string sceneName, int mode = 0)
    {
        //シーンが既に登録されているなら False を返す
        if (StandByContains(sceneName)) return false;

        var LoadMode = (LoadSceneMode)Enum.ToObject(typeof(LoadSceneMode), mode);

        //シーン読み込み
        var op = SceneManager.LoadSceneAsync(sceneName, LoadMode);
        op.allowSceneActivation = false;
        //シーン登録
        standbyScene.Add(sceneName, op);

        return true;
    }

    /// <summary>
    /// シーン遷移
    /// </summary>
    /// <param name="sceneName">シーン名</param>
    /// <returns>True : シーン遷移成功 | False : 失敗</returns>
    public static bool OpenScene(string sceneName)
    {
        return GetInstance()._openScene(sceneName);
    }

    /// <summary>
    /// シーン遷移
    /// </summary>
    /// <param name="scene">シーン</param>
    /// <returns>True : シーン遷移成功 | False : 失敗</returns>
    public static bool OpenScene(Scene scene)
    {
        return GetInstance()._openScene(scene.name);
    }

    private bool _openScene(string sceneName)
    {
        //シーンが登録されていないなら False を返す
        if (!_standByContains(sceneName)) return false;

        //シーン遷移
        var op = standbyScene[sceneName];
        op.allowSceneActivation = true;

        //登録解除
        standbyScene.Remove(sceneName);
        return true;
    }

    /// <summary>
    /// シーン削除
    /// </summary>
    /// <param name="sceneName">シーン名</param>
    /// <returns>True : 成功 | False : 失敗</returns>
    public static async UniTask<bool> CloseScene(string sceneName)
    {
        return await GetInstance()._closeScene(sceneName);
    }

    /// <summary>
    /// シーン削除
    /// </summary>
    /// <param name="scene">シーン名</param>
    /// <returns>True : 成功 | False : 失敗</returns>
    public static async UniTask<bool> CloseScene(Scene scene)
    {
        return await GetInstance()._closeScene(scene.name);
    }

    private async UniTask<bool> _closeScene(string sceneName)
    {
        //ゲームにシーンが存在しない場合
        if (!_containsScene(sceneName)) return false;

        //シーン削除
        await SceneManager.UnloadSceneAsync(sceneName);

        return true;
    }


    /// <summary>
    /// シーンの読み込み状況取得
    /// </summary>
    /// <param name="sceneName">シーン名</param>
    /// <returns>0~0.9 : 読み込み状況 | -1 : 取得失敗</returns>
    public static float GetProgress(string sceneName)
    {
        return GetInstance()._getProgress(sceneName);
    }

    /// <summary>
    /// シーンの読み込み状況取得
    /// </summary>
    /// <param name="scene">シーン</param>
    /// <returns>0~0.9 : 読み込み状況 | -1 : 取得失敗</returns>
    public static float GetProgress(Scene scene)
    {
        return GetInstance()._getProgress(scene.name);
    }

    private float _getProgress(string sceneName)
    {
        //シーンが既に登録されていないなら False を返す
        if (!StandByContains(sceneName)) return -1;

        var op = standbyScene[sceneName];

        return op.progress;
    }

    /// <summary>
    /// 遷移待機中のシーンに同名のシーンがあるか判別
    /// </summary>
    /// <param name="sceneName">シーン名</param>
    /// <returns>True : 有る | False : 無い</returns>
    public static bool StandByContains(string sceneName)
    {
        return GetInstance()._standByContains(sceneName);
    }
    private bool _standByContains(string sceneName)
    {
        if (standbyScene.ContainsKey(sceneName)) return true;
        return false;
    }


    /// <summary>
    /// 遷移待機中のシーン取得
    /// </summary>
    /// <returns>シーン名のList</returns>
    public static List<string> GetStandbyScene()
    {
        return GetInstance()._getStandbyScene();
    }

    private List<string> _getStandbyScene()
    {
        sceneList.Clear();
        foreach(string scene in standbyScene.Keys)
        {
            sceneList.Add(scene);
        }

        return sceneList;
    }


    /// <summary>
    /// シーンが既に存在しているか判別
    /// </summary>
    /// <param name="sceneName">シーン名</param>
    /// <returns>True : 有る | Flase 無い</returns>
    public static bool ContainsScene(string sceneName)
    {
        return GetInstance()._containsScene(sceneName);
    }
    public static bool ContainsScene(Scene scene)
    {
        return GetInstance()._containsScene(scene.name);
    }

    private bool _containsScene(string sceneName)
    {
        for(int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == sceneName) return true;
        }
        return false;
    }
}
