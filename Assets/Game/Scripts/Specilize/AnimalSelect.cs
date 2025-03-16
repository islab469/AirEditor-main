using UnityEngine;

public class AnimalSelect : MonoBehaviour
{

    public void SetSelectedAnimalById(int animalID)
    {
        SetSelectedAnimal((Animal)animalID);
    }

    private void SetSelectedAnimal(Animal animal)
    {
        PlayerPrefs.SetInt("SelectedModel", (int)animal); // �x�s��ܪ��ʪ��� PlayerPrefs
        PlayerPrefs.Save(); // �T�O�x�s���\

        Debug.Log($"�i�w��ܰʪ��j{animal}");
    }
}
