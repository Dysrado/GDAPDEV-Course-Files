using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppPaused : MonoBehaviour, ITappable
{
    //[SerializeField] Panel;
    // Start is called before the first frame update
    bool isPaused = false;
    [SerializeField] GameObject PausePanel;

    public void OnTap()
    {
        if (isPaused)
        {
            PausePanel.SetActive(false);
            Time.timeScale = 1;
        }
    }

    void Start()
    {
        PausePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused)
        {
            PausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }
    private void OnApplicationFocus(bool focus)
    {
        isPaused = !focus;
    }

    private void OnApplicationPause(bool pause)
    {
        isPaused = pause;
    }
}
