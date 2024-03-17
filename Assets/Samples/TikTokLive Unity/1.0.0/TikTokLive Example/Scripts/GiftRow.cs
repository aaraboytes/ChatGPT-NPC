using TikTokLiveSharp.Events.Objects;
using TikTokLiveUnity.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TikTokLiveUnity.Example
{
    /// <summary>
    /// Displays Gift (with Updates) in ExampleScene
    /// </summary>
    public class GiftRow : MonoBehaviour
    {
        #region Properties
        /// <summary>
        /// Gift being Displayed
        /// </summary>
        public TikTokGift Gift { get; private set; }

        /// <summary>
        /// Text displaying Sender-Name
        /// </summary>
        [SerializeField]
        [Tooltip("")]
        private TMP_Text txtUserName;
        /// <summary>
        /// Text displaying Amount sent
        /// </summary>
        [SerializeField]
        [Tooltip("Text displaying Amount sent")]
        private TMP_Text txtAmount;
        /// <summary>
        /// Image displaying ProfilePicture of Sender
        /// </summary>
        [SerializeField]
        [Tooltip("Image displaying ProfilePicture of Sender")]
        private Image imgUserProfile;
        /// <summary>
        /// Image displaying Icon for Gift
        /// </summary>
        [SerializeField]
        [Tooltip("Image displaying Icon for Gift")]
        private Image imgGiftIcon;
        #endregion

        #region Methods
        /// <summary>
        /// Initializes GiftRow
        /// </summary>
        /// <param name="gift">Gift to Display</param>
        public void Init(TikTokGift gift)
        {
            Gift = gift;
            Gift.OnAmountChanged += AmountChanged;
            Gift.OnStreakFinished += StreakFinished;
            txtUserName.text = $"{Gift.Sender.UniqueId} sent a {Gift.Gift.Name}!";
            txtAmount.text = $"{Gift.Amount}x";
            RequestImage(imgUserProfile, Gift.Sender.AvatarThumbnail);
            RequestImage(imgGiftIcon, Gift.Gift.Image);
            // Run Streak-End for non-streakable gifts
            if (gift.StreakFinished)
                StreakFinished(gift, gift.Amount);
        }
        /// <summary>
        /// Deinitializes GiftRow
        /// </summary>
        private void OnDestroy()
        {
            gameObject.SetActive(false);
            if (Gift == null)
                return;
            Gift.OnAmountChanged -= AmountChanged;
            Gift.OnStreakFinished -= StreakFinished;
        }
        /// <summary>
        /// Updates Gift-Amount if Amount Changed
        /// </summary>
        /// <param name="gift">Gift for Event</param>
        /// <param name="newAmount">New Amount</param>
        private void AmountChanged(TikTokGift gift, long change, long newAmount)
        {
            txtAmount.text = $"{newAmount}x";
        }
        /// <summary>
        /// Called when GiftStreaks Ends. Starts Destruction-Timer
        /// </summary>
        /// <param name="gift">Gift for Event</param>
        /// <param name="finalAmount">Final Amount for Streak</param>
        private void StreakFinished(TikTokGift gift, long finalAmount)
        {
            AmountChanged(gift, 0, finalAmount);
            Destroy(gameObject, 2f);
        }
        /// <summary>
        /// Requests Image from TikTokLive-Manager
        /// </summary>
        /// <param name="img">UI-Image used for display</param>
        /// <param name="picture">Data for Image</param>
        private static void RequestImage(Image img, Picture picture)
        {
            Dispatcher.RunOnMainThread(() =>
            {
                if (TikTokLiveManager.Exists)
                    TikTokLiveManager.Instance.RequestSprite(picture, spr =>
                    {
                        if (img != null && img.gameObject != null && img.gameObject.activeInHierarchy)
                            img.sprite = spr;
                    });
            });
        }
        #endregion
    }
}
