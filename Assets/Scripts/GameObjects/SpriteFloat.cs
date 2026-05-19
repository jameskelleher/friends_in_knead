using UnityEngine;

public class SpriteFloat : MonoBehaviour
{
    public float floatRange = 1f;
    public float floatSpeed = 1f;

    Vector3 _defaultLocalPos;

    void Start()
    {
     _defaultLocalPos = transform.localPosition;   
    }

    void Update()
    {
        float sineY = Mathf.Sin(Time.time * floatSpeed) * floatRange;
        Vector3 offset = new Vector3(0f, sineY, 0f);
        transform.localPosition = _defaultLocalPos + offset;
    }
}
