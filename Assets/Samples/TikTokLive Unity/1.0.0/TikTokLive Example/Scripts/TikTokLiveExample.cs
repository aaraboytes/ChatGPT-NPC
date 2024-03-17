using System.Collections;
using TikTokLiveSharp.Client;
using TikTokLiveSharp.Events;
using TikTokLiveSharp.Events.Objects;
using TikTokLiveUnity.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TikTokLiveUnity.Example
{
    /// <summary>
    /// Example-Script for TikTokLiveUnity
    /// </summary>
    public class TikTokLiveExample : MonoBehaviour
    {
        #region Properties
        [Header("Settings")]
        [SerializeField]
        [Tooltip("Duration for objects to exist")]
        private float timeToLive = 3f;

        /// <summary>
        /// ScrollRect for Join-Texts
        /// </summary>
        [Header("ScrollRects")]
        [SerializeField]
        [Tooltip("ScrollRect for Join-Texts")]
        private ScrollRect scrJoin;
        /// <summary>
        /// ScrollRect for Like-Texts
        /// </summary>
        [SerializeField]
        [Tooltip("ScrollRect for Like-Texts")]
        private ScrollRect scrLike;
        /// <summary>
        /// ScrollRect for Gift-Texts
        /// </summary>
        [SerializeField]
        [Tooltip("ScrollRect for Gift-Texts")]
        private ScrollRect scrGift;
        /// <summary>
        /// ScrollRect for Comment-Texts
        /// </summary>
        [SerializeField]
        [Tooltip("ScrollRect for Comment-Texts")]
        private ScrollRect scrComment;

        /// <summary>
        /// Title for Status-Panel
        /// </summary>
        [Header("StatusPanel")]
        [SerializeField]
        [Tooltip("Title for Status-Panel")]
        private TMP_Text txtStatusTitle;
        /// <summary>
        /// Text displaying Host connected to
        /// </summary>
        [SerializeField]
        [Tooltip("Text displaying Host connected to")]
        private TMP_Text txtStatusHostId;
        /// <summary>
        /// InputField for Host to connect to
        /// </summary>
        [SerializeField]
        [Tooltip("InputField for Host to connect to")]
        private TMP_InputField ifHostId;
        /// <summary>
        /// InputField for Host to connect to
        /// </summary>
        [SerializeField]
        [Tooltip("InputField for Host to connect to")]
        private TMP_InputField ifRoomId;
        /// <summary>
        /// Connect-Button
        /// </summary>
        [SerializeField]
        [Tooltip("Connect-Button")]
        private Button btnConnect;

        /// <summary>
        /// Prefab for Rows to display Info
        /// </summary>
        [Header("Prefabs")]
        [SerializeField]
        [Tooltip("Prefab for Rows to display Info")]
        private GameObject rowPrefab;
        /// <summary>
        /// Prefab for Row to display Gift
        /// </summary>
        [SerializeField]
        [Tooltip("Prefab for Row to display Gift")]
        private GiftRow giftRowPrefab;

        /// <summary>
        /// ShortHand for TikTokLiveManager-Access
        /// </summary>
        private TikTokLiveManager mgr => TikTokLiveManager.Instance;
        #endregion

        #region Methods
        #region Unity
        /// <summary>
        /// Initializes this Object
        /// </summary>
        private IEnumerator Start()
        {
            btnConnect.onClick.AddListener(OnClick_Connect);
            mgr.OnConnected += ConnectStatusChange;
            mgr.OnDisconnected += ConnectStatusChange;
            mgr.OnJoin += OnJoin;
            mgr.OnLike += OnLike;
            mgr.OnChatMessage += OnComment;
            mgr.OnGift += OnGift;
            for (int i = 0; i < 3; i++)
                yield return null; // Wait 3 frames in case Auto-Connect is enabled
            UpdateStatus();
        }
        /// <summary>
        /// Deinitializes this Object
        /// </summary>
        private void OnDestroy()
        {
            btnConnect.onClick.RemoveListener(OnClick_Connect);
            if (!TikTokLiveManager.Exists)
                return;
            mgr.OnConnected -= ConnectStatusChange;
            mgr.OnDisconnected -= ConnectStatusChange;
            mgr.OnJoin -= OnJoin;
            mgr.OnLike -= OnLike;
            mgr.OnChatMessage -= OnComment;
            mgr.OnGift -= OnGift;
        }
        #endregion

        #region Private
        /// <summary>
        /// Handler for Connect-Button
        /// </summary>
        private void OnClick_Connect()
        {
            bool connected = mgr.Connected;
            bool connecting = mgr.Connecting;
            if (connected || connecting)
                mgr.DisconnectFromLivestreamAsync();
            else
            {
                if (!string.IsNullOrEmpty(ifRoomId.text))
                    mgr.ConnectToRoomAsync(ifRoomId.text, Debug.LogException);
                else
                    mgr.ConnectToStreamAsync(ifHostId.text, Debug.LogException);
            }
            UpdateStatus();
            Invoke(nameof(UpdateStatus), .5f);
        }
        /// <summary>
        /// Handler for Connection-Events. Updates StatusPanel
        /// </summary>
        private void ConnectStatusChange(TikTokLiveClient sender, bool e) => UpdateStatus();
        /// <summary>
        /// Handler for Gift-Event
        /// </summary>
        private void OnGift(TikTokLiveClient sender, TikTokGift gift)
        {
            GiftRow instance = Instantiate(giftRowPrefab);
            instance.Init(gift);
            instance.transform.SetParent(scrGift.content, false);
            instance.transform.localScale = Vector3.one;
            instance.gameObject.SetActive(true);
        }
        /// <summary>
        /// Handler for Join-Event
        /// </summary>
        private void OnJoin(TikTokLiveClient sender, Join join)
        {
            GameObject instance = Instantiate(rowPrefab);
            Image img = instance.GetComponentInChildren<Image>();
            RequestImage(img, join.User.AvatarThumbnail);
            TMP_Text txt = instance.GetComponentInChildren<TMP_Text>();
            txt.text = $"{join.User.UniqueId} Joined the stream";
            instance.transform.SetParent(scrJoin.content, false);
            instance.transform.localScale = Vector3.one;
            instance.SetActive(true);
            Destroy(instance, timeToLive);
        }
        /// <summary>
        /// Handler for Like-Event
        /// </summary>
        private void OnLike(TikTokLiveClient sender, Like like)
        {
            GameObject instance = Instantiate(rowPrefab);
            Image img = instance.GetComponentInChildren<Image>();
            RequestImage(img, like.Sender.AvatarThumbnail);
            TMP_Text txt = instance.GetComponentInChildren<TMP_Text>();
            txt.text = $"{like.Sender.UniqueId} {like.Count}x Liked the stream";
            instance.transform.SetParent(scrLike.content, false);
            instance.transform.localScale = Vector3.one;
            instance.SetActive(true);
            Destroy(instance, timeToLive);
        }
        /// <summary>
        /// Handler for Comment-Event
        /// </summary>
        private void OnComment(TikTokLiveClient sender, Chat comment)
        {
            GameObject instance = Instantiate(rowPrefab);
            Image img = instance.GetComponentInChildren<Image>();
            RequestImage(img, comment.Sender.AvatarThumbnail);
            TMP_Text txt = instance.GetComponentInChildren<TMP_Text>();
            txt.text = $"{comment.Sender.UniqueId} - {comment.Message}";
            instance.transform.SetParent(scrComment.content, false);
            instance.transform.localScale = Vector3.one;
            instance.SetActive(true);
            Destroy(instance, timeToLive);
        }
        /// <summary>
        /// Requests Image from TikTokLive-Manager
        /// </summary>
        /// <param name="img">UI-Image used for display</param>
        /// <param name="picture">Data for Image</param>
        private void RequestImage(Image img, Picture picture)
        {
            Dispatcher.RunOnMainThread(() =>
            {
                mgr.RequestSprite(picture, spr =>
                {
                    if (img != null && img.gameObject != null && img.gameObject.activeInHierarchy)
                        img.sprite = spr;
                });
            });
        }
        /// <summary>
        /// Updates Status-Panel based on ConnectionState
        /// </summary>
        private void UpdateStatus()
        {
            bool connected = mgr.Connected;
            bool connecting = mgr.Connecting;
            txtStatusTitle.text = connected ? "Connected to:" : connecting ? "Connecting to:" : "Connect to:";
            txtStatusHostId.gameObject.SetActive(connecting || connected);
            if (connected || connecting)
                txtStatusHostId.text = string.IsNullOrWhiteSpace(mgr.HostName) ? mgr.RoomId : mgr.HostName;
            ifHostId.gameObject.SetActive(!connected && !connecting);
            ifRoomId.gameObject.SetActive(!connected && !connecting);
            btnConnect.GetComponentInChildren<TMP_Text>().text = connected ? "Disconnect" : connecting ? "Cancel" : "Connect";
        }
        #endregion
        #endregion
    }
}
