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
        float designCount = 1 +  _gameManager.CharactersDesignerList.Count;
        float artCount = 1 + _gameManager.CharactersArtistList.Count;
        float programmerCount = 1 + _gameManager.CharactersProgrammerList.Count;
        
        float totalNumberOfCharacters = designCount + artCount + programmerCount;

        float designPercentage =  (designCount / totalNumberOfCharacters);
        float artPercentage = (designPercentage + (artCount / totalNumberOfCharacters));
        
        return new Vector2(designPercentage, artPercentage);
    }

    public void UpdateRoleGauges()
    {
        var gaugeVector = GaugesLenghtsValues();
        const float animSpeed = .5f;
        _designRoleGauge.DOFillAmount(gaugeVector.x, animSpeed);
        _artRoleGauge.DOFillAmount(gaugeVector.y, animSpeed);
    }
}
