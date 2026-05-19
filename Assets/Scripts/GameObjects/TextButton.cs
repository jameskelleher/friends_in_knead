using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextButton : MonoBehaviour
{
    public TMP_FontAsset clearFont;
    public TMP_FontAsset opaqueFont;
    public InitialEntry initialEntry;
    public Option option;
    public float slowNextLetterDelay = .5f;
    public float fastNextLetterDelay = .2f;

    [HideInInspector]
    public static bool optionSelected;

    TextMeshPro _text;
    float _startedAt;

    float _nextLetterElapsed;
    float _currentLetterDelay;


    void Start()
    {
        optionSelected = false;
        _startedAt = Time.time;

        _text = GetComponent<TextMeshPro>();
        _text.font = clearFont;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (optionSelected || Time.time - _startedAt < 1)
            return;

        optionSelected = true;
        _text.font = opaqueFont;

        switch (option)
        {
            case Option.NextLetter:
                _nextLetterElapsed = 0f;
                _currentLetterDelay = slowNextLetterDelay;
                initialEntry.StartBlink();
                initialEntry.NextChar();
                break;
            case Option.ConfirmLetter:
                initialEntry.ConfirmCurrentChar();
                break;
            default:
                throw new ArgumentOutOfRangeException($"unexpected tutorial option {option}");
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (option != Option.NextLetter) return;

        _nextLetterElapsed += Time.deltaTime;
        if (_nextLetterElapsed > _currentLetterDelay)
        {
            _currentLetterDelay = fastNextLetterDelay;
            initialEntry.NextChar();
            _nextLetterElapsed = 0;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        optionSelected = false;
        _text.font = clearFont;
        initialEntry.StartBlink();
    }

    IEnumerator StartGame(Action nextScene)
    {
        yield return new WaitForSeconds(2f);
        nextScene();
    }

    public enum Option { NextLetter, ConfirmLetter }
}
