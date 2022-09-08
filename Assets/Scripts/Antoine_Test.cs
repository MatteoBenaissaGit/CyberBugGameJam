using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antoine_Test : MonoBehaviour
{
    [SerializeField] private int Number;
    [SerializeField] private string String;
    [SerializeField] private float Decimal;
    private int MaxNumber = 1000;

    void IncreaseDecimal() {
        if (Decimal < MaxNumber) {
                    Decimal++;
                } else {
                    Decimal = MaxNumber;
                }

                print(Decimal);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // Give the modulo of the given number
        print(Number % 2);

        // Add an exclamation mark to the string
        print(String + "!");

        // Give Number - Decimal
        print(Number - Decimal);
    }

    // Update is called once per frame
    void Update()
    {
        IncreaseDecimal();
    }
}
