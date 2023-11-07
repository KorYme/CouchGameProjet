using Rewired;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class QTEListSequences
{
    List<QTESequence> _sequences;
    bool[] _inputsSucceeded;
    
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
    public void SetUpList()
    {
        _inputsSucceeded = new bool[TotalLengthInputs];
    }

    public void SetInputSucceeded(int index, bool isSucceeded)
    {
        _inputsSucceeded[index] = isSucceeded;
    }
    public int Length
    {
        get { return _sequences.Count; }
    }
    public QTEListSequences()
    {
        _sequences = new List<QTESequence>();
    }

    public void AddSequence(QTESequence sequence)
    {
        _sequences.Add(sequence);
    }

    public QTESequence GetSequence(int index)
    {
        return _sequences[index];
    }
    public void Clear()
    {
        _sequences.Clear();
    }

    public string ToString(int currentIndexSequence,int currentIndex)
    {
        QTESequence sequence;
        UnitInput input;
        StringBuilder str = new StringBuilder();
        int index = 0;
        if (_inputsSucceeded != null)
        {
            for (int i = 0; i < _sequences.Count; i++)
            {
                sequence = _sequences[i];
                for (int j = 0; j < sequence.ListSubHandlers.Count; j++)
                {
                    input = sequence.ListSubHandlers[j];

                    InputAction action = ReInput.mapping.GetAction(input.ActionIndex);
                    if (action != null)
                    {
                        if (_inputsSucceeded[index])
                        {
                            Debug.Log($"{index} GOOD");
                            str.Append("<color=\"green\">");
                        }
                        else if (currentIndex == j && currentIndexSequence == i
                            && sequence.SequenceType == InputsSequence.SEQUENCE)
                        {
                            str.Append("<color=\"orange\">");
                        }
                        else
                        {
                            Debug.Log($"{index} WRONG");
                            str.Append("<color=\"red\">");
                        }
                        str.Append(action.descriptiveName);
                        str.Append("</color> ");
                    }
                    else
                    {
                        str.Append("(Not found) ");
                    }
                    if (sequence.SequenceType == InputsSequence.SIMULTANEOUS && input.Index != sequence.ListSubHandlers.Count - 1 && sequence.Index != _sequences.Count - 1)
                    {
                        str.Append("+ ");
                    }
                    ++index;
                }
            }
        }
        return str.ToString();
    }
    public IEnumerable<UnitInput> Iterator()
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
