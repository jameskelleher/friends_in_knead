using System;
using UnityEngine;

public class Dough : MonoBehaviour
{
    public GameObject finalResult;
    [Range(0f, 0.1f)]
    public float bounceAmount = 0.05f;
    public Sprite[] sprites = new Sprite[0];

    int _spriteIx;
    SpriteRenderer _doughSprite;
    SpriteRenderer _resultSprite;
    Vector3 _defaultPos;
    bool _doResultAnimation;
    float _animationStartTime;
    double _secondsPerBeat;

    void Start()
    {
        _doughSprite = GetComponent<SpriteRenderer>();
        _doughSprite.sprite = sprites[_spriteIx];

        _resultSprite = finalResult.GetComponent<SpriteRenderer>();
        finalResult.SetActive(false);

        _defaultPos = transform.position;
    }

    void Update()
    {
        Vector3 deltaPos = Vector3.zero;

        if (_doResultAnimation)
        {
            float theta = (Time.time - _animationStartTime) / (float)_secondsPerBeat;
            float deltaY = bounceAmount * Mathf.Sin(2 * Mathf.PI * theta);
            deltaPos = new Vector3(0f, deltaY);
        }

        transform.position = _defaultPos + deltaPos;
    }

    public void LoopDoughSprites(bool reset = false)
    {
        _doResultAnimation = false;
        _doughSprite.enabled = true;
        
        finalResult.SetActive(false);

        if (sprites.Length == 0) return;

        _spriteIx = reset ? 0 : (_spriteIx + 1) % sprites.Length;
        _doughSprite.sprite = sprites[_spriteIx];
    }

    public void SetResultSprite(Sprite sprite, double secondsPerBeat)
    {
        _doResultAnimation = true;
        _animationStartTime = Time.time;

        _secondsPerBeat = secondsPerBeat;

        _doughSprite.enabled = false;

        finalResult.SetActive(true);
        _resultSprite.sprite = sprite;
    }
}
