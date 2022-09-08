using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpawnerCard : MonoBehaviour
{
    private CardData cardDataAlea;
    private CardData whoSpawn;
    private int coordX;
    private float coordY =-3f;
    private int coordZ = 0;
    private float rotationZ = 0f;
    public GameObject card;
    [Range(0,7)] public int nbCard;
    public List<CardData> cardPossible;

    [SerializeField] private int OffsetX = -1;
    [SerializeField] private float OffsetY = 0f;
    [SerializeField] private int OffsetZ = 0;

    public void SpawnCard()
    {
        float animationSpeed = .5f;
        float startOffsetY = -1.5f;
        
        for(int i = 0; i<nbCard; i++)
        {
            whoSpawn = WhatCardSpawn();
            if (i % 2 == 1)
            {
                coordX = (i / 2) + 1 + OffsetX;
                rotationZ = ((i / 2) + 1)*(-10);
                if(i == 1){
                    coordY=-3.14f + OffsetY;
                }
                else if(i == 3){
                    coordY=-3.46f + OffsetY;
                }
                else if(i == 5){
                    coordY=-4f + OffsetY;
                }
                coordZ+=1 + OffsetZ;
            }
            if(i%2 == 0)
            {
                coordX = i / (-2) + OffsetX;
                rotationZ = i / (-2)*(-10);
            }
            if (i == 0)
            {
                coordX = i + OffsetX;
                coordY = -3f + OffsetY;
            }
            
            Vector3 endPosition = new Vector3(coordX, coordY, coordZ);
            GameObject cardPrefab = Instantiate(card, endPosition, Quaternion.identity);
            cardPrefab.transform.Rotate(new Vector3(0, 0, rotationZ));
            
            cardPrefab.GetComponent<Card>().gA = cardDataAlea.gA;
            cardPrefab.GetComponent<Card>().gD = cardDataAlea.gD;
            cardPrefab.GetComponent<Card>().gP = cardDataAlea.gP;
            
            //animation card (slow speed for each card to have a delayed effect)
            var position = cardPrefab.transform.position;
            Vector3 startPosition = new Vector3(position.x, position.y + startOffsetY, position.z);
            position = startPosition;
            cardPrefab.transform.position = position;
            cardPrefab.transform.DOComplete();
            cardPrefab.transform.DOMove(new Vector3(coordX, coordY, coordZ), animationSpeed);

            startOffsetY -= 2f;
            animationSpeed += .3f;
            
            //associate data with card description text
        }
    }

    private CardData WhatCardSpawn()
    {
        int intCardDataAlea = Random.Range(0, cardPossible.Count);
        cardDataAlea = cardPossible[intCardDataAlea];
        return (cardDataAlea);
    }


}
