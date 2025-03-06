using Unity.Entities;
using UnityEngine.SceneManagement;

public partial class BattleScene2ConnectionSceneSystem : SystemBase {
    protected override void OnCreate() {
        var curScene = SceneManager.GetActiveScene();
        if (curScene.name == SceneNameHelper.BattleScene) {
            SceneManager.LoadScene(SceneNameHelper.ConnectionScene);
        }

        Enabled = false;
    }

    protected override void OnUpdate() { }
}