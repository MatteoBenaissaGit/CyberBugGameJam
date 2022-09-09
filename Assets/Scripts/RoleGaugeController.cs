using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RoleGaugeController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float _lenghtValue;
    
    [Header("Gauges References")]
    [SerializeField] private Transform _designRoleGauge;
    [SerializeField] private Transform _artRoleGauge;
    [SerializeField] private Transform _programmerRoleGauge;

    [Header("Game Manager Reference")] 
    [SerializeField] private GameManager _gameManager;

    private Vector3 GaugesLenghtsValues()
    {
        var totalNumberOfCharacters = _gameManager.CharactersDesignerList.Count + _gameManager.CharactersArtistList.Count + _gameManager.CharactersProgrammerList.Count;
        
        var designListCountValue = _gameManager.CharactersDesignerList.Count == 0 ? 1 : _gameManager.CharactersDesignerList.Count;
        float designLenghtValue = (_lenghtValue / totalNumberOfCharacters) * designListCountValue;
        
        var artListCountValue = _gameManager.CharactersArtistList.Count == 0 ? 1 : _gameManager.CharactersArtistList.Count;
        float artLenghtValue = (_lenghtValue / totalNumberOfCharacters) * artListCountValue;
        
        var programmerListCountValue = _gameManager.CharactersProgrammerList.Count == 0 ? 1 : _gameManager.CharactersProgrammerList.Count;
        float programmerLenghtValue = (_lenghtValue / totalNumberOfCharacters) * programmerListCountValue;
        
        return new Vector3(designLenghtValue, artLenghtValue, programmerLenghtValue);
    }

    public void UpdateRoleGauges()
    {
        var gaugeVector = GaugesLenghtsValues();
        const float animSpeed = .5f;
        var designVector = new Vector3(gaugeVector.x, _designRoleGauge.transform.localScale.y, _designRoleGauge.transform.localScale.z);
        _designRoleGauge.transform.DOScale(designVector, animSpeed);
        var artVector = new Vector3(gaugeVector.y, _artRoleGauge.transform.localScale.y, _artRoleGauge.transform.localScale.z);
        _artRoleGauge.transform.DOScaleX(gaugeVector.y, animSpeed);
        var programmerVector = new Vector3(gaugeVector.z, _programmerRoleGauge.transform.localScale.y, _programmerRoleGauge.transform.localScale.z);
        _programmerRoleGauge.transform.DOScaleX(gaugeVector.z,animSpeed);
    }
}
