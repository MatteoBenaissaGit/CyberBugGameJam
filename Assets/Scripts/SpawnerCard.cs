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
        for(int i =0; i<nbCard; i++)
        {
            whoSpawn = WhatCardSpawn();
            if (i % 2 == 1 && i !=0)
            {
                coordX = (i / 2) + 1;
                rotationZ = ((i / 2) + 1)*(-10);
                if(i == 1){
                    coordY=-3.14f;
                }
                else if(i == 3){
                    coordY=-3.46f;
                }
                else if(i == 5){
                    coordY=-4f;
                }
                coordZ+=1;
            }
            if(i%2 == 0 && i != 0)
            {
                coordX = i / (-2);
                rotationZ = i / (-2)*(-10);
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
