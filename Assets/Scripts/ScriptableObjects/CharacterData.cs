using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/CharacterData", order = 1)]
public class CharacterData : ScriptableObject
{
    public string CharacterName;
    public Role CharacterPredefinedRole;

    public Dictionary<Role, int> RolesGaugesDictionary = new Dictionary<Role, int>()
    {
        { Role.Designer, 0 },
        { Role.Artist, 0},
        { Role.Programmer, 0}
    };

    public void AddValueToGauges(int designValue, int artValue, int progValue)
    {
        RolesGaugesDictionary[Role.Designer] += designValue;
        RolesGaugesDictionary[Role.Artist] += artValue;
        RolesGaugesDictionary[Role.Programmer] += progValue;
        
        //guard if it gets over 100
        if (RolesGaugesDictionary[Role.Designer] > 100) RolesGaugesDictionary[Role.Designer] = 100;
        if (RolesGaugesDictionary[Role.Artist] > 100) RolesGaugesDictionary[Role.Artist] = 100;
        if (RolesGaugesDictionary[Role.Programmer] > 100) RolesGaugesDictionary[Role.Programmer] = 100;
    }

    public Role? CharacterRoleGiver()
    {
        //get the role with the biggest value
        RolesGaugesDictionary = RolesGaugesDictionary
            .OrderBy(x => x.Value)
            .ToDictionary(x => x.Key, x => x.Value);
        
        //return the role where the character has the biggest values
        return RolesGaugesDictionary.First().Key switch
        {
            Role.Designer => Role.Designer,
            Role.Artist => Role.Artist,
            Role.Programmer => Role.Programmer,
            _ => null
        };
    }
}

public enum Role
{
    Designer,
    Artist,
    Programmer
}
