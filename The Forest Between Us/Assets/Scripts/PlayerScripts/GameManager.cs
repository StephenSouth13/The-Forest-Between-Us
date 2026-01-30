using UnityEngine;

public class GameManager : MonoBehaviour
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
            Debug.Log("Minimap Panel Opened");
        }
    }

    public void ClosePanel()
    {
        minimapPanel.SetActive(false);
        Debug.Log("Minimap Panel Closed");
    }
}
