using UnityEngine;

public class MinimapManager : MonoBehaviour
{
    public GameObject minimapPanel;
    
    void Start()
    {
        minimapPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        OpenPanel();
    }

    public void OpenPanel()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            minimapPanel.SetActive(true);
            PlayerAvaterEffect.instance.ShowIndicator();
        }
    }

    public void ClosePanel()
    {
        minimapPanel.SetActive(false);
        PlayerAvaterEffect.instance.HideIndicator();
    }
}
