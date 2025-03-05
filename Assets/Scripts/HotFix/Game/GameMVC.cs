using UnityEngine;

public class GameMVC : MonoBehaviour
{
    public Game_View game_View { get; private set; }
    public Game_Controller game_Contriller { get; private set; }
    public Game_Model game_Model { get; private set; }
    public GameControlView gameControlView { get; private set; }

    private void Awake()
    {
        Debug.Log("GameMVC 初始化...");

        game_Contriller = GetComponent<Game_Controller>();
        game_Model = GetComponent<Game_Model>();
        game_View = GetComponent<Game_View>();

        game_Contriller.Initialize(this);
        game_Model.Initialize(this);
        game_View.Initialize(this);

        ViewManager.I.OpenView<GameControlView>(ViewEnum.GameControlView, (view) =>
        {
            gameControlView = view;
            view.SetGameMVC(this);

            Debug.Log("GameMVC 初始化完成");
        });
    }
}
