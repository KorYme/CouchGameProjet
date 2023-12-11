using Rewired;
using System.Collections.Generic;
using System.Text;

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

    public void SetInputSucceeded(int index, QTE_STATE qteState)
    {
        _inputsSucceeded[index] = qteState == QTE_STATE.IS_PRESSED;
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
                    str.Append("<sprite name=\"");
                    if (action != null)
                    {
                        str.Append(GetInputStringFromActionType(action, i, j));
                        str.Append("\" ");
                        if (_inputsSucceeded[index])
                        {
                            str.Append("color=#008600");
                        }
                        else if (currentIndex == j && currentIndexSequence == i
                            && sequence.SequenceType == InputsSequence.SEQUENCE)
                        {
                            str.Append("color=#FFA500");
                        }
                    }
                    else
                    {
                        str.Append("(Not found) ");
                    }
                    str.Append(">");
                    if (sequence.SequenceType == InputsSequence.SIMULTANEOUS && input.Index != sequence.ListSubHandlers.Count - 1 && sequence.Index != _sequences.Count - 1)
                    {
                        str.Append("+");
                    }
                    ++index;
                }
            }
        }
        return str.ToString();
    }
    public string GetInputString(int indexOfSequence,int indexInSequence)
    {
        StringBuilder str = new StringBuilder();
        if (indexOfSequence >= 0 && indexOfSequence < _sequences.Count &&
            indexInSequence >= 0 && indexInSequence < _sequences[indexOfSequence].ListSubHandlers.Count)
        {
            str.Append("<sprite name=\"");
            InputAction action = ReInput.mapping.GetAction(_sequences[indexOfSequence].ListSubHandlers[indexInSequence].ActionIndex);
            str.Append(GetInputStringFromActionType(action, indexOfSequence, indexInSequence));
            str.Append("\">");
        }
        return str.ToString();
    }

    private string GetInputStringFromActionType(InputAction action, int indexOfSequence, int indexInSequence)
    {
        if (action.type == InputActionType.Button)
        {
            return action.descriptiveName; 
        }
        // Only Axis type
        if (_sequences[indexOfSequence].Status == InputStatus.LONG && _sequences[indexOfSequence].LongInputType == LongInputType.SHAKE)
        {
            return "RS";
        }
        //Only Short input or hold (direction)
        if (_sequences[indexOfSequence].ListSubHandlers[indexInSequence].PositiveValue)
        {
            return action.positiveDescriptiveName;
        }
        else
        {
            return action.negativeDescriptiveName;
        }
    }
}
