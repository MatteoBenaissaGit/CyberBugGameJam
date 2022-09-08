using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/CharacterData", order = 1)]
public class CharacterData : ScriptableObject
{
    public string CharacterName;
    public Role CharacterRole;

    public int DesignerGauge = 0;
    public int ArtistGauge = 0;
    public int ProgrammerGauge = 0;
}

public enum Role
{
    Designer,
    Artist,
    Programmer
}
