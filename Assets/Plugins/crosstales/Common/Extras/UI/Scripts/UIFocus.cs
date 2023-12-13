using UnityEngine;
using UnityEngine.UI;

namespace Crosstales.UI
{
   /// <summary>Change the Focus on from a Window.</summary>
   [DisallowMultipleComponent]
   public class UIFocus : MonoBehaviour
   {
      #region Variables

      /// <summary>Name of the gameobject containing the UIWindowManager.</summary>
      [Tooltip("Name of the gameobject containing the UIWindowManager.")] public string ManagerName = "Canvas";

      private UIWindowManager _manager;
      private Image _image;

      private Transform _tf;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         //do nothing, just allow to enable/disable the script
      }

      private void Awake()
      {
         _tf = transform;

         _manager = GameObject.Find(ManagerName).GetComponent<UIWindowManager>();

         _image = _tf.Find("Panel/Header").GetComponent<Image>();
      }

      #endregion


      #region Public methods

      ///<summary>Panel entered.</summary>
      public void OnPanelEnter()
      {
         if (_manager != null)
            _manager.ChangeState(gameObject);

         Color c = _image.color;
         c.a = 255;
         _image.color = c;

         _tf.SetAsLastSibling(); //move to the front (on parent)
         _tf.SetAsFirstSibling(); //move to the back (on parent)
         _tf.SetSiblingIndex(-1); //move to position, whereas 0 is the back-most, transform.parent.childCount -1 is the front-most position
         _tf.GetSiblingIndex(); //get the position in the hierarchy (on parent)
      }

      #endregion
   }
}
// © 2017-2023 crosstales LLC (https://www.crosstales.com)