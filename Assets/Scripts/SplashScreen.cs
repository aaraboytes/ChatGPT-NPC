using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    private void Start()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            var callbacks = new PermissionCallbacks();
            callbacks.PermissionGranted += OnPermissionGranted;
            callbacks.PermissionDenied += OnPermissionDenied;
            callbacks.PermissionDeniedAndDontAskAgain += OnPermissionDenied;
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
        else
        {
            Invoke(nameof(GoToGame), 0.1f);
        }
    }
    private void OnPermissionGranted(string permissionName)
    {
        Debug.Log(permissionName + " permission granted");
        GoToGame();
    }
    private void OnPermissionDenied(string permissionName)
    {
        Debug.Log(permissionName + " was not authorized by the user");
        Application.Quit();
    }
    private void GoToGame()
    {
        SceneManager.LoadScene("ChatGPTNPC");
    }
}
