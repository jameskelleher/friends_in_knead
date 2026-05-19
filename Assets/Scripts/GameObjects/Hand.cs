using UnityEngine;

public class Hand : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite rest;
    public Sprite soft;
    public Sprite hard;
    public Sprite success;
    public Sprite miss;

    [HideInInspector]
    public InputStrength inputStrength;
    [HideInInspector]
    public bool needsToReset = false;

    SpriteRenderer _sprite;
    Result _result = Result.Ready;
    float _defaultX;
    float _offsetX;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _defaultX = transform.position.x;
    }

    public void UpdatePos(float inputNormalized, ScreenYPosition yPosition, InputStrengthPartitions partitions)
    {
        if (!gameObject.activeInHierarchy) return;
        
        Vector3 pos = transform.position;

        float height = Camera.main.orthographicSize;
        float minY = Mathf.Lerp(-height, height, yPosition.low);
        float maxY = Mathf.Lerp(-height, height, yPosition.high);
        float newY = Mathf.Lerp(minY, maxY, inputNormalized);
        float newX = _result == Result.Ready ? _defaultX : _defaultX + _offsetX;

        transform.position = new Vector3(newX, newY, pos.z);

        if (inputNormalized < partitions.low)
        {
            _sprite.sprite = rest;
            inputStrength = InputStrength.Rest;
            needsToReset = false;
        }
        else if (inputNormalized < partitions.medium)
        {
            _sprite.sprite = soft;
            inputStrength = InputStrength.Soft;
        }
        else
        {
            _sprite.sprite = hard;
            inputStrength = InputStrength.Hard;
        }

        if (_result == Result.Success)
            _sprite.sprite = success;
        else if (_result == Result.Miss)
            _sprite.sprite = miss;
    }

    public void SetResult(Result newResult, float resultXOffset)
    {
        _result = newResult;
        _offsetX = resultXOffset;
    }

    public bool IsResting => inputStrength == InputStrength.Rest;
}