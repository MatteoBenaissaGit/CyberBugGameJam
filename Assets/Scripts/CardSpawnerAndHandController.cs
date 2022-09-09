using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardSpawnerAndHandController : MonoBehaviour
{
    [Header("Card")]
    public GameObject cardPrefabGameObject;
    [Range(0,7)] public int numberoOfCardInHand;
    [SerializeField] [Range(0,7)] private int _currentNumberOfCardInHand;
    public List<CardData> AvailableCardList;

    [Header("Card placement")]
    [SerializeField] private int _offsetX = -1;
    [SerializeField] private float _offsetY = 0f;
    [SerializeField] private int _offsetZ = 0;

    //game manager
    private GameManager _gameManager;
    
    //card data
    private CardData _randomCardData;
    private CardData _spawningCardData;
    
    //coordinates
    private int _coordinateX;
    private float _coordinateY =-3f;
    private int _coordinateZ = 0;
    private float _rotationZ = 0f;
    private float _middleCardDetailedCardPositionY = 0;
    private float _middleCardDetailedCardOffsetPositionY = 4f;

    private List<Card> _inHandCardList = new List<Card>();
    private List<Card> _cardOnSlots = new List<Card>();

    private void Start()
    {
        _gameManager = gameObject.GetComponent<GameManager>();
    }

    public void SetupCardsForNewCharacter()
    {
        SetupAvailableCardList();
        CardSetup();
    }
    
    private void SetupAvailableCardList()
    {
        _currentNumberOfCardInHand = numberoOfCardInHand;
        foreach (var cardData in _gameManager.CardDataList) {
            AvailableCardList.Add(cardData);
        }
        if (_currentNumberOfCardInHand > AvailableCardList.Count) _currentNumberOfCardInHand = AvailableCardList.Count;
    }

    private void CardSetup()
    {
        var animationSpeed = .5f;
        var startOffsetY = -1.5f;
        
        for(var i = 0; i < _currentNumberOfCardInHand; i++)
        {
            //dataCard choice -> block cardData being chosen twice by only allowing cardData choice that haven't been chosen
            _spawningCardData = CardDataChoice();
            while (!AvailableCardList.Contains(_spawningCardData)) {
                _spawningCardData = CardDataChoice();
            }
            AvailableCardList.Remove(_spawningCardData);

            //card instantiate
            var endPosition = CardEndPosition(i);
            GameObject cardPrefab = Instantiate(cardPrefabGameObject, endPosition, Quaternion.identity);
            var cardComponent = cardPrefab.GetComponent<Card>();
            _inHandCardList.Add(cardComponent);
            //detailed card position&rotation setup
            var cardSelectAnimationComponent = cardPrefab.GetComponent<CardAnimation>();
            var cardOffsetX = cardPrefab.transform.position.x / 3;
            cardSelectAnimationComponent.CardDetailedOffsetX = endPosition.w == 0 ? 0 : endPosition.w < 0 ? -cardOffsetX : cardOffsetX;
            cardSelectAnimationComponent.DetailedCardPositionY = _middleCardDetailedCardPositionY;
            cardPrefab.transform.Rotate(new Vector3(0, 0, _rotationZ));
            
            //value association
            cardPrefab.GetComponent<Card>().ArtValue = _randomCardData.ArtValue;
            cardPrefab.GetComponent<Card>().DesignValue = _randomCardData.DesignValue;
            cardPrefab.GetComponent<Card>().ProgrammerValue = _randomCardData.ProgrammingValue;
            
            //animation card 
            var position = cardPrefab.transform.position;
            Vector3 startPosition = new Vector3(position.x, position.y + startOffsetY, position.z);
            position = startPosition;
            cardPrefab.transform.position = position;
            cardPrefab.transform.DOComplete();
            cardPrefab.transform.DOMove(new Vector3(_coordinateX, _coordinateY, _coordinateZ), animationSpeed);

            //value changes to animate
            startOffsetY -= 2f;
            animationSpeed += .3f;
            
            //associate data with card description text&image
            cardComponent.CardSetup(_spawningCardData);
        }
    }

    private CardData CardDataChoice()
    {
        var cardDataIndex = Random.Range(0, AvailableCardList.Count);
        _randomCardData = AvailableCardList[cardDataIndex];
        return (_randomCardData);
    }

    private Vector4 CardEndPosition(int i)
    {
        bool isCardOnLeft = true;
        bool isMiddleCard = false;
        
        if (i % 2 == 1)
        {
            _coordinateX = (i / 2) + 1 + _offsetX;
            _rotationZ = ((i / 2) + 1)*(-10);
            if(i == 1){
                _coordinateY=-3.14f + _offsetY;
            }
            else if(i == 3){
                _coordinateY=-3.46f + _offsetY;
            }
            else if(i == 5){
                _coordinateY=-4f + _offsetY;
            }
            _coordinateZ+=1 + _offsetZ;
            isCardOnLeft = false;
        }
        if(i%2 == 0)
        {
            _coordinateX = i / (-2) + _offsetX;
            _rotationZ = i / (-2)*(-10);
        }
        if (i == 0)
        {
            _coordinateX = i + _offsetX;
            _coordinateY = -3f + _offsetY;
            _middleCardDetailedCardPositionY = _coordinateY + _middleCardDetailedCardOffsetPositionY;
            isMiddleCard = true;
        }
            
        return new Vector4(_coordinateX, _coordinateY, _coordinateZ, isMiddleCard? 0 : isCardOnLeft ?-1:1);
        //the w in the Vector4 allows to know either the card is middle, left of right placed in the hand
    }

    private void CardRepositioning()
    {
        for (int i = 0; i < _inHandCardList.Count; i++)
        {
            const float moveAnimationSpeed = .2f;
            var newEndPosition = CardEndPosition(i);
            _inHandCardList[i].transform.DOMove(newEndPosition, moveAnimationSpeed);
        }
    }
    
    public void TakeOffCard(Card cardToTakeOff, Vector3 cardSpotPosition)
    {
        //take off and move card
        CardAnimation spawnerCardComponent = cardToTakeOff.gameObject.GetComponent<CardAnimation>();
        spawnerCardComponent.MoveCardToTargetPosition(cardSpotPosition);
        cardToTakeOff.isPlaced = true;
        
        //transform
        cardToTakeOff.transform.rotation = Quaternion.Euler(Vector3.zero);

        _inHandCardList.Remove(cardToTakeOff);
        _cardOnSlots.Add(cardToTakeOff);

        _currentNumberOfCardInHand--;

        CardRepositioning();
    }

    public void RemoveCurrentHandCardAndSpawnNewHand()
    {
        //cards
        foreach (var card in _inHandCardList)
        {
            card.CardAnimationComponent.CardExit();
        }
        foreach (var card in _cardOnSlots)
        {
            card.CardAnimationComponent.CardExit();
        }
        _cardOnSlots.Clear();
        _inHandCardList.Clear();
        AvailableCardList.Clear();
        
        //spots
        foreach (var cardSpotController in _gameManager.CardSpotControllerList)
        {
            cardSpotController.isTaken = false;
        }
    }
    
}
