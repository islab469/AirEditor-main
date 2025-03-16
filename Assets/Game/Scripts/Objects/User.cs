using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User{
    public string email;
    public string displayName;

    public User(string email, string displayName)
    {
        this.email = email;
        this.displayName = displayName;
    }

    public Dictionary<string, string> getDicionary()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("email", email);
        dict.Add("displayName", displayName);
        return dict;
    }
}
