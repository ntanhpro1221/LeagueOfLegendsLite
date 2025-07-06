using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadToHomeScene : MonoBehaviour {
    private void Start() => SceneManager.LoadSceneAsync(SceneNameHelper.HomeScene);
}