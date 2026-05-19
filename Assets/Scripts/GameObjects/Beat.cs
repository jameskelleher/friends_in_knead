using System;
using UnityEngine;

public class Beat : MonoBehaviour
{
    public BeatInfo beatInfo;
    public BeatSpriteLibrary beatSpriteLibrary;
    [HideInInspector]
    public bool isActive;
    [Header("Callout Params")]
    [Range(0f, 0.1f)]
    public float calloutExtraHeight = 0.02f;
    [Range(1f, 2f)]
    public float calloutRotation = 15f;

    BeatRotation _currentRotation = BeatRotation.None;
    Coroutine _currentAnimation;
    SpriteRenderer _sprite;
    Vector3 _defaultLocalPos;
    Vector3 _calloutOffset;
    bool _initialized = false;
    GameObject _shadow;

    void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        _shadow = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if (!_initialized)
        {
            _initialized = true;
            return;
        }

        transform.localPosition = _defaultLocalPos;

        if (isActive)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.localPosition += _calloutOffset;
            if (!IsRest)
                _shadow.SetActive(true);
            return;
        }
        else
            _shadow.SetActive(false);


        switch (_currentRotation)
        {
            case BeatRotation.None:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case BeatRotation.Left:
                transform.rotation = Quaternion.Euler(0, 0, -calloutRotation);
                break;
            case BeatRotation.Right:
                transform.rotation = Quaternion.Euler(0, 0, calloutRotation);
                break;
            default:
                throw new ArgumentOutOfRangeException($"unexpected rotation type: {_currentRotation}");
        }
    }

    public void Init(InputStep targetInput)
    {
        beatInfo = BeatInfo.FromStep(targetInput, Result.Ready);

        if (IsRest)
            calloutExtraHeight *= 4;

        _defaultLocalPos = transform.localPosition;
        _calloutOffset = new Vector3(0f, calloutExtraHeight, -0.1f);


        _sprite = GetComponent<SpriteRenderer>();
        _sprite.enabled = true;
        Sprite beatSprite = beatSpriteLibrary.FindSprite(beatInfo);
        _sprite.sprite = beatSprite;
        _shadow.GetComponent<SpriteRenderer>().sprite = beatSprite;

        if (IsSoft)
            _shadow.transform.localPosition *= 0.75f;
    }

    public void SetResult(Result result)
    {
        _sprite.enabled = true;
        beatInfo.result = result;
        _sprite.sprite = beatSpriteLibrary.FindSprite(beatInfo);

        if (_currentAnimation != null)
            StopCoroutine(_currentAnimation);
    }

    public bool SuccessDetected(Hand left, Hand right)
    {
        // rests are only correct if waited out
        if (beatInfo.inputStrength == InputStrength.Rest)
            return false;

        // because we already check InvalidInput, we can set unchecked input to true
        bool leftCheck = beatInfo.position == Position.Right ? true : CheckHand(left);
        bool rightCheck = beatInfo.position == Position.Left ? true : CheckHand(right);

        if (!leftCheck || !rightCheck)
            return false;

        if (beatInfo.position == Position.Left || beatInfo.position == Position.Both)
            left.needsToReset = true;
        else if (beatInfo.position == Position.Left || beatInfo.position == Position.Both)
            right.needsToReset = true;

        return true;
    }

    bool CheckHand(Hand hand)
    {
        if (hand.needsToReset || hand.inputStrength != beatInfo.inputStrength)
            return false;

        return true;
    }

    public void RhythmRotate(bool reset = false)
    {
        if (reset)
            _currentRotation = BeatRotation.None;
        else if (_currentRotation == BeatRotation.Left)
            _currentRotation = BeatRotation.Right;
        else
            _currentRotation = BeatRotation.Left;
    }

    public bool InvalidInput(Hand left, Hand right)
    {
        bool inputOnRest = beatInfo.inputStrength == InputStrength.Rest && (
            left.inputStrength != InputStrength.Rest || right.inputStrength != InputStrength.Rest);
        bool leftShouldRest = beatInfo.position == Position.Right && left.inputStrength != InputStrength.Rest;
        bool rightShouldRest = beatInfo.position == Position.Left && right.inputStrength != InputStrength.Rest;
        bool leftOvershoot = left.inputStrength > beatInfo.inputStrength;
        bool rightOvershoot = right.inputStrength > beatInfo.inputStrength;

        if (inputOnRest || leftShouldRest || rightShouldRest || leftOvershoot || rightOvershoot)
            return true;

        return false;
    }

    public bool IsRest => beatInfo.inputStrength == InputStrength.Rest;
    public bool IsSoft => beatInfo.inputStrength == InputStrength.Soft;
    public bool IsHard => beatInfo.inputStrength == InputStrength.Hard;

    public void UpdateDefaultPos(Vector3 pos) => _defaultLocalPos = pos;
}
