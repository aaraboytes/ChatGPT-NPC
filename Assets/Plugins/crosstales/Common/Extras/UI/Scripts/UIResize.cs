using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Crosstales.UI
{
   /// <summary>Resize a UI element.</summary>
   [DisallowMultipleComponent]
   public class UIResize : MonoBehaviour, IPointerDownHandler, IDragHandler
   {
      #region Variables

      /// <summary>Minimum size of the UI element.</summary>
      [Tooltip("Minimum size of the UI element.")] public Vector2 MinSize = new Vector2(300, 160);

      /// <summary>Maximum size of the UI element.</summary>
      [Tooltip("Maximum size of the UI element.")] public Vector2 MaxSize = new Vector2(800, 600);

      /// <summary>Ignore maximum size of the UI element (default: false).</summary>
      [Tooltip("Ignore maximum size of the UI element (default: false).")] public bool IgnoreMaxSize = false;

      /// <summary>Resize speed (default: 2).</summary>
      [Tooltip("Resize speed (default: 2).")] public float SpeedFactor = 2;

      private RectTransform _panelRectTransform;
      private Vector2 _originalLocalPointerPosition;
      private Vector2 _originalSizeDelta;
      private Vector2 _originalSize;

      #endregion


      #region MonoBehaviour methods

      private void Awake()
      {
         _panelRectTransform = transform.parent.GetComponent<RectTransform>();
         Rect rect = _panelRectTransform.rect;
         _originalSize = new Vector2(rect.width, rect.height);
      }

      #endregion


      #region Implemented methods

      public void OnPointerDown(PointerEventData data)
      {
         _originalSizeDelta = _panelRectTransform.sizeDelta;

         RectTransformUtility.ScreenPointToLocalPointInRectangle(_panelRectTransform, data.position, data.pressEventCamera, out _originalLocalPointerPosition);
      }

      public void OnDrag(PointerEventData data)
      {
         if (_panelRectTransform == null)
            return;

         RectTransformUtility.ScreenPointToLocalPointInRectangle(_panelRectTransform, data.position, data.pressEventCamera, out Vector2 localPointerPosition);
         Vector3 offsetToOriginal = localPointerPosition - _originalLocalPointerPosition;

         Vector2 sizeDelta = _originalSizeDelta + new Vector2(offsetToOriginal.x * SpeedFactor, -offsetToOriginal.y * SpeedFactor);

         if (_originalSize.x + sizeDelta.x < MinSize.x)
         {
            sizeDelta.x = -(_originalSize.x - MinSize.x);
         }
         else if (!IgnoreMaxSize && _originalSize.x + sizeDelta.x > MaxSize.x)
         {
            sizeDelta.x = MaxSize.x - _originalSize.x;
         }

         if (_originalSize.y + sizeDelta.y < MinSize.y)
         {
            sizeDelta.y = -(_originalSize.y - MinSize.y);
         }
         else if (!IgnoreMaxSize && _originalSize.y + sizeDelta.y > MaxSize.y)
         {
            sizeDelta.y = MaxSize.y - _originalSize.y;
         }

         _panelRectTransform.sizeDelta = sizeDelta;
      }

      #endregion
   }
}
// © 2018-2023 crosstales LLC (https://www.crosstales.com)