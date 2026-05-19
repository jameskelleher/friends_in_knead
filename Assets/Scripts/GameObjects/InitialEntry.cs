using System.Collections;
using TMPro;
using UnityEngine;

public class InitialEntry : MonoBehaviour
{
    public TMP_Text ordinalText;
    public TMP_Text initialsText;
    public TMP_Text scoreText;
    public float interval = 0.25f;
    [Range(0f, 1f)]
    public float monospaceAmount;

    bool _isBlinked;
    char[] _initials = new char[] { 'A', 'A', 'A' };
    int _currentIx;
    char _currentChar;
    Coroutine _blinkCoroutine;
    int _asciiIx = 0;
    const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ.-!?0123456789";


    void OnEnable()
    {
        _currentIx = 0;
        _currentChar = 'A';
        scoreText.text = $"- {StaticData.Score}";

        DisplayInitials();
        StopBlink();
        StartBlink();
    }

    public void SetOrdinal(int ord) => ordinalText.text = $"{ord}.";

    IEnumerator Blink()
    {
        while (_currentIx < _initials.Length)
        {
            yield return new WaitForSeconds(interval);
            _isBlinked = !_isBlinked;
            _initials[_currentIx] = _isBlinked ? '_' : _currentChar;
            DisplayInitials();
        }
    }

    public void NextChar()
    {
        if (_currentIx >= _initials.Length) return;

        _asciiIx = (_asciiIx + 1) % Letters.Length;
        char c = Letters[_asciiIx];

        _initials[_currentIx] = c;
        _currentChar = c;

        DisplayInitials();
    }

    public void ConfirmCurrentChar()
    {
        if (_currentIx >= _initials.Length) return;

        StopBlink();
        _initials[_currentIx] = _currentChar;
        DisplayInitials();
        _currentIx++;
        if (_currentIx < _initials.Length)
            _currentChar = _initials[_currentIx];
        _asciiIx = 0;
        StartBlink();
    }

    public void StartBlink()
    {
        Debug.Log(_blinkCoroutine == null);
        if (_blinkCoroutine == null)
            _blinkCoroutine = StartCoroutine(Blink());
    }

    public void StopBlink()
    {
        if (_blinkCoroutine != null)
        {
            _initials[_currentIx] = _currentChar;
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }
    }

    public bool IsEntered() => _currentIx >= _initials.Length;
    void DisplayInitials() => initialsText.text = $"<mspace={monospaceAmount}em>{new string(_initials)}</mspace>";
    public string GetInitials() => new string(_initials);
}
