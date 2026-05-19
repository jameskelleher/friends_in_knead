using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class TriggerRelay : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent<Collider2D> onTriggerEnter;
    [HideInInspector]
    public UnityEvent<Collider2D> onTriggerStay;
    [HideInInspector]
    public UnityEvent<Collider2D> onTriggerExit;
    [HideInInspector]
    public float floatHeight;
    [HideInInspector]
    public float floatSpeed;

    Vector3 _defaultLocalPos;

    void OnTriggerEnter2D(Collider2D collision) => onTriggerEnter.Invoke(collision);
    void OnTriggerStay2D(Collider2D collision) => onTriggerStay.Invoke(collision);
    void OnTriggerExit2D(Collider2D collision) => onTriggerExit.Invoke(collision);

    void Start()
    {
        _defaultLocalPos = transform.localPosition;
    }

    void Update()
    {
        float yOff = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        Vector3 offset = new Vector3(0f, yOff, 0f);
        transform.localPosition = _defaultLocalPos + offset;
    }
}
