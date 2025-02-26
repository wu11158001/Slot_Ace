using UnityEngine;

public class GameMVC : MonoBehaviour
{
    public Game_View game_View { get; private set; }
    public Game_Controller game_Contriller { get; private set; }
    public Game_Model game_Model { get; private set; }

    private void Awake()
    {
        game_View = GetComponent<Game_View>();
        game_Contriller = GetComponent<Game_Controller>();
        game_Model = GetComponent<Game_Model>();

        ViewManager.I.OpenView<GameControlView>(ViewEnum.GameControlView, (view) =>
        {
            view.SetGameMVC(this);
        });
    }
}
