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
    public int CurrentNumberOfCardSelected;
    
    [Header("Data list")]
    public List<CardData> CardDataList = new List<CardData>();
    public List<CharacterData> CharacterDataList = new List<CharacterData>();
    
    //role listing (Empty lists that are completed throughout the game when character's roles are decided)
    public List<CharacterData> CharactersDesignerList = new List<CharacterData>();
    public List<CharacterData> CharactersArtistList = new List<CharacterData>();
    public List<CharacterData> CharactersProgrammerList = new List<CharacterData>();

    [Header("Gauges References")]
    [Space(5)] public Image DesignGauge;
    [SerializeField] private Image _designBackGauge;
    [SerializeField] private Image _designTempGauge;
    [Space(5)] public Image ArtGauge;
    [SerializeField] private Image _artBackGauge;
    [SerializeField] private Image _artTempGauge;
    [Space(5)] public Image ProgrammingGauge;
    [SerializeField] private Image _programmingBackGauge;
    [SerializeField] private Image _programmingTempGauge;

    [Header("Role Gauge Reference")]
    [SerializeField] private RoleGaugeController _roleGaugeController;

    [Header("CardSpot References")] 
    public List<CardSpotController> CardSpotControllerList = new List<CardSpotController>();

    [Header("Character References")]
    [SerializeField] private GameObject _characterPrefab;
    [SerializeField] private Transform _characterTransformPosition;
    private GameObject _currentCharacterGameObject;

    [Header("Card button References")] 
    [SerializeField] private Button _cardConfirmButton;
    [SerializeField] private Button _cardCancelButton;

    [Header("End Image References")] 
    [SerializeField] private SpriteRenderer _endSpriteRenderer;
    [SerializeField] private Sprite _tooMuchDesignerEndSprite;
    [SerializeField] private Sprite _tooMuchArtistEndSprite;
    [SerializeField] private Sprite _tooMuchProgrammerEndSprite;
    [SerializeField] private Sprite _perfectlyBalancedEndSprite;

    [Header("Debug")] 
    [SerializeField] private GameState _gameState;
    public CharacterData CurrentCharacter;

    private bool _cardIsCurrentlySelected = false;
    private CardSpawnerAndHandController _cardSpawnerAndHandController;
    public bool CanSelectCards = true;

    private void Start()
    {
        UIGaugesReset(Vector3.zero, true);
        UpdateEndImage();
        UIButtonReset();
        UpdateRoleListsVisual();
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

    public void UIGaugesReset(Vector3 gaugeAdd, bool resetToZero)
    {
        const float fillSpeed = .2f;
        //gauges
        //design
        var designValue = resetToZero ? 0 : DesignGauge.fillAmount + gaugeAdd.x / 100;
        DesignGauge.DOFillAmount(designValue,fillSpeed);
        _designBackGauge.DOFillAmount(designValue,fillSpeed);
        _designTempGauge.DOFillAmount(0,fillSpeed);
        //art
        var artValue = resetToZero ? 0 : ArtGauge.fillAmount + gaugeAdd.y / 100;
        ArtGauge.DOFillAmount(artValue,fillSpeed);
        _artBackGauge.DOFillAmount(artValue,fillSpeed);
        _artTempGauge.DOFillAmount(0,fillSpeed);
        //programming
        var programmingValue = resetToZero ? 0 : ProgrammingGauge.fillAmount + gaugeAdd.z / 100;
        ProgrammingGauge.DOFillAmount(programmingValue,fillSpeed);
        _programmingBackGauge.DOFillAmount(programmingValue,fillSpeed);
        _programmingTempGauge.DOFillAmount(0,fillSpeed);
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
            CurrentCharacter = CharacterDataList[0];
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
        characterControllerComponent.CharacterNameTextMesh.text = CurrentCharacter.CharacterName;
        if (CurrentCharacter.CharacterSprite != null)
            characterControllerComponent.CharacterSpriteRenderer.sprite = CurrentCharacter.CharacterSprite;
        //gauges values
        CurrentCharacter.AddValueToGauges(CurrentCharacter.DesignBaseValue,CurrentCharacter.ArtBaseValue,CurrentCharacter.ProgrammingBaseValue);
        GaugeAnimation();
        
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
                if (!CanSelectCards) return;

                //card select
                cardAnimationComponent.GameManager = this;
                if (!cardComponent.isPlaced)
                {
                    cardAnimationComponent.CardSelect();
                    cardAnimationComponent.DetailedCard.transform.rotation = Quaternion.Euler(Vector3.zero);
                    CardSelectionGaugeTempShow(cardComponent);
                }

                //card activation if click
                if (Input.GetMouseButtonDown(0))
                {
                    if (cardComponent.isPlaced)
                    {
                        //take card back
                        _cardSpawnerAndHandController.TakeOneSpotCardBackToHand(cardComponent);
                        return;
                    }
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
        var designTempFillAmount = DesignGauge.fillAmount + ((float)cardData.DesignValue / 100);
        _designTempGauge.DOFillAmount(designTempFillAmount, fillAnimationTime);
        //art
        var artTempFillAmount = ArtGauge.fillAmount + ((float)cardData.ArtValue / 100);
        _artTempGauge.DOFillAmount(artTempFillAmount, fillAnimationTime);
        //programming
        var programmingTempFillAmount = ProgrammingGauge.fillAmount + ((float)cardData.ProgrammerValue / 100);
        _programmingTempGauge.DOFillAmount(programmingTempFillAmount, fillAnimationTime);
    }

    public void TempGaugeReset()
    {
        const float fillAnimationTime = .2f;
        
        _designTempGauge.DOFillAmount((float)CurrentCharacter.RolesGaugesDictionary[Role.Designer]/100, fillAnimationTime);
        _artTempGauge.DOFillAmount((float)CurrentCharacter.RolesGaugesDictionary[Role.Artist]/100, fillAnimationTime);
        _programmingTempGauge.DOFillAmount((float)CurrentCharacter.RolesGaugesDictionary[Role.Programmer]/100, fillAnimationTime);
    }

    private void CardActivation(Card cardComponent)
    {
        //gauges modification
        CurrentCharacter.AddValueToGauges(cardComponent.DesignValue,cardComponent.ArtValue,cardComponent.ProgrammerValue);
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
                cardComponent.CardSpot = cardSpot;
                cardSpot.isTaken = true;
            }

        }
        
        //check if the good amount of cards are selected
        CurrentNumberOfCardSelected++;
        if (CurrentNumberOfCardSelected >= _numberOfCardsRequiredPerCharacter)
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
        _designBackGauge.fillAmount = (float)CurrentCharacter.RolesGaugesDictionary[Role.Designer] / 100;
        _artBackGauge.fillAmount = (float)CurrentCharacter.RolesGaugesDictionary[Role.Artist] / 100;
        _programmingBackGauge.fillAmount = (float)CurrentCharacter.RolesGaugesDictionary[Role.Programmer] / 100;
        
        //DOTween effect with real gauge right after
        DesignGauge.DOComplete();
        ArtGauge.DOComplete();
        ProgrammingGauge.DOComplete();
        DesignGauge.DOFillAmount(_designBackGauge.fillAmount, _fillAmountTime);
        ArtGauge.DOFillAmount(_artBackGauge.fillAmount, _fillAmountTime);
        ProgrammingGauge.DOFillAmount(_programmingBackGauge.fillAmount, _fillAmountTime);
    }

    private void CharacterRoleAttribution()
    {
        //variable to know if character has been greatly associated with his role
        bool doesCharacterHasPredefinedRole = CurrentCharacter.CharacterPredefinedRole == CurrentCharacter.CharacterRoleGiver();

        AddCharacterToList(CurrentCharacter, CurrentCharacter.CharacterRoleGiver());
        UpdateEndImage();
        
        ChangeState(GameState.CharacterExit);
    }

    private void CharacterExit()
    {
        //character exit animation
        CharacterDataList.Remove(CurrentCharacter);
        var characterController = _currentCharacterGameObject.GetComponent<CharacterController>();
        if (characterController!=null)
            characterController.CharacterExitAnimation();
        
        //remove cards
        _cardSpawnerAndHandController.RemoveCurrentHandCardAndSpawnNewHand();
        CurrentNumberOfCardSelected = 0;
        
        //next step
        StartCoroutine(SpawnANewCharacter());
    }

    private IEnumerator SpawnANewCharacter()
    {
        const float secondsToWait = 2f;
        yield return new WaitForSeconds(secondsToWait);
        UIGaugesReset(Vector3.zero, true);
        ChangeState(GameState.CharacterSpawn);
    }
    
    private void AddCharacterToList(CharacterData characterData, Role? role)
    {
        switch (role)
        {
            case Role.Designer:
                CharactersDesignerList.Add(characterData);
                break;
            case Role.Artist:
                CharactersArtistList.Add(characterData);
                break;
            case Role.Programmer:
                CharactersProgrammerList.Add(characterData);
                break;
        }
        UpdateRoleListsVisual();
    }
    
    private void UpdateRoleListsVisual()
    {
        _roleGaugeController.UpdateRoleGauges();
    }

    private void EndGame()
    {
        UpdateEndImage();
        print("end game");
    }

    private void UpdateEndImage()
    {
        var characterCountDictionary = new Dictionary<Role, int>()
        {
            { Role.Designer, CharactersDesignerList.Count},
            { Role.Artist, CharactersArtistList.Count},
            { Role.Programmer, CharactersProgrammerList.Count}
        };
        
        const int diffTolerance = 1;
        var ordered = characterCountDictionary.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        var diff = ordered.ElementAt(0).Value - ordered.ElementAt(1).Value;
        if (diff > diffTolerance)
        {
            _endSpriteRenderer.sprite = ordered.ElementAt(0).Key switch
            {
                Role.Designer => _tooMuchDesignerEndSprite,
                Role.Artist => _tooMuchArtistEndSprite,
                Role.Programmer => _tooMuchProgrammerEndSprite,
                _ => _endSpriteRenderer.sprite
            };
        }
        else
        {
            _endSpriteRenderer.sprite = _perfectlyBalancedEndSprite;
        }

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

