using UnityEngine;

namespace Crosstales.UI
{
   /// <summary>Allow to Drag the Windows around.</summary>
   [DisallowMultipleComponent]
   public class UIDrag : MonoBehaviour
   {
      #region Variables

      private float _offsetX;
      private float _offsetY;

      private Transform _tf;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         _tf = transform;
      }

      #endregion


      #region Public methods

      ///<summary>Drag started.</summary>
      public void BeginDrag()
      {
         Vector3 position = _tf.position;
         _offsetX = position.x - Input.mousePosition.x;
         _offsetY = position.y - Input.mousePosition.y;
      }

      ///<summary>While dragging.</summary>
      public void OnDrag()
      {
         _tf.position = new Vector3(_offsetX + Input.mousePosition.x, _offsetY + Input.mousePosition.y);
      }

      #endregion
   }
}
// © 2017-2023 crosstales LLC (https://www.crosstales.com)