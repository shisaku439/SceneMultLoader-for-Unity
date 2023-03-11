# SceneMultLoader for Unity
 SceneChange for Unity

### Unity上で1つ以上のシーンを遷移、管理する目的のために作ったアセットです。

#### ※注意点　読み込んだシーンの順と遷移させるシーンの順は変更することができません。
```
例えば、
シーンA、シーンB、シーンC、の順番でシーンを読み込んで待機させた場合
シーンA、シーンB、シーンC、の順番でしか遷移しません。
シーンB、シーンA、シーンC、など順番を変えて遷移させることはできません。

原因は、
AsyncOperation.allowSceneActivation を使っているためです。
シーンAの allowSceneActivation = true を実行しないと
次のシーンBの allowSceneActivation = true が実行されないようです。
```
　[AsyncOperation.allowSceneActivatio の公式リファレンス参照](https://docs.unity3d.com/ja/2019.4/ScriptReference/AsyncOperation-allowSceneActivation.html)


# 主なメソッド一覧

#### シーン読み込み
```
  SetScene(string sceneName,int mode)
   sceneName:遷移先のシーン名
   mode:遷移の仕方
    0:single 通常
    1:additive 追加　
    
 （この段階ではシーン遷移はせず裏でシーンの読み込みを開始する）

```

#### シーンの読み込み状況
```
  GetProgress(sceneName)
   sceneName:読み込みの進捗を知りたいシーン名
   
   返り値は　float で0~0.9の値を返す。（0.9が読み込み完了）
   シーン名が一致しない場合　‐１を返す。
```

#### 読み込み完了したシーンの遷移
```
  OpenScene(string sceneName)
   sceneName:読み込んだシーン名
```   

#### ゲームシーンにあるシーンの削除
```
  CloseScene(string sceneName)
   sceneName:削除したいシーン名
```



