using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;

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

    [Header("References")] 
    [SerializeField] private SpawnerCard _spawnerCard;
    
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
    [SerializeField] private List<Transform> _cardSpotList = new List<Transform>();

    [Header("Debug")] 
    [SerializeField] private GameState _gameState;
    [SerializeField] private CharacterData _currentCharacter;

    private bool _cardIsCurrentlySelected = false;

    private void Start()
    {
        UIReset();
        ChangeState(GameState.Start);
    }

    private void UIReset()
    {
        //gauges
        //design
        _designGauge.fillAmount = 0;
        _designBackGauge.fillAmount = 0;
        _designTempGauge.fillAmount = 0;
        //art
        _artGauge.fillAmount = 0;
        _artBackGauge.fillAmount = 0;
        _artTempGauge.fillAmount = 0;
        //programming
        _programmingGauge.fillAmount = 0;
        _programmingBackGauge.fillAmount = 0;
        _programmingTempGauge.fillAmount = 0;
        
        //text
        _designerListTextMeshPro.text = "";
        _artistListTextMeshPro.text = "";
        _programmerListTextMeshPro.text = "";
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
        }
        
        //Anim character

        ChangeState(GameState.CardSpawn);
    }
    
    private void CardSpawn()
    {
        //card spawning animation (other script)
        _spawnerCard.CardSpawning();
        
        //temporary ! step to the next state directly instead of waiting for the card spawn
        ChangeState(GameState.WaitingForInput);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void CardSelectionAndActivation()
    {
        Card cardComponent = null;
        CardSelectAnimation cardSelectAnimationComponent = null;
        
        //card reveal if mouse over card
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity);

        if (hit) 
        {
            //get the card component and check if it isn't null
            if (hit.collider.GetComponent<Card>() != null)
            {
                cardComponent = hit.collider.GetComponent<Card>();
                cardSelectAnimationComponent = hit.collider.GetComponent<CardSelectAnimation>();
                
                //card select
                cardSelectAnimationComponent.GameManager = this;
                cardSelectAnimationComponent.CardSelect();
                cardSelectAnimationComponent.DetailedCard.transform.rotation = Quaternion.Euler(Vector3.zero);
                CardSelectionGaugeTempShow(cardComponent);

                //card activation if click
                if (Input.GetMouseButtonDown(0))
                {
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
        _currentNumberOfCardSelected++;
        
        //gauges modification
        _currentCharacter.AddValueToGauges(cardComponent.DesignValue,cardComponent.ArtValue,cardComponent.ProgrammerValue);
        GaugeAnimation();
        
        //check if the good amount of cards are selected
        if (_currentNumberOfCardSelected >= _numberOfCardsRequiredPerCharacter)
            ChangeState(GameState.CharacterRoleAttribution);
        
        //move card to selected card spot
        
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
        
        //remove cards
        
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

