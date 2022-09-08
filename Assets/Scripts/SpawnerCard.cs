using System.Collections;
using System.Collections.Generic;
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
            GameObject cardPrefab = Instantiate(card, new Vector3(coordX,coordY,coordZ), Quaternion.identity);
            cardPrefab.transform.Rotate(new Vector3(0, 0, rotationZ));
            cardPrefab.GetComponent<Card>().gA = cardDataAlea.gA;
            cardPrefab.GetComponent<Card>().gD = cardDataAlea.gD;
            cardPrefab.GetComponent<Card>().gP = cardDataAlea.gP;

        }
    }

    private CardData WhatCardSpawn()
    {
        int intCardDataAlea = Random.Range(0, cardPossible.Count);
        cardDataAlea = cardPossible[intCardDataAlea];
        return (cardDataAlea);
    }


}
