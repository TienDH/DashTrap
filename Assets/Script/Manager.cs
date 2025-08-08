using UnityEngine;

public class Manager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject panelLv;
    [SerializeField] private GameObject close;
    private void Start()
    {
        panelLv.SetActive(false);
        close.SetActive(false);
    }
    public void TogglePanel()
    {
            panelLv.SetActive(true);
            close.SetActive(true);
    }
    public void ToggleClose()
    {
        panelLv.SetActive(false);
        close.SetActive(false);
    }
}
