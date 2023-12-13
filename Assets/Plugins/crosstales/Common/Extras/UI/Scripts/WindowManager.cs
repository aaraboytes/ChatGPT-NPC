using UnityEngine;

namespace Crosstales.UI
{
   /// <summary>Manager for a Window.</summary>
   public class WindowManager : MonoBehaviour
   {
      #region Variables

      /// <summary>Window movement speed (default: 3).</summary>
      [Tooltip("Window movement speed (default: 3).")] public float Speed = 3f;

      /// <summary>Dependent GameObjects (active == open).</summary>
      [Tooltip("Dependent GameObjects (active == open).")] public GameObject[] Dependencies;

      /// <summary>Close the window at Start (default: true).</summary>
      [Tooltip("Close the window at Start (default: true).")] public bool ClosedAtStart = true;

      private UIFocus _focus;

      private bool _open;
      private bool _close;

      private Vector3 _startPos;
      private Vector3 _centerPos;
      private Vector3 _lerpPos;

      private float _openProgress;
      private float _closeProgress;

      private GameObject _panel;

      private Transform _tf;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         _tf = transform;

         _panel = _tf.Find("Panel").gameObject;

         _startPos = _tf.position;

         if (ClosedAtStart)
         {
            ClosePanel();

            _panel.SetActive(false);

            if (Dependencies != null)
            {
               foreach (GameObject go in Dependencies)
               {
                  go.SetActive(false);
               }
            }
         }
         else
         {
            OpenPanel();
         }
      }

      private void Update()
      {
         _centerPos = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

         if (_open && _openProgress < 1f)
         {
            _openProgress += Speed * Time.deltaTime;

            _tf.position = Vector3.Lerp(_lerpPos, _centerPos, _openProgress);
         }
         else if (_close)
         {
            if (_closeProgress < 1f)
            {
               _closeProgress += Speed * Time.deltaTime;

               _tf.position = Vector3.Lerp(_lerpPos, _startPos, _closeProgress);
            }
            else
            {
               _panel.SetActive(false);

               if (Dependencies != null)
               {
                  foreach (GameObject go in Dependencies)
                  {
                     go.SetActive(false);
                  }
               }
            }
         }
      }

      #endregion


      #region Public methods

      ///<summary>Switch between open and close.</summary>
      public void SwitchPanel()
      {
         if (_open)
         {
            ClosePanel();
         }
         else
         {
            OpenPanel();
         }
      }

      ///<summary>Open the panel.</summary>
      public void OpenPanel()
      {
         _panel.SetActive(true);

         if (Dependencies != null)
         {
            foreach (GameObject go in Dependencies)
            {
               go.SetActive(true);
            }
         }

         _focus = gameObject.GetComponent<UIFocus>();
         _focus.OnPanelEnter();

         _lerpPos = _tf.position;
         _open = true;
         _close = false;
         _openProgress = 0f;
      }

      ///<summary>Close the panel.</summary>
      public void ClosePanel()
      {
         _lerpPos = _tf.position;
         _open = false;
         _close = true;
         _closeProgress = 0f;
      }

      #endregion
   }
}
// © 2017-2023 crosstales LLC (https://www.crosstales.com)