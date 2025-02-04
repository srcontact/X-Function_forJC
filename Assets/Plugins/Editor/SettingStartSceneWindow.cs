//  SettingStartSceneWindow.cs
//  http://kan-kikuchi.hatenablog.com/entry/playModeStartScene
//
//  Created by kan.kikuchi on 2017.09.30.

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// エディタ上で最初に表示するシーンの設定を行うウィンドウ
/// </summary>
public class SettingStartSceneWindow : EditorWindow
{

    //設定したシーンのパスを保存するKEY
    private const string SAVE_KEY = "StartScenePathKey";
    private const string ACTIVE_KEY = "StartSceneActiveKey";
    static SceneAsset sceneAsset;
    static bool IsWork
    {
        get
        {
            string value = EditorUserSettings.GetConfigValue(ACTIVE_KEY);
            return !string.IsNullOrEmpty(value) && value.Equals("True");
        }
        set
        {
            EditorUserSettings.SetConfigValue(ACTIVE_KEY, value.ToString());
        }
    }

    //メニューからウィンドウを表示
    [MenuItem("Window/SettingStartSceneWindow")]
    public static void Open()
    {
        GetWindow<SettingStartSceneWindow>(typeof(SettingStartSceneWindow));
    }

    //初期化(ウィンドウを開いた時等に実行)
    private void OnEnable()
    {
        SetPlayModeStartScene();
    }

    private void OnGUI()
    {
        IsWork = EditorGUILayout.ToggleLeft("Active", IsWork);
        sceneAsset = (SceneAsset)EditorGUILayout.ObjectField(new GUIContent("Start Scene"), sceneAsset, typeof(SceneAsset), false);
        SetPlayModeStartScene();
    }

    private void SetPlayModeStartScene()
    {
        //保存されている最初のシーンのパスがあれば、読み込んで設定
        string startScenePath = EditorUserSettings.GetConfigValue(SAVE_KEY);
        if (!string.IsNullOrEmpty(startScenePath))
        {

            //パスからシーンを取得、シーンがなければ警告表示
            sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(startScenePath);
            if (sceneAsset == null)
            {
                Debug.LogWarning(startScenePath + "がありません！");
            }
            else
            {
                EditorSceneManager.playModeStartScene = IsWork ? sceneAsset : null;
            }

        }
    }

}