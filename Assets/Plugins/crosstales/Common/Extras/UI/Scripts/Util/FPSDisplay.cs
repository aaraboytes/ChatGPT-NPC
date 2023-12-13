using UnityEngine;
using UnityEngine.UI;

namespace Crosstales.UI.Util
{
   /// <summary>Simple FPS-Counter.</summary>
   [DisallowMultipleComponent]
   public class FPSDisplay : MonoBehaviour
   {
      #region Variables

      /// <summary>Text component to display the FPS.</summary>
      [Tooltip("Text component to display the FPS.")] public Text FPS;

      /// <summary>Update every set frame (default: 5).</summary>
      [Tooltip("Update every set frame (default: 5)."), Range(1, 300)] public int FrameUpdate = 5;

      [Tooltip("Key to activate the FPS counter (default: none).")] public KeyCode Key = KeyCode.None;

      private float _deltaTime;
      private float _elapsedTime;

      private float _msec;
      private float _fps;

      private const string WAIT = "<i>...calculating <b>FPS</b>...</i>";
      private const string RED = "<color=#E57373><b>FPS: {0:0.}</b> ({1:0.0} ms)</color>";
      private const string ORANGE = "<color=#FFB74D><b>FPS: {0:0.}</b> ({1:0.0} ms)</color>";
      private const string GREEN = "<color=#81C784><b>FPS: {0:0.}</b> ({1:0.0} ms)</color>";

      #endregion


      #region MonoBehaviour methods

      private void Update()
      {
         if (Key == KeyCode.None || Input.GetKey(Key))
         {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
            _elapsedTime += Time.unscaledDeltaTime;

            if (_elapsedTime > 1f)
            {
               if (Time.frameCount % FrameUpdate == 0)
               {
                  FPS.enabled = true;

                  _msec = _deltaTime * 1000f;
                  _fps = 1f / _deltaTime;

                  if (_fps < 15f)
                  {
                     FPS.text = string.Format(RED, _fps, _msec);
                  }
                  else if (_fps < 29f)
                  {
                     FPS.text = string.Format(ORANGE, _fps, _msec);
                  }
                  else
                  {
                     FPS.text = string.Format(GREEN, _fps, _msec);
                  }
               }
            }
            else
            {
               FPS.text = WAIT;
            }
         }
         else
         {
            //elapsedTime = 0;
            FPS.enabled = false;
         }
      }

      #endregion
   }
}
// © 2017-2023 crosstales LLC (https://www.crosstales.com)