using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardSelectAnimation : MonoBehaviour

{
    // Show and hide Description Variables
    public GameObject DetailedCard;
    public float CardDetailedOffsetX = 0;
    public float DetailedCardPositionY = 0;
    public bool IsSelected = true;
    private float basePositionX;

    // Move card Variables
    [SerializeField] private Transform _movingCard;
    public Transform TargetSpot;
    [SerializeField] private float _moveTime; 
    
    //move animation
    const float moveRangeY = .2f;
    const float moveSpeed = .2f;
    
    //references
    [Header("References")]
    public GameManager GameManager;

    void Start() {
        DetailedCard.SetActive(false);
        basePositionX = transform.position.x;
    }

    public void CardSelect() {
        if (IsSelected) return;
        IsSelected = true;
        
        //detailed card offset
        var position = DetailedCard.transform.position;
        DetailedCard.transform.position = new Vector3(basePositionX + CardDetailedOffsetX, DetailedCardPositionY, position.z);
        
        //detailed card animation
        DetailedCard.transform.DOComplete();
        Vector3 baseScale = DetailedCard.transform.localScale;
        const float animationSpeed = .2f;
        DetailedCard.transform.localScale = Vector3.zero;
        DetailedCard.transform.DOScale(baseScale, animationSpeed);
        
        //move card
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

        if (GameManager != null)
            GameManager.TempGaugeReset();
    }

    public void CardMove() {

        _movingCard.DOMove(new Vector3(TargetSpot.localPosition.x,TargetSpot.localPosition.y,0),_moveTime);

    }


}
