using UnityEngine;
using UnityEngine.UI;

public class UIQte : MonoBehaviour
{
    [SerializeField] protected Image[] _imagesInput;

    public virtual void ChangeSprites(Sprite[] newSprites)
    {
        ResetDisplay();
        int countSprites = newSprites.Length;
        for (int i = 0; i < _imagesInput.Length; i++)
        {
            _imagesInput[i].sprite = i < countSprites ? newSprites[i] : null;
            if (i >= countSprites)
                _imagesInput[i].color = Color.clear;
        }
    }

    protected virtual void ResetDisplay() {}
}
