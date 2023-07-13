using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// This is set up as a quick start
// You can fully customize this script and the scene file.

public class ResolutionDialog : MonoBehaviour {

	private Canvas canvas = null;

	private List<Resolution> resolutions = new List<Resolution>();
	private TMP_Dropdown dropdownResolution = null;
	private TextMeshProUGUI txtResolution = null;

	private TMP_Dropdown dropdownQuality = null;
	private TextMeshProUGUI txtQuality = null;

	private TMP_Dropdown dropdownMonitor = null;
	private TextMeshProUGUI txtMonitor = null;

	private UnityEngine.UI.Toggle toggleFullscreen = null;
	private UnityEngine.UI.Toggle toggleAutoLaunch = null;

	private TextMeshProUGUI txtDebug = null;

	private UnityEngine.UI.Button btnPlay = null;
	private UnityEngine.UI.Button btnQuit = null;

	private Plow.Launcher.AppUtils.ConfigurationData configuration = null;
	private Plow.Launcher.AppUtils.LaunchSettings lastLaunchSettings = null;

	public float autolaunchTime = 10.0f;

	public void Awake() {
		configuration = Plow.Launcher.AppUtils.Configuration;
		lastLaunchSettings = Plow.Launcher.AppUtils.GetSavedLaunchSettings();

		// Enforce the resolution of the launcher
		Screen.SetResolution(Mathf.FloorToInt(configuration.windowSize.x), Mathf.FloorToInt(configuration.windowSize.y), FullScreenMode.Windowed);

		RectTransform rectTransform = null;
		List<TMP_Dropdown.OptionData> options = null;
		Color color = Color.white;
		float disabledAlpha = 0.5f;

		canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

		btnPlay = canvas.transform.Find("BtnPlay").GetComponent<UnityEngine.UI.Button>();
		btnPlay.onClick.AddListener(OnClickBtnPlay);

		btnQuit = canvas.transform.Find("BtnQuit").GetComponent<UnityEngine.UI.Button>();
		btnQuit.onClick.AddListener(OnClickBtnQuit);

		rectTransform = canvas.transform.Find("Resolution").GetComponent<RectTransform>();
		dropdownResolution = rectTransform.Find("Dropdown").GetComponent<TMP_Dropdown>();
		txtResolution = rectTransform.Find("TxtLabel").GetComponent<TextMeshProUGUI>();

		resolutions = Plow.Launcher.AppUtils.GetResolutionOptions(true);
		options = new List<TMP_Dropdown.OptionData>();
		for (int i = 0; i < resolutions.Count; i++) {
			Resolution resolution = resolutions[i];
			TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
			option.text = resolution.width + " x " + resolution.height;
			options.Add(option);
		}

		dropdownResolution.options = options;
		dropdownResolution.value = 0;
		for (int i = 0; i < resolutions.Count; i++) {
			Resolution resolution = resolutions[i];
			if (resolution.width != lastLaunchSettings.width)
				continue;
			if (resolution.height != lastLaunchSettings.height)
				continue;
			dropdownResolution.value = i;
			break;
		}

		rectTransform = canvas.transform.Find("Quality").GetComponent<RectTransform>();
		dropdownQuality = rectTransform.Find("Dropdown").GetComponent<TMP_Dropdown>();
		txtQuality = rectTransform.Find("TxtLabel").GetComponent<TextMeshProUGUI>();
		options = new List<TMP_Dropdown.OptionData>();
		for (int i = 0; i < configuration.qualitySettings.Length; i++) {
			TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
			option.text = configuration.qualitySettings[i];

			options.Add(option);
		}
		dropdownQuality.options = options;
		dropdownQuality.value = 0;
		if (configuration.qualitySettings.Length <= 0) {
			dropdownQuality.interactable = false;
			color = txtQuality.color;
			color.a = disabledAlpha;
			txtQuality.color = color;
		}

		if (dropdownQuality.interactable && lastLaunchSettings.quality.Length > 0) {
			for (int i = 0; i < configuration.qualitySettings.Length; i++) {
				if (lastLaunchSettings.quality != configuration.qualitySettings[i])
					continue;
				dropdownQuality.value = i;
				break;
			}
		}

		List<DisplayInfo> displayInfo = new List<DisplayInfo>();
		Screen.GetDisplayLayout(displayInfo);
		int monitorCount = displayInfo.Count;

		rectTransform = canvas.transform.Find("Monitor").GetComponent<RectTransform>();
		dropdownMonitor = rectTransform.Find("Dropdown").GetComponent<TMP_Dropdown>();
		txtMonitor = rectTransform.Find("TxtLabel").GetComponent<TextMeshProUGUI>();
		options = new List<TMP_Dropdown.OptionData>();
		for (int i = 0; i < monitorCount; i++) {
			TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
			option.text = (i + 1).ToString();
			options.Add(option);
		}
		dropdownMonitor.options = options;
		dropdownMonitor.value = 0;
		if (monitorCount <= 1) {
			dropdownMonitor.interactable = false;
			color = txtQuality.color;
			color.a = disabledAlpha;
			txtMonitor.color = color;
		}
		if (dropdownMonitor.interactable && lastLaunchSettings.monitor > 0) {
			dropdownMonitor.value = lastLaunchSettings.monitor - 1;
		}

		rectTransform = canvas.transform.Find("ToggleFullscreen").GetComponent<RectTransform>();
		toggleFullscreen = rectTransform.GetComponent<UnityEngine.UI.Toggle>();
		toggleFullscreen.isOn = true;
		toggleFullscreen.isOn = lastLaunchSettings.isFullscreen;

		if (configuration.customResolutions.Length > 0) {
			toggleFullscreen.isOn = false;
			toggleFullscreen.interactable = false;
		}

		rectTransform = canvas.transform.Find("ToggleAutoLaunch").GetComponent<RectTransform>();
		toggleAutoLaunch = rectTransform.GetComponent<UnityEngine.UI.Toggle>();
		toggleAutoLaunch.isOn = true;
		toggleAutoLaunch.isOn = lastLaunchSettings.isAutolaunchOn;

		txtDebug = canvas.transform.Find("TxtDebug").GetComponent<TextMeshProUGUI>();
		txtDebug.text = "";

		// Example of using custom options
		string countdownTimeValue = Plow.Launcher.AppUtils.Configuration.GetCustomOption("AutoLaunchCountdown");
		if (countdownTimeValue != null)
			float.TryParse(countdownTimeValue, out autolaunchTime);
	}

	private bool bAttemptedLaunch = false;

	public void Update() {
		// if (Input.GetKeyDown(KeyCode.Escape))
		// 	OnClickBtnQuit();

		if (lastLaunchSettings.isAutolaunchOn && !bAttemptedLaunch) {
			if (toggleAutoLaunch.isOn)
				if (Time.realtimeSinceStartup >= autolaunchTime)
					OnClickBtnPlay();
		}
	}

	public void OnClickBtnPlay() {
		bAttemptedLaunch = true;
		Resolution resolution = resolutions[dropdownResolution.value];
		
		string strQualitySetting = string.Empty;
		if (Plow.Launcher.AppUtils.Configuration.qualitySettings.Length > 0)
			strQualitySetting = Plow.Launcher.AppUtils.Configuration.qualitySettings[dropdownQuality.value];

		Plow.Launcher.AppUtils.LaunchSettings launchSettings = new Plow.Launcher.AppUtils.LaunchSettings() {
			width = resolution.width,
			height = resolution.height,
			isFullscreen = toggleFullscreen.isOn,
			monitor = dropdownMonitor.value,
			quality = strQualitySetting,
			isAutolaunchOn = toggleAutoLaunch.isOn
		};

		bool bSuccessful = Plow.Launcher.AppUtils.LaunchApplication(new Plow.Launcher.AppUtils.LaunchSettings());

		if (!bSuccessful)
			txtDebug.text += Plow.Launcher.AppUtils.LaunchFeedback;

		if (bSuccessful)
			OnClickBtnQuit();
	}

	public void OnClickBtnQuit() {
		Application.Quit();
	}
}