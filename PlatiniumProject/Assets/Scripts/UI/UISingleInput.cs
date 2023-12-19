using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UISingleInput : MonoBehaviour
{
    [SerializeField] Image _image;
    [SerializeField] InputDisplayed _input;
    [SerializeField] PlayerRole _role;
    IEnumerator Start()
    {
        yield return new WaitUntil(() => PlayerInputsAssigner.GetDeviceByRole(_role) != InputDevice.None);
        _image.sprite = Globals.DatabaseActionSprites.GetInput(_input, PlayerInputsAssigner.GetDeviceByRole(_role));
    }
}
