
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public static class SetupMenu
{
    [MenuItem("Tools/Build Runner Prototype")]
    public static void BuildRunnerPrototype()
    {
        // Create new scene
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "Game";
        
        // GameRoot
        var gameRoot = new GameObject("GameRoot");
        
        // Camera under GameRoot
        var camGO = new GameObject("Main Camera");
        camGO.transform.SetParent(gameRoot.transform);
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        camGO.tag = "MainCamera";
        camGO.transform.position = new Vector3(0, 0, -10);
        
        // Lanes
        var lanes = new GameObject("Lanes");
        lanes.transform.SetParent(gameRoot.transform);
        var laneTop = new GameObject("LaneTop");   laneTop.transform.SetParent(lanes.transform);
        var laneMid = new GameObject("LaneMid");   laneMid.transform.SetParent(lanes.transform);
        var laneBottom = new GameObject("LaneBottom"); laneBottom.transform.SetParent(lanes.transform);
        laneTop.transform.position    = new Vector3(0,  2.5f, 0);
        laneMid.transform.position    = new Vector3(0,  0.0f, 0);
        laneBottom.transform.position = new Vector3(0, -2.5f, 0);

        // Visualize lanes (simple line renderers)
        AddLaneLine(laneTop,   new Color(0.2f, 0.8f, 0.2f, 0.8f));
        AddLaneLine(laneMid,   new Color(0.8f, 0.8f, 0.8f, 0.5f));
        AddLaneLine(laneBottom,new Color(0.2f, 0.4f, 0.8f, 0.8f));

        // PlayerRoot + Player (square sprite)
        var playerRoot = new GameObject("PlayerRoot");
        playerRoot.transform.SetParent(gameRoot.transform);
        playerRoot.transform.position = laneMid.transform.position;

        var playerGO = new GameObject("Player");
        playerGO.transform.SetParent(playerRoot.transform);
        var sr = playerGO.AddComponent<SpriteRenderer>();
        sr.sprite = MakeSquareSprite();
        sr.color = new Color(1f, 0.95f, 0.2f, 1f);
        var rb = playerGO.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        playerGO.AddComponent<BoxCollider2D>();

        // Attach scripts
        var mover = playerRoot.AddComponent<PlayerMover>();
        mover.laneTop    = laneTop.transform;
        mover.laneMid    = laneMid.transform;
        mover.laneBottom = laneBottom.transform;
        mover.transitionTime = 0.18f; // 0.15–0.20

        var dash = gameRoot.AddComponent<DashInverter>();
        dash.worldRoot = gameRoot.transform;
        dash.dashDuration = 3.0f;
        dash.cooldown = 6.0f;
        
        // Save scene
        string path = "Assets/Scenes/Game.unity";
        System.IO.Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(scene, path);
        EditorUtility.DisplayDialog("Runner Prototype", "Scene created at:\n" + path + "\nPress Play to test.\n\nKeys:\nUp/W = Up lane\nDown/S = Down lane\nSpace/LeftShift = Dash (flip 180°)", "OK");
    }

    static void AddLaneLine(GameObject host, Color color)
    {
        var lr = host.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.useWorldSpace = true;
        lr.widthMultiplier = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lr.endColor = color;
        lr.SetPosition(0, host.transform.position + new Vector3(-10f, 0, 0));
        lr.SetPosition(1, host.transform.position + new Vector3( 10f, 0, 0));
    }

    static Sprite MakeSquareSprite()
    {
        var tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
        var col = new Color(1f,1f,1f,1f);
        var px = new Color[16*16];
        for(int i=0;i<px.Length;i++) px[i] = col;
        tex.SetPixels(px);
        tex.Apply();
        var rect = new Rect(0,0,16,16);
        var pivot = new Vector2(0.5f,0.5f);
        var sprite = Sprite.Create(tex, rect, pivot, 16f);
        sprite.name = "WhiteSquare";
        return sprite;
    }
}
#endif
