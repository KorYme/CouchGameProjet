using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Drink
{
    PinaColada = 0,
    Martini = 1,
    JSP = 2
}
public class DrinkList : MonoBehaviour
{
    int[][] _drinksRecipe;
    [SerializeField] TextMeshProUGUI[] _texts;
    [SerializeField] PlayerInput _playerInput;
    public int[][] DrinksRecipe { get => _drinksRecipe;}

    public void Awake()
    {
        _drinksRecipe = new int[3][];
    }
    
    private void Start()
    {
        SetUpDrinks();
        DisplayAllInputs();
    }

    void SetUpDrinks()
    {
        for (int i = 0; i < 3; i++)
        {
            _drinksRecipe[i] = new int[4];
            for (int j = 0; j < 4; j++)
            {
                int index = Random.Range(0, _playerInput.actions.ToArray()[0].controls.Count);
                _drinksRecipe[i][j] = index;
            }
        }
    }

    public void DisplayAllInputs()
    {
        for (int j = 0;j < 3; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                _texts[(j * 4) + i].text = _playerInput.actions.ToArray()[0].controls[_drinksRecipe[j][i]].name;
            }
        }
    }

}
