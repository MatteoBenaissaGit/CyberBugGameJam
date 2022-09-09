using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RoleGaugeController : MonoBehaviour
{
    [Header("Gauges References")]
    [SerializeField] private Image _designRoleGauge;
    [SerializeField] private Image _artRoleGauge;

    [Header("Game Manager Reference")] 
    [SerializeField] private GameManager _gameManager;

    private Vector2 GaugesLenghtsValues()
    {
        var designCount = _gameManager.CharactersDesignerList.Count == 0 ? 1 : _gameManager.CharactersDesignerList.Count;
        var artCount = _gameManager.CharactersArtistList.Count == 0 ? 1 : _gameManager.CharactersArtistList.Count;
        var programmerCount = _gameManager.CharactersProgrammerList.Count == 0 ? 1 : _gameManager.CharactersProgrammerList.Count;
        
        var totalNumberOfCharacters = designCount + artCount + programmerCount;
        
        var designPercentage =  (designCount / totalNumberOfCharacters)/100;
        var artPercentage = (designPercentage + (artCount / totalNumberOfCharacters))/100;
        
        return new Vector2(designPercentage, artPercentage);
    }

    public void UpdateRoleGauges()
    {
        var gaugeVector = GaugesLenghtsValues();
        print(gaugeVector);
        const float animSpeed = .5f;
        _designRoleGauge.DOFillAmount(gaugeVector.x, animSpeed);
        _artRoleGauge.DOFillAmount(gaugeVector.y, animSpeed);
    }
}
