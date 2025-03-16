using UnityEngine;

public class SpriteManager
{
    static private SpriteManager instance;

    public Sprite dogSprite; // �����Ϥ�

    public Sprite catSprite; // �ߪ��Ϥ�

    public Sprite chickenSprite; // �����Ϥ�

    public Sprite horseSprite; // �����Ϥ�

    public Sprite elephantSprite; // �H���Ϥ�

    public Sprite deerSprite; // �����Ϥ�

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
