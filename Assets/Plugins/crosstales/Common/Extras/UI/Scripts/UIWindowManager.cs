using UnityEngine;
using UnityEngine.UI;

namespace Crosstales.UI
{
   /// <summary>Change the state of all Window panels.</summary>
   [DisallowMultipleComponent]
   public class UIWindowManager : MonoBehaviour
   {
      #region Variables

      /// <summary>All Windows of the scene.</summary>
      [Tooltip("All Windows of the scene.")] public GameObject[] Windows;

      private Image _image;
      private GameObject _dontTouch;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         foreach (GameObject window in Windows)
         {
            _image = window.transform.Find("Panel/Header").GetComponent<Image>();

            Color c = _image.color;
            c.a = 0.2f;
            _image.color = c;
         }
      }

      #endregion


      #region Public methods

      ///<summary>Change the state of all windows.</summary>
      /// <param name="active">Active window.</param>
      public void ChangeState(GameObject active)
      {
         foreach (GameObject window in Windows)
         {
            if (window != active)
            {
               _image = window.transform.Find("Panel/Header").GetComponent<Image>();

               Color c = _image.color;
               c.a = 0.2f;
               _image.color = c;
            }

            _dontTouch = window.transform.Find("Panel/DontTouch").gameObject;

            _dontTouch.SetActive(window != active);
         }
      }

      #endregion
   }
}
// © 2017-2023 crosstales LLC (https://www.crosstales.com)