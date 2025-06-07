using UnityEngine.SceneManagement;

public enum SceneType : int
{
    SCENE_LOGIN,
    SCENE_CHOOSE_IMAGE,
    SCENE_3DMODEL,
    SCENE_CREATE_PROJECT,
    SCENE_AIQA,
    SCENE_AIQUESTION,

    SCENE_LENGTH
}

public class SceneSystem{
    public static SceneType curnSceneType = SceneType.SCENE_LOGIN;

    public static SceneType lastSceneType = SceneType.SCENE_LOGIN;




    public static void changeScene(SceneType type) {
        lastSceneType = curnSceneType;
        curnSceneType = type;
        SceneManager.LoadScene((int)type);
    }



}