using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    [SerializeField]
    private Canvas joinCanvas;
    [SerializeField]
    private Canvas settingsCanvas;
    [SerializeField]
    private Canvas mainCanvas;
    [SerializeField]
    private TMP_Dropdown fullscreenDropdown;
    [SerializeField]
    private TMP_Dropdown resolutionDropdown;

    private Canvas currentCanvas;
    private Resolution[] resolutions;

    // Start is called before the first frame update
    void Start()
    {
        currentCanvas = mainCanvas;
        mainCanvas.gameObject.SetActive(true);
        settingsCanvas.gameObject.SetActive(false);
        joinCanvas.gameObject.SetActive(false);

        resolutions = Screen.resolutions;
        List<TMP_Dropdown.OptionData> opts = new();
        for (int i = resolutions.Length - 1; i >= 0; i--)
        {
            var res = resolutions[i];
            opts.Add(new TMP_Dropdown.OptionData(res.width + "x" + res.height + ": " + res.refreshRateRatio + "hz"));
        }
        resolutionDropdown.AddOptions(opts);
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenJoin()
    {
        currentCanvas.gameObject.SetActive(false);
        currentCanvas = joinCanvas;
        joinCanvas.gameObject.SetActive(true);
    }

    public void OpenSettings()
    {
        currentCanvas.gameObject.SetActive(false);
        currentCanvas = settingsCanvas;
        settingsCanvas.gameObject.SetActive(true);
    }

    public void OpenMain()
    {
        currentCanvas.gameObject.SetActive(false);
        currentCanvas = mainCanvas;
        mainCanvas.gameObject.SetActive(true);
    }

    public void SetFullscreen()
    {
        switch (fullscreenDropdown.value)
        {
            case 2:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
        }
    }

    public void SetResolution()
    {
        var res = resolutions[resolutions.Length - resolutionDropdown.value - 1];
        Screen.SetResolution(res.width,
            res.height,
            Screen.fullScreenMode,
            res.refreshRateRatio);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
