using System.Collections.Generic;
using UnityEngine;

public class AuthUser
{
    public string email;
    public string displayName;

    public AuthUser(string email, string displayName)
    {
        this.email = email;
        this.displayName = displayName;
    }

    public Dictionary<string, string> GetDictionary()
    {
        return new Dictionary<string, string>
        {
            { "email", email },
            { "displayName", displayName }
        };
    }
}
