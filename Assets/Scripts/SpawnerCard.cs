using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerCard : MonoBehaviour
{
    private CardData cardDataAlea;
    private CardData whoSpawn;
    private int CoordX;
    public GameObject card;
    public int nbCard;
    public List<CardData> cardPossible;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SpawnCard();
        }
    }

    void SpawnCard()
    {
        whoSpawn = WhatCardSpawn();
        for(int i =0; i<nbCard; i++)
        {
            if (i % 2 == 1 && i !=0)
            {
                CoordX = (i / 2) + 1;
            }
            if(i%2 == 0 && i != 0)
            {
                CoordX = i / (-2);
            }
            GameObject cardPrefab = Instantiate(card, new Vector2(CoordX,0), Quaternion.identity);
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
