using UnityEngine;
using UnityEngine.UI;

public class UIQte : MonoBehaviour
{
    [SerializeField] protected Image[] _imagesInput;

    public void ChangeSprites(Sprite[] newSprites)
    {
        ResetDisplay();
        if (newSprites == null)
        {
            Debug.LogWarning("List of sprites null");
            return;
        }
        int countSprites = newSprites.Length;
        for (int i = 0; i < _imagesInput.Length; i++)
        {
            _imagesInput[i].sprite = i < countSprites ? newSprites[i] : null;
        }
        ModifyDisplay();
    }

    protected virtual void ResetDisplay() {}
    protected virtual void ModifyDisplay() { }
}
