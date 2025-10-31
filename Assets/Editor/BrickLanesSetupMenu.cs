#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public static class BrickLanesSetupMenu
{
    [MenuItem("Tools/Build Brick + Lanes Prototype")]
    public static void Build()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "Game";

        var gameRoot = new GameObject("GameRoot");

        var camGO = new GameObject("Main Camera");
        camGO.transform.SetParent(gameRoot.transform);
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        camGO.tag = "MainCamera";
        camGO.transform.position = new Vector3(0, 0, -10);

        var bg = new GameObject("Background");
        bg.transform.SetParent(gameRoot.transform);
        var bgSR = bg.AddComponent<SpriteRenderer>();
        bgSR.sprite = MakeSolidSprite(32, 32, new Color(0.11f, 0.2f, 0.36f, 1f));
        bg.transform.localScale = new Vector3(20f, 12f, 1f);
        bgSR.sortingOrder = -3;

        var brick = new GameObject("Brick");
        brick.transform.SetParent(gameRoot.transform);
        var brickSR = brick.AddComponent<SpriteRenderer>();
        brickSR.sprite = MakeSolidSprite(16, 16, new Color(0.66f, 0.49f, 0.32f, 1f));
        brick.transform.position = Vector3.zero;
        brick.transform.localScale = new Vector3(6.0f, 0.6f, 1f);
        brickSR.sortingOrder = -1;

        var lanes = new GameObject("Lanes");
        lanes.transform.SetParent(brick.transform);
        lanes.transform.localPosition = Vector3.zero;
        float topY = 1.2f, midY = 0f, botY = -1.2f;
        CreateLaneLine(lanes.transform, "LaneTop",   topY,  new Color(0.2f, 0.85f, 0.2f, 0.9f));
        CreateLaneLine(lanes.transform, "LaneMid",   midY,  new Color(0.9f, 0.9f, 0.9f, 0.6f));
        CreateLaneLine(lanes.transform, "LaneBottom",botY,  new Color(0.2f, 0.4f, 0.9f, 0.9f));

        var playerRoot = new GameObject("PlayerRoot");
        playerRoot.transform.SetParent(brick.transform);
        playerRoot.transform.localPosition = Vector3.zero;

        var player = new GameObject("Player");
        player.transform.SetParent(playerRoot.transform);
        var sr = player.AddComponent<SpriteRenderer>();
        sr.sprite = MakeSolidSprite(16, 16, new Color(1f, 0.95f, 0.2f, 1f));
        sr.sortingOrder = 0;
        var rb = player.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        player.AddComponent<BoxCollider2D>();

        var hitTop = new GameObject("HitZone_Top");
        hitTop.transform.SetParent(playerRoot.transform);
        hitTop.transform.localPosition = new Vector3(0, topY, 0);
        var ct = hitTop.AddComponent<CircleCollider2D>();
        ct.isTrigger = true; ct.radius = 0.25f;

        var hitMid = new GameObject("HitZone_Mid");
        hitMid.transform.SetParent(playerRoot.transform);
        hitMid.transform.localPosition = new Vector3(0, midY, 0);
        var cm = hitMid.AddComponent<CircleCollider2D>();
        cm.isTrigger = true; cm.radius = 0.25f;

        var hitBot = new GameObject("HitZone_Bottom");
        hitBot.transform.SetParent(playerRoot.transform);
        hitBot.transform.localPosition = new Vector3(0, botY, 0);
        var cb = hitBot.AddComponent<CircleCollider2D>();
        cb.isTrigger = true; cb.radius = 0.25f;

        var mover = playerRoot.AddComponent<PlayerBrickController>();
        mover.jumpHeight = 0.8f;
        mover.jumpUpTime = 0.18f;
        mover.jumpDownTime = 0.15f;
        mover.underOffset = 0.7f;
        mover.hitTop = hitTop.GetComponent<Collider2D>();
        mover.hitMid = hitMid.GetComponent<Collider2D>();
        mover.hitBottom = hitBot.GetComponent<Collider2D>();

        var dash = gameRoot.AddComponent<DashInverter>();
        dash.worldRoot = gameRoot.transform;
        dash.dashDuration = 3.0f;
        dash.cooldown = 6.0f;

        System.IO.Directory.CreateDirectory("Assets/Scenes");
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, "Assets/Scenes/Game.unity");
        EditorUtility.DisplayDialog("Brick + Lanes", "Scene created at Assets/Scenes/Game.unity", "OK");
    }

    static void CreateLaneLine(Transform parent, string name, float localY, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.localPosition = new Vector3(0, localY, 0);
        var lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.useWorldSpace = false;
        lr.widthMultiplier = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = color;
        lr.sortingOrder = -2;
        lr.SetPosition(0, new Vector3(-10f, 0, 0));
        lr.SetPosition(1, new Vector3( 10f, 0, 0));
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
