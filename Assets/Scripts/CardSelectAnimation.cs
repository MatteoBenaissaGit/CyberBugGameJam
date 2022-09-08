using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardSelectAnimation : MonoBehaviour

{
    // Show and hide Description Variables
    [SerializeField] private GameObject _detailsCard;
    public bool IsSelected = true;

    // Move card Variables
    [SerializeField] private Transform _movingCard;
    public Transform TargetCard;
    [SerializeField] private float MoveTime; 
    

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

    public void CardMove() {

        _movingCard.DOMove(new Vector3(TargetCard.localPosition.x,TargetCard.localPosition.y,0),MoveTime);

    }


}
