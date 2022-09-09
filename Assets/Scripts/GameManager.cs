using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;

[RequireComponent(typeof(CardSpawnerAndHandController))]
public class GameManager : MonoBehaviour
{
    [Header("Parameters")] 
    [SerializeField] private int _numberOfCardsRequiredPerCharacter = 3;
    [SerializeField] private float _fillAmountTime = 1.5f;
    private int _currentNumberOfCardSelected;
    
    [Header("Data list")]
    public List<CardData> CardDataList = new List<CardData>();
    public List<CharacterData> CharacterDataList = new List<CharacterData>();
    
    //role listing (Empty lists that are completed throughout the game when character's roles are decided)
    private List<CharacterData> _charactersDesignerList = new List<CharacterData>();
    private List<CharacterData> _charactersArtistList = new List<CharacterData>();
    private List<CharacterData> _charactersProgrammerList = new List<CharacterData>();

    [Header("TextMesh References")]
    [SerializeField] private TextMeshProUGUI _designerListTextMeshPro;
    [SerializeField] private TextMeshProUGUI _artistListTextMeshPro;
    [SerializeField] private TextMeshProUGUI _programmerListTextMeshPro;

    [Header("Gauges References")]
    [Space(5)] [SerializeField] private Image _designGauge;
    [SerializeField] private Image _designBackGauge;
    [SerializeField] private Image _designTempGauge;
    [Space(5)] [SerializeField] private Image _artGauge;
    [SerializeField] private Image _artBackGauge;
    [SerializeField] private Image _artTempGauge;
    [Space(5)] [SerializeField] private Image _programmingGauge;
    [SerializeField] private Image _programmingBackGauge;
    [SerializeField] private Image _programmingTempGauge;

    [Header("CardSpot References")] 
    public List<CardSpotController> CardSpotControllerList = new List<CardSpotController>();

    [Header("Character References")]
    [SerializeField] private GameObject _characterPrefab;
    [SerializeField] private Transform _characterTransformPosition;
    private GameObject _currentCharacterGameObject;

    [Header("Card button References")] 
    [SerializeField] private Button _cardConfirmButton;
    [SerializeField] private Button _cardCancelButton;

    [Header("Debug")] 
    [SerializeField] private GameState _gameState;
    [SerializeField] private CharacterData _currentCharacter;

    private bool _cardIsCurrentlySelected = false;
    private CardSpawnerAndHandController _cardSpawnerAndHandController;
    public bool CanSelectCards = true;

    private void Start()
    {
        UIGaugesReset();
        UITextReset();
        UIButtonReset();
        ChangeState(GameState.Start);
        
        //references
        _cardSpawnerAndHandController = gameObject.GetComponent<CardSpawnerAndHandController>();
        
        //card button
        _cardConfirmButton.onClick.AddListener(CardConfirm);
        _cardCancelButton.onClick.AddListener(CardCancel);
        _cardConfirmButton.gameObject.SetActive(false);
        _cardCancelButton.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _cardConfirmButton.onClick.RemoveListener(CardConfirm);
        _cardCancelButton.onClick.RemoveListener(CardCancel);
    }

    public void UIGaugesReset()
    {
        const float fillSpeed = .2f;
        //gauges
        //design
        _designGauge.DOFillAmount(0,fillSpeed);
        _designBackGauge.DOFillAmount(0,fillSpeed);
        _designTempGauge.DOFillAmount(0,fillSpeed);
        //art
        _artGauge.DOFillAmount(0,fillSpeed);
        _artBackGauge.DOFillAmount(0,fillSpeed);
        _artTempGauge.DOFillAmount(0,fillSpeed);
        //programming
        _programmingGauge.DOFillAmount(0,fillSpeed);
        _programmingBackGauge.DOFillAmount(0,fillSpeed);
        _programmingTempGauge.DOFillAmount(0,fillSpeed);
    }

    private void UITextReset()
    {
        //text
        _designerListTextMeshPro.text = "";
        _artistListTextMeshPro.text = "";
        _programmerListTextMeshPro.text = "";
    }

    private void UIButtonReset()
    {
        //buttons card
        _cardConfirmButton.gameObject.transform.localScale = Vector3.zero;
        _cardCancelButton.gameObject.transform.localScale = Vector3.zero;
    }

    private void ChangeState(GameState gameState)
    {
        _gameState = gameState;
        
        switch (gameState)
        {
            case GameState.Start:
                GameStart();
                break;
            case GameState.CharacterSpawn:
                CharacterSpawn();
                break;
            case GameState.CardSpawn:
                CardSpawn();
                break;
            case GameState.WaitingForInput:
                break;
            case GameState.CharacterRoleAttribution:
                CharacterRoleAttribution();
                break;
            case GameState.CharacterExit:
                CharacterExit();
                break;
            case GameState.End:
                EndGame();
                break;
        }
    }

    private void Update()
    {
        if (_gameState != GameState.WaitingForInput) return;
            CardSelectionAndActivation();
    }

    private void GameStart()
    {
        //starting animation or text ?

        const float secondsToStartCharacterSpawn = 2f;
        StartCoroutine(StartToFirstCharacterSpawn(secondsToStartCharacterSpawn));
    }

    private IEnumerator StartToFirstCharacterSpawn(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ChangeState(GameState.CharacterSpawn);
    }

    private void CharacterSpawn()
    {
        //get a random character and make it spawn
        if (CharacterDataList.Count>0)
            _currentCharacter = CharacterDataList[0];
        else
        {
            print("no more character");
            //end the game
            ChangeState(GameState.End);
            return;
        }
        
        //setup character
        _currentCharacterGameObject = Instantiate(_characterPrefab, _characterTransformPosition.position, Quaternion.identity);
        CharacterController characterControllerComponent = _currentCharacterGameObject.GetComponent<CharacterController>();
        characterControllerComponent.CharacterNameTextMesh.text = _currentCharacter.CharacterName;
        if (_currentCharacter.CharacterSprite != null)
            characterControllerComponent.CharacterSpriteRenderer.sprite = _currentCharacter.CharacterSprite;
        //character animation
        characterControllerComponent.CharacterSpawningAnimation();

        ChangeState(GameState.CardSpawn);
    }
    
    private void CardSpawn()
    {
        //card spawning animation (other script)
        _cardSpawnerAndHandController.SetupCardsForNewCharacter();

        CanSelectCards = true;
        ChangeState(GameState.WaitingForInput);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void CardSelectionAndActivation()
    {
        Card cardComponent = null;
        CardAnimation cardAnimationComponent = null;
        
        //card reveal if mouse over card
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity);

        if (hit) 
        {
            //get the card component and check if it isn't null
            if (hit.collider.GetComponent<Card>() != null)
            {
                cardComponent = hit.collider.GetComponent<Card>();
                cardAnimationComponent = hit.collider.GetComponent<CardAnimation>();

                //guard if card already placed or cant be selected
                if (cardComponent.isPlaced || !CanSelectCards)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        _cardSpawnerAndHandController.TakeOneSpotCardBackToHand(cardComponent);
                    }
                    return;
                }
                
                //card select
                cardAnimationComponent.GameManager = this;
                cardAnimationComponent.CardSelect();
                cardAnimationComponent.DetailedCard.transform.rotation = Quaternion.Euler(Vector3.zero);
                CardSelectionGaugeTempShow(cardComponent);

                //card activation if click
                if (Input.GetMouseButtonDown(0))
                {
                    cardAnimationComponent.CardDeselect();
                    cardComponent.isPlaced = true;
                    CardActivation(cardComponent);
                }
            }
        }
    }

    private void CardSelectionGaugeTempShow(Card cardData)
    {
        const float fillAnimationTime = .2f;
        //gauges
        //design
        var designTempFillAmount = _designGauge.fillAmount + ((float)cardData.DesignValue / 100);
        _designTempGauge.DOFillAmount(designTempFillAmount, fillAnimationTime);
        //art
        var artTempFillAmount = _artGauge.fillAmount + ((float)cardData.ArtValue / 100);
        _artTempGauge.DOFillAmount(artTempFillAmount, fillAnimationTime);
        //programming
        var programmingTempFillAmount = _programmingGauge.fillAmount + ((float)cardData.ProgrammerValue / 100);
        _programmingTempGauge.DOFillAmount(programmingTempFillAmount, fillAnimationTime);
    }

    public void TempGaugeReset()
    {
        const float fillAnimationTime = .2f;
        
        _designTempGauge.DOFillAmount(_designGauge.fillAmount, fillAnimationTime);
        _artTempGauge.DOFillAmount(_artGauge.fillAmount, fillAnimationTime);
        _programmingTempGauge.DOFillAmount(_programmingGauge.fillAmount, fillAnimationTime);
    }

    private void CardActivation(Card cardComponent)
    {
        //gauges modification
        _currentCharacter.AddValueToGauges(cardComponent.DesignValue,cardComponent.ArtValue,cardComponent.ProgrammerValue);
        GaugeAnimation();

        //move card to selected card spot
        CardAnimation ?cardSelectAnimationComponent = cardComponent.gameObject.GetComponent<CardAnimation>();
        if (cardSelectAnimationComponent != null && _cardSpawnerAndHandController!=null)
        {
            //move card to spot
            if (CardSpotControllerList.Count > 0)
            {
                CardSpotController cardSpot = CardSpotControllerList.OrderBy(spot => spot.isTaken).FirstOrDefault();
                _cardSpawnerAndHandController.TakeOffCardFromHandToSpot(cardComponent, cardSpot!.Position);
                cardComponent._cardSpot = cardSpot;
                cardSpot.isTaken = true;
            }

        }
        
        //check if the good amount of cards are selected
        _currentNumberOfCardSelected++;
        if (_currentNumberOfCardSelected >= _numberOfCardsRequiredPerCharacter)
        {
            CanSelectCards = false;
            StartCoroutine(MakeCardButtonsAppears());
        }
    }

    private IEnumerator MakeCardButtonsAppears()
    {
        const float seconds = 2f;
        yield return new WaitForSeconds(seconds);
        _cardConfirmButton.gameObject.SetActive(true);
        _cardCancelButton.gameObject.SetActive(true);
        
        //anim
        const float animSpeed = .5f;
        _cardConfirmButton.gameObject.transform.DOScale(Vector3.one, animSpeed);
        _cardCancelButton.gameObject.transform.DOScale(Vector3.one, animSpeed);
    }

    private void CardConfirm()
    {
        MakeCardButtonDisappear();
        EndCharacterWithRoleAttribution();
    }

    private void CardCancel()
    {
        MakeCardButtonDisappear();
        _cardSpawnerAndHandController.TakeAllSpotsCardsBackToHand();
        ChangeState(GameState.WaitingForInput);
    }

    private void MakeCardButtonDisappear()
    {
        const float animSpeed = .2f;
        _cardConfirmButton.gameObject.transform.DOScale(Vector3.zero, animSpeed).OnComplete(UIButtonReset);
        _cardCancelButton.gameObject.transform.DOScale(Vector3.zero, animSpeed).OnComplete(UIButtonReset);
    }

    private void EndCharacterWithRoleAttribution()
    {
        ChangeState(GameState.CharacterRoleAttribution);
    }

    private void GaugeAnimation()
    {
        //white instant effect behind gauges
        _designBackGauge.fillAmount = (float)_currentCharacter.RolesGaugesDictionary[Role.Designer] / 100;
        _artBackGauge.fillAmount = (float)_currentCharacter.RolesGaugesDictionary[Role.Artist] / 100;
        _programmingBackGauge.fillAmount = (float)_currentCharacter.RolesGaugesDictionary[Role.Programmer] / 100;
        
        //DOTween effect with real gauge right after
        _designGauge.DOComplete();
        _artGauge.DOComplete();
        _programmingGauge.DOComplete();
        _designGauge.DOFillAmount(_designBackGauge.fillAmount, _fillAmountTime);
        _artGauge.DOFillAmount(_artBackGauge.fillAmount, _fillAmountTime);
        _programmingGauge.DOFillAmount(_programmingBackGauge.fillAmount, _fillAmountTime);
    }

    private void CharacterRoleAttribution()
    {
        //variable to know if character has been greatly associated with his role
        bool doesCharacterHasPredefinedRole = _currentCharacter.CharacterPredefinedRole == _currentCharacter.CharacterRoleGiver();

        AddCharacterToList(_currentCharacter, _currentCharacter.CharacterRoleGiver());
        ChangeState(GameState.CharacterExit);
    }

    private void CharacterExit()
    {
        //character exit animation
        CharacterDataList.Remove(_currentCharacter);
        var characterController = _currentCharacterGameObject.GetComponent<CharacterController>();
        if (characterController!=null)
            characterController.CharacterExitAnimation();
        
        //remove cards
        _cardSpawnerAndHandController.RemoveCurrentHandCardAndSpawnNewHand();
        _currentNumberOfCardSelected = 0;
        
        //next step
        StartCoroutine(SpawnANewCharacter());
    }

    private IEnumerator SpawnANewCharacter()
    {
        const float secondsToWait = 2f;
        yield return new WaitForSeconds(secondsToWait);
        UIGaugesReset();
        ChangeState(GameState.CharacterSpawn);
    }
    
    private void AddCharacterToList(CharacterData characterData, Role? role)
    {
        switch (role)
        {
            case Role.Designer:
                _charactersDesignerList.Add(characterData);
                UpdateListsVisual(_charactersDesignerList, _designerListTextMeshPro);
                break;
            case Role.Artist:
                _charactersArtistList.Add(characterData);
                UpdateListsVisual(_charactersArtistList, _artistListTextMeshPro);
                break;
            case Role.Programmer:
                _charactersProgrammerList.Add(characterData);
                UpdateListsVisual(_charactersProgrammerList, _programmerListTextMeshPro);
                break;
        }
    }
    
    private void UpdateListsVisual(List<CharacterData> characterList, TextMeshProUGUI textMesh)
    {
        string listText = string.Empty;
        foreach (var character in characterList)
        {
            listText += $"\n- {character.CharacterName}";
        }
        textMesh.text = listText;
    }

    private void EndGame()
    {
        print("end game");
    }
}

public enum GameState
{
    Start,
    CharacterSpawn,
    CardSpawn,
    WaitingForInput,
    CharacterRoleAttribution,
    CharacterExit,
    End
}

