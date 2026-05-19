using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputConfig", menuName = "Scriptable Objects/InputConfig")]
public class InputConfig : ScriptableObject
{
    [Header("Inputs")]
    public InputAction leftInput;
    public InputAction rightInput;
    public InputType currentInput;
    public List<InputTypeConfig> inputConfigs = new();

    Dictionary<InputType, InputTypeConfig> _inputDictionary = new();

    void OnEnable()
    {
        leftInput.Enable();
        rightInput.Enable();

        inputConfigs.ForEach(config => _inputDictionary[config.inputType] = config);
    }

    void OnValidate()
    {
        inputConfigs.ForEach(config => _inputDictionary[config.inputType] = config);
    }

    public InputPair ReadInput()
    {
        InputTypeConfig config = _inputDictionary[currentInput];

        InputPair raw = ReadRawInput(config);
        InputPair normalized = RemapInput(config, raw);

        // Debug.Log($"RAW - L: {raw.left}, R: {raw.right}");
        // Debug.Log($"NORM - L: {normalized.left:F2}, R: {normalized.right:F2}");

        return normalized;
    }

    InputPair ReadRawInput(InputTypeConfig config)
    {
        float inputLeft, inputRight;

        if (config.swapInputPositions)
        {
            inputRight = leftInput.ReadValue<float>();
            inputLeft = rightInput.ReadValue<float>();
        }
        else
        {
            inputLeft = leftInput.ReadValue<float>();
            inputRight = rightInput.ReadValue<float>();
        }

        return new InputPair(inputLeft, inputRight);
    }

    InputPair RemapInput(InputTypeConfig config, InputPair input)
    {
        if (config.bypassRemapping)
            return input;

        InputMap leftMap = config.swapInputMaps ? config.inputMap2 : config.inputMap1;
        InputMap rightMap = config.swapInputMaps ? config.inputMap1 : config.inputMap2;

        float remappedLeft = RemapInput(leftMap, input.left);
        float remappedRight = RemapInput(rightMap, input.right);

        return new InputPair(remappedLeft, remappedRight);
    }

    float RemapInput(InputMap map, float input)
    {
        map.accumulator = EMA(map.accumulator, input, map.EMAAmt);
        float clamped = Mathf.Clamp01(Mathf.InverseLerp(map.min, map.max, map.accumulator));
        return clamped;
    }

    // public float HandleEMA(float value)
    float EMA(float prevVal, float newVal, float alpha) => prevVal * alpha + newVal * (1 - alpha);
}


[Serializable]
public class InputMap
{
    [Range(-1f, 1f)] public float min;
    [Range(-1f, 1f)] public float max;
    [Range(0f, 1f)] public float EMAAmt;
    [HideInInspector] public float accumulator;

}

public enum InputType { Prototype, Gamepad, Cabinet }

[Serializable]
public struct InputTypeConfig
{
    public InputType inputType;
    public InputMap inputMap1;
    public InputMap inputMap2;
    public bool swapInputPositions;
    public bool swapInputMaps;
    public bool bypassRemapping;

}