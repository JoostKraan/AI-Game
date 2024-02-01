using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

public class UI_Functions : MonoBehaviour
{
    public void SwitchScenes(string sceneName) => EditorSceneManager.LoadScene(sceneName);

	public void QuitGame() => Application.Quit();
}
