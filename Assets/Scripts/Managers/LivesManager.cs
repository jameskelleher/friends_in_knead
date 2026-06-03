using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[ExecuteAlways]
public class LivesManager : MonoBehaviour
{
    public GameObject heartPrefab;
    public int livesCount = 5;
    [Range(0f, 1f)]
    public float yOffset;
    [Range(1f, 2f)]
    public float heartbeatScale = 1f;

    List<GameObject> _hearts = new();
    bool _heartbeatToggle = false;
    Vector3 _baseScale;

    void Start()
    {
        if (Application.isPlaying)
        {
            _baseScale = heartPrefab.transform.localScale;

            // clear any children generated in the editor preview
            for (int i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);

            RepositionDuringRuntime();
        }
    }

    public bool LoseLife()
    {
        livesCount--;
        RepositionDuringRuntime();

        return livesCount == 0;
    }

    public void ToggleHeartbeat()
    {
        _heartbeatToggle = !_heartbeatToggle;

        foreach (var h in _hearts)
        {
            if (_heartbeatToggle)
                h.transform.localScale = _baseScale * heartbeatScale;
            else
                h.transform.localScale = _baseScale;
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        EditorApplication.delayCall += () =>
        {
            if (this == null) return;
            if (Application.isPlaying)
                RepositionDuringRuntime();
            else
                RepositionInEditor();
        };
    }
    
    void RepositionInEditor()
    {
        if (this == null) return;

        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        for (int i = 0; i < livesCount; i++)
        {
            var heart = (GameObject)PrefabUtility.InstantiatePrefab(heartPrefab, transform);
            heart.transform.localPosition = new Vector3(0, i * yOffset, 0);
        }
    }
#endif

    void RepositionDuringRuntime()
    {
        foreach (var h in _hearts)
        {
            Destroy(h);
        }
        _hearts.Clear();

        for (int i = 0; i < livesCount; i++)
        {
            var heart = Instantiate(heartPrefab, transform);
            heart.transform.localPosition = new Vector3(0, i * yOffset, 0);
            _hearts.Add(heart);
        }
    }
}
