using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TitleScreenRunner : MonoBehaviour
{
    public TriggerRelay playTutorialTrigger;
    public TriggerRelay skipTutorialTrigger;
    public TriggerRelay easyModeTrigger;
    public TriggerRelay hardModeTrigger;
    public TMP_FontAsset clearFont;
    public TMP_FontAsset opaqueFont;
    public GameObject titleDefaultObject;
    public GameObject difficultySelectObject;
    public float floatSpeed = 1f;
    public float floatHeight = 1f;

    bool _optionSelected;
    bool _easyOptionReady;
    bool _hardOptionReady;
    TMP_Text _playText;
    TMP_Text _skipText;
    TMP_Text _easyText;
    TMP_Text _hardText;

    void Awake()
    {
        _optionSelected = false;
        _easyOptionReady = false;
        _hardOptionReady = false;

        playTutorialTrigger.onTriggerEnter.AddListener(PlayTutorialTriggeredEntered);

        skipTutorialTrigger.onTriggerEnter.AddListener(SkipTutorialTriggerEntered);

        easyModeTrigger.onTriggerEnter.AddListener(easyModeTriggerEntered);
        easyModeTrigger.onTriggerStay.AddListener((Collider2D col) => _easyOptionReady = false);
        easyModeTrigger.onTriggerExit.AddListener((Collider2D col) => _easyOptionReady = true);

        hardModeTrigger.onTriggerEnter.AddListener(hardModeTriggerEntered);
        hardModeTrigger.onTriggerStay.AddListener((Collider2D col) => _hardOptionReady = false);
        hardModeTrigger.onTriggerExit.AddListener((Collider2D col) => _hardOptionReady = true);

        _playText = playTutorialTrigger.GetComponent<TMP_Text>();
        _skipText = skipTutorialTrigger.GetComponent<TMP_Text>();
        _easyText = easyModeTrigger.GetComponent<TMP_Text>();
        _hardText = hardModeTrigger.GetComponent<TMP_Text>();

        _playText.font = clearFont;
        _skipText.font = clearFont;
        _easyText.font = clearFont;
        _hardText.font = clearFont;

        titleDefaultObject.SetActive(true);
        difficultySelectObject.SetActive(false);

        UpdateFloatParams();
    }

    void OnValidate()
    {
        UpdateFloatParams();
    }

    void PlayTutorialTriggeredEntered(Collider2D collider)
    {
        if (_optionSelected) return;

        _optionSelected = true;
        _playText.font = opaqueFont;

        StartCoroutine(DelayAction(SceneNav.GoToTutorial));
    }

    void SkipTutorialTriggerEntered(Collider2D collider)
    {
        if (_optionSelected) return;

        _optionSelected = true;
        _skipText.font = opaqueFont;

        StartCoroutine(EnableDifficultySelect());
    }

    void easyModeTriggerEntered(Collider2D collider)
    {
        if (!_easyOptionReady || _optionSelected) return;
        _optionSelected = true;
        _easyText.font = opaqueFont;
        StaticData.difficulty = Difficulty.Easy;
        StartCoroutine(DelayAction(SceneNav.GoToGame));
    }

    void hardModeTriggerEntered(Collider2D collider)
    {
        Debug.Log("entered");
        if (!_hardOptionReady || _optionSelected) return;
        _optionSelected = true;
        _hardText.font = opaqueFont;
        StaticData.difficulty = Difficulty.Hard;
        StartCoroutine(DelayAction(SceneNav.GoToGame));
    }

    IEnumerator EnableDifficultySelect()
    {
        yield return new WaitForSeconds(1f);
        
        titleDefaultObject.SetActive(false);
        difficultySelectObject.SetActive(true);

        _optionSelected = false;
        _easyOptionReady = false;
        _hardOptionReady = false;

        yield return new WaitForFixedUpdate();

        _easyOptionReady = true;
        _hardOptionReady = true;
    }

    IEnumerator DelayAction(Action action)
    {
        yield return new WaitForSeconds(1f);
        action();
    }

    void UpdateFloatParams()
    {
        playTutorialTrigger.floatSpeed = floatSpeed;
        playTutorialTrigger.floatHeight = floatHeight;

        skipTutorialTrigger.floatSpeed = floatSpeed;
        skipTutorialTrigger.floatHeight = floatHeight;
    }
}
