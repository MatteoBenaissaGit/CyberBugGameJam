using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTurnAnimation : MonoBehaviour
{
    // State
    // True == Face
    // False == Back
    private bool _cardIsFace = true;
    public bool CardIsTurning = false;
    [SerializeField] private Transform Face;
    [SerializeField] private Transform Back;

    // Fonction for the turn around animation
    void TurnAround() 
    {   
        CardIsTurning = true;
        switch(_cardIsFace) {
            case true:

            break;
            case false:

            break;
        }
        
        
    }
}
