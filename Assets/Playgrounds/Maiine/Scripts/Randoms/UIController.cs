using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private List<Image> UI;
    [SerializeField] private bool UIOn;

    [SerializeField] private List<GameObject> _demonHearths;
    [SerializeField] private List<GameObject> _angelHearths;

    [SerializeField] private GameObject _demonUI;
    [SerializeField] private GameObject _angelUI;

    [SerializeField] private GameObject _demonPlayer;
    [SerializeField] private GameObject _angelPlayer;

    [SerializeField] private GameObject _demonHearthBeatUI;
    [SerializeField] private GameObject _angelHearthBeatUI;

    private bool _isDemond = true;
    private int _uiDemondLife = 3;
    private int _uiAngelLife = 3;
    private Color c;

    public RectTransform rtDemon;
    public RectTransform rtAngel;
    Vector2 pos;

    private void Start()
    {
        c.a = 1;

        EventManager.Subscribe("UITakeDamage", TakeUIDMG);
        EventManager.Subscribe("UITakeHealth", HealthUI);
        EventManager.Subscribe("OnPlayerChange", ChangeUI);
    }

    private void FixedUpdate()
    {
        if (_isDemond)
        {
            rtDemon.position = RectTransformUtility.WorldToScreenPoint(Camera.main, _demonPlayer.transform.position) + new Vector2(0, 25);
        }
        else
        {
            rtAngel.position = RectTransformUtility.WorldToScreenPoint(Camera.main, _angelPlayer.transform.position) + new Vector2(0, 25);
        }
    }

    public void ChangeUI(object[] parameters)
    {
        _isDemond = !_isDemond;
        if (_isDemond)
        {
            _demonUI.gameObject.SetActive(true);
            _angelUI.gameObject.SetActive(false);
        }
        else
        {
            _demonUI.gameObject.SetActive(false);
            _angelUI.gameObject.SetActive(true);
        }
    }

    public void TakeUIDMG(object[] parameters)
    {
        if ((bool)parameters[0])
        {
            _uiDemondLife--;
            _demonHearths[_uiDemondLife].gameObject.SetActive(false);
            if (_uiDemondLife == 1)
                _demonHearthBeatUI.SetActive(true);
        }
        else
        {
            _uiAngelLife--;
            _angelHearths[_uiAngelLife].gameObject.SetActive(false);
            if (_uiAngelLife == 1)
                _angelHearthBeatUI.SetActive(true);
        }
    }

    public void HealthUI(object[] parameters)
    {
        if ((bool)parameters[0])
        {
            _demonHearths[_uiDemondLife].gameObject.SetActive(true);
            _uiDemondLife++;
            _demonHearthBeatUI.SetActive(false);
        }
        else
        {
            _angelHearths[_uiAngelLife].gameObject.SetActive(true);
            _uiAngelLife++;
            _angelHearthBeatUI.SetActive(false);
        }
    }
}
