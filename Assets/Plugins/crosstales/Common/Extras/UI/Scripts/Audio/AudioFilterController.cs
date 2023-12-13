using UnityEngine;
using UnityEngine.UI;

namespace Crosstales.UI.Audio
{
   /// <summary>Controller for audio filters.</summary>
   public class AudioFilterController : MonoBehaviour
   {
      #region Variables

      /// <summary>Searches for all audio filters in the whole scene (default: true).</summary>
      [Header("Audio Filters")] [Tooltip("Searches for all audio filters in the whole scene (default: true).")]
      public bool FindAllAudioFiltersOnStart = true;

      public AudioReverbFilter[] ReverbFilters;
      public AudioChorusFilter[] ChorusFilters;
      public AudioEchoFilter[] EchoFilters;
      public AudioDistortionFilter[] DistortionFilters;
      public AudioLowPassFilter[] LowPassFilters;
      public AudioHighPassFilter[] HighPassFilters;

      [Header("Settings")] [Tooltip("Resets all active audio filters (default: on).")] public bool ResetAudioFiltersOnStart = true;
      public bool ChorusFilter;
      public bool EchoFilter;
      public bool DistortionFilter;
      public float DistortionFilterValue = 0.5f;
      public bool LowpassFilter;
      public float LowpassFilterValue = 5000f;
      public bool HighpassFilter;
      public float HighpassFilterValue = 5000f;

      [Header("UI Objects")] public Dropdown ReverbFilterDropdown;

      public Text DistortionText;
      public Text LowpassText;
      public Text HighpassText;

      private readonly System.Collections.Generic.List<AudioReverbPreset> _reverbPresets = new System.Collections.Generic.List<AudioReverbPreset>();
      private bool _initialized;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         System.Collections.Generic.List<Dropdown.OptionData> options = new System.Collections.Generic.List<Dropdown.OptionData>();


         foreach (AudioReverbPreset arp in System.Enum.GetValues(typeof(AudioReverbPreset)))
         {
            options.Add(new Dropdown.OptionData(arp.ToString()));

            _reverbPresets.Add(arp);
         }

         if (ReverbFilterDropdown != null)
         {
            ReverbFilterDropdown.ClearOptions();
            ReverbFilterDropdown.AddOptions(options);
         }
      }

      private void Update()
      {
         if (!_initialized && Time.frameCount % 30 == 0)
         {
            _initialized = true;

            if (FindAllAudioFiltersOnStart)
               FindAllAudioFilters();

            if (ResetAudioFiltersOnStart)
               ResetAudioFilters();
         }
      }

      #endregion


      #region Public methods

      /// <summary>Finds all audio filters in the scene.</summary>
      public void FindAllAudioFilters()
      {
#if UNITY_2023_1_OR_NEWER
         ReverbFilters = FindObjectsByType<AudioReverbFilter>(FindObjectsSortMode.None);
         ChorusFilters = FindObjectsByType<AudioChorusFilter>(FindObjectsSortMode.None);
         EchoFilters = FindObjectsByType<AudioEchoFilter>(FindObjectsSortMode.None);
         DistortionFilters = FindObjectsByType<AudioDistortionFilter>(FindObjectsSortMode.None);
         LowPassFilters = FindObjectsByType<AudioLowPassFilter>(FindObjectsSortMode.None);
         HighPassFilters = FindObjectsByType<AudioHighPassFilter>(FindObjectsSortMode.None);
#else
         ReverbFilters = FindObjectsOfType(typeof(AudioReverbFilter)) as AudioReverbFilter[];
         ChorusFilters = FindObjectsOfType(typeof(AudioChorusFilter)) as AudioChorusFilter[];
         EchoFilters = FindObjectsOfType(typeof(AudioEchoFilter)) as AudioEchoFilter[];
         DistortionFilters = FindObjectsOfType(typeof(AudioDistortionFilter)) as AudioDistortionFilter[];
         LowPassFilters = FindObjectsOfType(typeof(AudioLowPassFilter)) as AudioLowPassFilter[];
         HighPassFilters = FindObjectsOfType(typeof(AudioHighPassFilter)) as AudioHighPassFilter[];
#endif
      }

      /// <summary>Resets all audio filters.</summary>
      public void ResetAudioFilters()
      {
         ReverbFilterDropdownChanged(0);
         ChorusFilterEnabled(ChorusFilter);
         EchoFilterEnabled(EchoFilter);
         DistortionFilterEnabled(DistortionFilter);
         DistortionFilterChanged(DistortionFilterValue);
         LowPassFilterEnabled(LowpassFilter);
         LowPassFilterChanged(LowpassFilterValue);
         HighPassFilterEnabled(HighpassFilter);
         HighPassFilterChanged(HighpassFilterValue);
      }

      public void ReverbFilterDropdownChanged(int index)
      {
         foreach (AudioReverbFilter reverbFilter in ReverbFilters)
         {
            reverbFilter.reverbPreset = _reverbPresets[index];
         }
      }

      public void ChorusFilterEnabled(bool isEnabled)
      {
         foreach (AudioChorusFilter chorusFilter in ChorusFilters)
         {
            chorusFilter.enabled = isEnabled;
         }
      }

      public void EchoFilterEnabled(bool isEnabled)
      {
         foreach (AudioEchoFilter echoFilter in EchoFilters)
         {
            echoFilter.enabled = isEnabled;
         }
      }

      public void DistortionFilterEnabled(bool isEnabled)
      {
         foreach (AudioDistortionFilter distortionFilter in DistortionFilters)
         {
            distortionFilter.enabled = isEnabled;
         }
      }

      public void DistortionFilterChanged(float value)
      {
         foreach (AudioDistortionFilter distortionFilter in DistortionFilters)
         {
            distortionFilter.distortionLevel = value;
         }

         if (DistortionText != null)
            DistortionText.text = value.ToString(Crosstales.Common.Util.BaseConstants.FORMAT_TWO_DECIMAL_PLACES);
      }

      public void LowPassFilterEnabled(bool isEnabled)
      {
         foreach (AudioLowPassFilter lowPassFilter in LowPassFilters)
         {
            lowPassFilter.enabled = isEnabled;
         }
      }

      public void LowPassFilterChanged(float value)
      {
         foreach (AudioLowPassFilter lowPassFilter in LowPassFilters)
         {
            lowPassFilter.cutoffFrequency = value;
         }

         if (LowpassText != null)
            LowpassText.text = value.ToString(Crosstales.Common.Util.BaseConstants.FORMAT_NO_DECIMAL_PLACES);
      }

      public void HighPassFilterEnabled(bool isEnabled)
      {
         foreach (AudioHighPassFilter highPassFilter in HighPassFilters)
         {
            highPassFilter.enabled = isEnabled;
         }
      }

      public void HighPassFilterChanged(float value)
      {
         foreach (AudioHighPassFilter highPassFilter in HighPassFilters)
         {
            highPassFilter.cutoffFrequency = value;
         }

         if (HighpassText != null)
            HighpassText.text = value.ToString(Crosstales.Common.Util.BaseConstants.FORMAT_NO_DECIMAL_PLACES);
      }

      #endregion
   }
}
// © 2016-2023 crosstales LLC (https://www.crosstales.com)