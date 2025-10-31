#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public static class BrickSetupMenu
{
    [MenuItem("Tools/Build Brick Prototype")]
    public static void BuildBrickPrototype()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "Game";

        // GameRoot
        var gameRoot = new GameObject("GameRoot");

        // Camera
        var camGO = new GameObject("Main Camera");
        camGO.transform.SetParent(gameRoot.transform);
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        camGO.tag = "MainCamera";
        camGO.transform.position = new Vector3(0, 0, -10);

        // Background
        var bg = new GameObject("Background");
        bg.transform.SetParent(gameRoot.transform);
        var bgSR = bg.AddComponent<SpriteRenderer>();
        bgSR.sprite = MakeSolidSprite(32, 32, new Color(0.11f, 0.2f, 0.36f, 1f));
        bg.transform.localScale = new Vector3(20f, 12f, 1f);
        bgSR.sortingOrder = -2;

        // Brick platform
        var brick = new GameObject("Brick");
        brick.transform.SetParent(gameRoot.transform);
        var brickSR = brick.AddComponent<SpriteRenderer>();
        brickSR.sprite = MakeSolidSprite(16, 16, new Color(0.66f, 0.49f, 0.32f, 1f));
        brick.transform.position = Vector3.zero;
        brick.transform.localScale = new Vector3(6.0f, 0.6f, 1f);
        brickSR.sortingOrder = -1;

        // PlayerRoot anchored to brick
        var playerRoot = new GameObject("PlayerRoot");
        playerRoot.transform.SetParent(brick.transform);
        playerRoot.transform.localPosition = Vector3.zero;

        // Player visual
        var player = new GameObject("Player");
        player.transform.SetParent(playerRoot.transform);
        var sr = player.AddComponent<SpriteRenderer>();
        sr.sprite = MakeSolidSprite(16,16, new Color(1f, 0.95f, 0.2f, 1f));
        sr.sortingOrder = 0;
        var rb = player.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        player.AddComponent<BoxCollider2D>();

        // Attach scripts
        var mover = playerRoot.AddComponent<PlayerBrickController>();
        mover.jumpHeight = 0.8f;
        mover.jumpUpTime = 0.18f;
        mover.jumpDownTime = 0.15f;
        mover.underOffset = 0.7f;

        var dash = gameRoot.AddComponent<DashInverter>();
        dash.worldRoot = gameRoot.transform;
        dash.dashDuration = 3.0f;
        dash.cooldown = 6.0f;

        System.IO.Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Game.unity");
        EditorUtility.DisplayDialog("Brick Prototype", "Scene created at Assets/Scenes/Game.unity\nPlay keys:\nUp/W = Jump/Return from inverted\nDown/S = Invert under brick\nSpace/LeftShift = Dash (flip 180°)", "OK");
    }

    static Sprite MakeSolidSprite(int w, int h, Color c)
    {
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        var px = new Color[w*h];
        for(int i=0;i<px.Length;i++) px[i] = c;
        tex.SetPixels(px);
        tex.Apply();
        var rect = new Rect(0,0,w,h);
        var pivot = new Vector2(0.5f,0.5f);
        var sp = Sprite.Create(tex, rect, pivot, w);
        sp.name = "Solid";
        return sp;
    }
}
#endif
