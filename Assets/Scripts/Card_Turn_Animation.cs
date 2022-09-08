using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Card_Turn_Animation : MonoBehaviour
{
    // State
    // True == Face
    // False == Back
    private bool _cardIsFace = true;
    public bool CardIsTurning = false;
    [SerializeField] private Transform _face;
    [SerializeField] private Transform _back;

    void Start() {
        TurnAround();

    }

    // Fonction for the turn around animation
    void TurnAround() 
    {   
        CardIsTurning = true;
        switch(_cardIsFace) {
            case true:
                _face.DOScale(new Vector3(0,_face.localScale.y,_face.localScale.z),1);
                _back.DOScale(new Vector3(1,_face.localScale.y,_face.localScale.z),1);
            break;
            case false:
                _back.DOScale(new Vector3(0,_face.localScale.y,_face.localScale.z),1);
                _face.DOScale(new Vector3(1,_face.localScale.y,_face.localScale.z),1);
            break;
        }
        
        
    }
}
