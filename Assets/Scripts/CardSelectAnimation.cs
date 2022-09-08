using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelectAnimation : MonoBehaviour

{
    [SerializeField] private GameObject _detailsCard;
    public bool IsSelected = true;

    void Start() {
        _detailsCard.SetActive(false);
    }

    public void CardSelect() {
        IsSelected = true;
        _detailsCard.SetActive(true);
    }

    public void CardDeselect() {
        IsSelected = false;
        _detailsCard.SetActive(false);
    }
}
