using UnityEngine;

public class SpriteManager
{
    static private SpriteManager instance;

    public Sprite dogSprite; // 狗的圖片

    public Sprite catSprite; // 貓的圖片

    public Sprite chickenSprite; // 雞的圖片

    public Sprite horseSprite; // 馬的圖片

    public Sprite elephantSprite; // 象的圖片

    public Sprite deerSprite; // 鹿的圖片

    private SpriteManager() {
        //load don't need to contains the .jpg .png ... sub title
        catSprite = Resources.Load<Sprite>("Image/Animals/cat");
        dogSprite = Resources.Load<Sprite>("Image/Animals/dog");
        chickenSprite = Resources.Load<Sprite>("Image/Animals/chicken");
        horseSprite = Resources.Load<Sprite>("Image/Animals/horse");
        elephantSprite = Resources.Load<Sprite>("Image/Animals/elephant");
        deerSprite = Resources.Load<Sprite>("Image/Animals/deer");
    }

    public static SpriteManager GetInstance() {
        if(instance == null){
            instance = new SpriteManager();
        }

        return instance;
    }






}
