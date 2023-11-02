using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEListSequences : MonoBehaviour
{
    List<QTESequence> _sequences = new List<QTESequence>();
    public void AddSequence(QTESequence sequence)
    {
        _sequences.Add(sequence);
    }

    public int TotalLengthInputs
    {
        get
        {
            int totalLength = 0;
            foreach (QTESequence sequence in _sequences)
            {
                totalLength += sequence.ListSubHandlers.Count;
            }
            return totalLength;
        }
    }
    public void Clear(QTESequence sequence)
    {
        _sequences.Clear();
    }
    public IEnumerable<UnitInput> GetCurrentSequence()
    {
        foreach(QTESequence sequence in _sequences)
        {
            foreach (UnitInput input in sequence.ListSubHandlers)
            {
                // Returning the element after every iteration
                yield return input;
            }
        }
    }
}
