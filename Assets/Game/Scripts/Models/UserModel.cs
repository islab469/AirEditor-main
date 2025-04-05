using Firebase.Firestore;
using System.Collections.Generic;

namespace FirestoreModels
{
    public class UserModel
    {
        public string email;
        public string displayName;

        public UserModel(string email, string displayName)
        {
            this.email = email;
            this.displayName = displayName;
        }

        public Dictionary<string, object> GetDictionary()
        {
            return new Dictionary<string, object>
            {
                { "email", email },
                { "displayName", displayName },
                { "createdAt", Timestamp.GetCurrentTimestamp() }
            };
        }
    }
}
