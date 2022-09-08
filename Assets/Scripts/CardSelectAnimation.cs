using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardSelectAnimation : MonoBehaviour

{
    // Show and hide Description Variables
    public GameObject DetailedCard;
    public bool IsSelected = true;

    // Move card Variables
    [SerializeField] private Transform _movingCard;
    public Transform TargetCard;
    [SerializeField] private float MoveTime; 
    
    const float moveRangeY = .2f;
    const float moveSpeed = .2f;

    void Start() {
        DetailedCard.SetActive(false);
    }

    public void CardSelect() {
        if (IsSelected) return;
    
        IsSelected = true;
        
        transform.DOComplete();
        Vector3 movePositionSelect =
            new Vector3(transform.position.x, transform.position.y + moveRangeY, transform.position.z);
        transform.DOMove(movePositionSelect, moveSpeed);
        
        DetailedCard.SetActive(true);
    }

    private void OnMouseExit()
    {
        CardDeselect();
        
    }

    private void CardDeselect() 
    {
        IsSelected = false;
        
        transform.DOComplete();
        Vector3 movePositionSelect =
            new Vector3(transform.position.x, transform.position.y - moveRangeY, transform.position.z);
        transform.DOMove(movePositionSelect, moveSpeed);
        
        DetailedCard.SetActive(false);
    }

    public void CardMove() {

        _movingCard.DOMove(new Vector3(TargetCard.localPosition.x,TargetCard.localPosition.y,0),MoveTime);

    }


}
