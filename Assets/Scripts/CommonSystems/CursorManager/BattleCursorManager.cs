using NGDtuanh.Collections;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class BattleCursorManager : MonoBehaviour {
    [SerializeField] private EnumMap<CursorType, Texture2D> textures;

    private EnumMap<CursorType, Vector2> hotspots = new();
    private EntityQuery                  inputCastQuery;
    private CursorType                   curCursor;

    private void Awake() {
        // Set hotspots
        hotspots[CursorType.Aiming] = textures[CursorType.Aiming].Size() / 2;
        hotspots[CursorType.Attack] = Vector2.zero;
        hotspots[CursorType.Normal] = Vector2.zero;

        // Create inputCast query
        using var inputCastQueryBuilder = new EntityQueryBuilder(Allocator.Temp);
        inputCastQuery = inputCastQueryBuilder
            .WithAll<InputCastData>()
            .Build(World.DefaultGameObjectInjectionWorld.EntityManager);

        // set default cursor
        SetCursor(CursorType.Normal, true);
    }

    private void LateUpdate() {
        if (inputCastQuery.IsEmpty) return;

        var castData = inputCastQuery.GetSingleton<InputCastData>();

        // FIRST: AIMING
        if (Keyboard.current.aKey.isPressed) {
            SetCursor(CursorType.Aiming);
            return;
        }

        // SECOND: ATTACK
        if (castData.isHitActor) {
            SetCursor(CursorType.Attack);
            return;
        }

        // THIRD: NORMAL
        SetCursor(CursorType.Normal);
    }

    private void SetCursor(CursorType newCursor, bool force = false) {
        if (!force && curCursor == newCursor) return;
        curCursor = newCursor;

        Cursor.SetCursor(textures[newCursor], hotspots[newCursor], CursorMode.Auto);
    }

    public enum CursorType {
        Aiming
      , Attack
      , Normal
    }
}