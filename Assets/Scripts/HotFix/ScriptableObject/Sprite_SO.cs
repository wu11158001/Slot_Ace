using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Sprite_SO", menuName = "Scriptable Objects/Sprite_SO")]
public class Sprite_SO : ScriptableObject
{
    public List<Sprite> SpriteList;
}
