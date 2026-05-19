using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SequenceLibrary", menuName = "Scriptable Objects/SequenceLibrary")]
public class SequenceLibrary : ScriptableObject
{
    public List<InputSequence> sequences;

    void OnValidate()
    {
        for (int i = 0; i < sequences.Count; i++)
        {
            var seq = sequences[i];

            if (seq.steps == null)
            {
                seq.steps = new InputStep[8];
                sequences[i] = seq;
            }
            else if (seq.steps.Length != 8)
            {
                var newSteps = new InputStep[8];
                int copyCount = Mathf.Min(seq.steps.Length, 8);
                System.Array.Copy(seq.steps, newSteps, copyCount);
                seq.steps = newSteps;
                sequences[i] = seq;
            }
        }
    }
}
