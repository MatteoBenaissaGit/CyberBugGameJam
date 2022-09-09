using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardAnimation : MonoBehaviour

{
    // Show and hide Description Variables
    [Header("Detailed Card")]
    public GameObject DetailedCard;
    public float CardDetailedOffsetX = 0;
    public float DetailedCardPositionY = 0;
    
    [Header("Variables")]
    public bool IsSelected = true;
    public int OrderInLayerBase = 3;
    private float basePositionX;

    //move animation
    const float moveRangeY = .2f;
    const float moveSpawnSpeed = .2f;
    private float slerpForce = .5f;
    
    //references
    [Header("References")]
    public GameManager GameManager;

    private Card _cardCompenent;

    private void Start() 
    {
        DetailedCard.SetActive(false);
        basePositionX = transform.position.x;
        _cardCompenent = GetComponent<Card>();
    }

    public void MoveCardToTargetPosition(Vector3 targetSpotPosition) 
    {
        const float moveAnimationTime = 1f;
        transform.DOMove(targetSpotPosition, moveAnimationTime);
    }

    public void CardSelect() {
        if (IsSelected || _cardCompenent.isPlaced) return;
        IsSelected = true;
        
        //sprite to front
        var OrderInLayerToFront = OrderInLayerBase + 1;
        _cardCompenent.CardImageSpriteRenderer.sortingOrder = OrderInLayerToFront;
        _cardCompenent.CardOutlineSpriteRenderer.sortingOrder = OrderInLayerToFront;
        
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
        transform.DOMove(movePositionSelect, moveSpawnSpeed);
        
        DetailedCard.SetActive(true);
    }

    private void OnMouseExit()
    {
        CardDeselect();
    }

    public void CardDeselect() 
    {
        IsSelected = false;
        
        //sprite to front
        var OrderInLayerToBack = OrderInLayerBase - 1;
        _cardCompenent.CardImageSpriteRenderer.sortingOrder = OrderInLayerToBack;
        _cardCompenent.CardOutlineSpriteRenderer.sortingOrder = OrderInLayerToBack;
        
        //guard if already placed
        if (_cardCompenent.isPlaced || !GameManager.CanSelectCards) return;
        
        transform.DOComplete();
        Vector3 movePositionSelect =
            new Vector3(transform.position.x, transform.position.y - moveRangeY, transform.position.z);
        transform.DOMove(movePositionSelect, moveSpawnSpeed);
        
        DetailedCard.SetActive(false);

        if (GameManager != null)
            GameManager.TempGaugeReset();
    }

    public void CardExit()
    {
        const float exitMoveY = -6f;
        const float exitMoveSpeed = .5f;
        transform.DOComplete();
        transform.DOMoveY(transform.position.y + exitMoveY, exitMoveSpeed).OnComplete(DestroyCard);
    }

    private void DestroyCard()
    {
        Destroy(gameObject);
    }

}
