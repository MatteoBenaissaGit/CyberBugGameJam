using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CardAnimation))]
public class Card : MonoBehaviour
{
    [Header("Card")]
    public SpriteRenderer CardSpriteRenderer;
    public TextMeshProUGUI CardNameTextMesh;
    public TextMeshProUGUI CardDescriptionTextMesh;
    public bool isPlaced = false;
    public CardAnimation CardAnimationComponent;
    
    [Header("Values")] [Space(10)]
    public int DesignValue;
    public int ArtValue;
    public int ProgrammerValue;

    private void Start()
    {
        CardAnimationComponent = gameObject.GetComponent<CardAnimation>();
    }

    public void CardSetup(CardData cardData)
    {
        if (cardData.cardImageSprite != null) CardSpriteRenderer.sprite = cardData.cardImageSprite;
        CardNameTextMesh.text = cardData.CardName;
        CardDescriptionTextMesh.text = cardData.CardDescription;
    }
}
