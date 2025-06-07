using UnityEngine;

public class AnimalUtil
{
    public static Sprite GetAnimalSpriteByAnimalID(int animalName) {
        Sprite selectedSprite;
        SpriteManager sm = SpriteManager.GetInstance();
        Animal animal = (Animal)animalName;
        Debug.Log("dog sprite!!");
        Debug.Log(sm.dogSprite);
        switch (animal)
        {
            case Animal.ANIMAL_DOG:
                selectedSprite = sm.dogSprite;
                break;
            case Animal.ANIMAL_CAT:
                selectedSprite = sm.catSprite;
                break;
            case Animal.ANIMAL_CHICKEN:
                selectedSprite = sm.chickenSprite;
                break;
            case Animal.ANIMAL_HORSE:
                selectedSprite = sm.horseSprite;
                break;
            case Animal.ANIMAL_ELEPHANT:
                selectedSprite = sm.elephantSprite;
                break;
            case Animal.ANIMAL_DEER:
                selectedSprite = sm.deerSprite;
                Debug.Log("deer sprite changed!");
                break;
            default:
                Debug.LogWarning($"【警告】未知的動物：{animalName}");
                return null; // 退出方法以避免錯誤
        }
        return selectedSprite;
    
    }
}
