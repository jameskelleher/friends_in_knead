using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[InitializeOnLoad]
#endif

public class JoystickRemapProcessor : InputProcessor<float>
{
#if UNITY_EDITOR
    static JoystickRemapProcessor()
    {
        Initialize();
    }
#endif

    [RuntimeInitializeOnLoadMethod]
    static void Initialize()
    {
        InputSystem.RegisterProcessor<JoystickRemapProcessor>();
    }

    public override float Process(float value, InputControl control)
    {
        return (value + 1f) / 2f; // -1..1 → 0..1
    }
}