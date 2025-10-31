#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

public static class FullSetupMenu
{
    [MenuItem("Tools/Build Full Prototype")]
    public static void BuildFull()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "Game";

        var gameRoot = new GameObject("GameRoot");
        var flipRoot = new GameObject("FlipRoot"); flipRoot.transform.SetParent(gameRoot.transform);

        var camGO = new GameObject("Main Camera");
        camGO.transform.SetParent(gameRoot.transform);
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true; cam.orthographicSize = 5f;
        camGO.tag = "MainCamera"; camGO.transform.position = new Vector3(0, 0, -10);

        var bg = new GameObject("Background"); bg.transform.SetParent(flipRoot.transform);
        var bgSR = bg.AddComponent<SpriteRenderer>(); bgSR.sprite = MakeSolidSprite(32, 32, new Color(0.11f, 0.2f, 0.36f, 1f));
        bg.transform.localScale = new Vector3(20f, 12f, 1f); bgSR.sortingOrder = -3;

        var brick = new GameObject("Brick"); brick.transform.SetParent(flipRoot.transform);
        var brickSR = brick.AddComponent<SpriteRenderer>(); brickSR.sprite = MakeSolidSprite(16, 16, new Color(0.66f, 0.49f, 0.32f, 1f));
        brick.transform.position = Vector3.zero; brick.transform.localScale = new Vector3(6f, 0.6f, 1f); brickSR.sortingOrder = -1;

        var lanes = new GameObject("Lanes"); lanes.transform.SetParent(brick.transform);
        float topY = 1.2f, midY = 0f, botY = -1.2f;
        CreateLaneLine(lanes.transform, "LaneTop", topY, new Color(0.2f, 0.85f, 0.2f, 0.9f));
        CreateLaneLine(lanes.transform, "LaneMid", midY, new Color(0.9f, 0.9f, 0.9f, 0.6f));
        CreateLaneLine(lanes.transform, "LaneBottom", botY, new Color(0.2f, 0.4f, 0.9f, 0.9f));

        var playerRoot = new GameObject("PlayerRoot"); playerRoot.transform.SetParent(brick.transform);

        var player = new GameObject("Player"); player.transform.SetParent(playerRoot.transform);
        var sr = player.AddComponent<SpriteRenderer>(); sr.sprite = MakeSolidSprite(16, 16, new Color(1f, 0.95f, 0.2f, 1f)); sr.sortingOrder = 0;
        var rb = player.AddComponent<Rigidbody2D>(); rb.bodyType = RigidbodyType2D.Kinematic; player.AddComponent<BoxCollider2D>();

        var hitTop = new GameObject("HitZone_Top"); hitTop.transform.SetParent(playerRoot.transform);
        hitTop.transform.localPosition = new Vector3(0, topY, 0); var ct = hitTop.AddComponent<CircleCollider2D>(); ct.isTrigger = true; ct.radius = 0.25f;
        var hitMid = new GameObject("HitZone_Mid"); hitMid.transform.SetParent(playerRoot.transform);
        hitMid.transform.localPosition = new Vector3(0, midY, 0); var cm = hitMid.AddComponent<CircleCollider2D>(); cm.isTrigger = true; cm.radius = 0.25f;
        var hitBot = new GameObject("HitZone_Bottom"); hitBot.transform.SetParent(playerRoot.transform);
        hitBot.transform.localPosition = new Vector3(0, botY, 0); var cb = hitBot.AddComponent<CircleCollider2D>(); cb.isTrigger = true; cb.radius = 0.25f;

        var pbc = playerRoot.AddComponent<PlayerBrickController>();
        pbc.jumpHeight = 1.4f; pbc.jumpUpTime = 0.25f; pbc.jumpDownTime = 0.2f; pbc.underOffset = 0.7f;
        pbc.hitTop = ct; pbc.hitMid = cm; pbc.hitBottom = cb;

        var dash = gameRoot.AddComponent<DashInverter>(); dash.worldRoot = flipRoot.transform; dash.dashDuration = 3f; dash.cooldown = 6f;
        var dir = gameRoot.AddComponent<DirectionManager>(); dir.switchEverySeconds = 10f;

        var itemsRoot = new GameObject("ItemsRuntime"); itemsRoot.transform.SetParent(flipRoot.transform);
        var spawners = new GameObject("Spawners"); spawners.transform.SetParent(flipRoot.transform);
        var spawner = spawners.AddComponent<ItemSpawner>();
        spawner.laneTop = lanes.transform.Find("LaneTop"); spawner.laneMid = lanes.transform.Find("LaneMid"); spawner.laneBottom = lanes.transform.Find("LaneBottom");
        spawner.itemsParent = itemsRoot.transform;
        spawner.strawPF = MakeItemPF("Straw_PF", new Color(1f, 0.9f, 0.1f, 1f), ItemType.Straw, false);
        spawner.rockPF = MakeItemPF("Rock_PF", new Color(0.5f, 0.5f, 0.5f, 1f), ItemType.Rock, true);
        spawner.datePF = MakeItemPF("Date_PF", new Color(0.7f, 0.3f, 0.2f, 1f), ItemType.Date, false);
        spawner.coinPF = MakeItemPF("Coin_PF", new Color(0.95f, 0.85f, 0.1f, 1f), ItemType.Coin, true);

        var gm = gameRoot.AddComponent<GameController>(); gm.spawner = spawner; gm.targetStraw = 14; gm.hearts = 3; gm.roundSeconds = 40f;

        var canvasGO = new GameObject("Canvas"); canvasGO.transform.SetParent(gameRoot.transform);
        var canvas = canvasGO.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>(); canvasGO.AddComponent<GraphicRaycaster>();

        var strawText = CreateUIText(canvas.transform, "StrawText", "🌾 0/14", new Vector2(10, -10), TextAnchor.UpperLeft);
        var heartsText = CreateUIText(canvas.transform, "HeartsText", "❤ 3", new Vector2(10, -40), TextAnchor.UpperLeft);
        var timerText = CreateUIText(canvas.transform, "TimerText", "40s", new Vector2(-10, -10), TextAnchor.UpperRight);

        var flipBtn = CreateUIButton(canvas.transform, "FLIP!", new Vector2(-120, 30), new Vector2(160, 40));
        var btn = flipBtn.GetComponent<Button>(); btn.onClick.AddListener(() => dash.OnDashButtonPressed());

        var winPanel = CreatePanel(canvas.transform, "WIN!", new Color(0, 0.6f, 0, 0.75f)); winPanel.SetActive(false);
        var losePanel = CreatePanel(canvas.transform, "LOSE!", new Color(0.6f, 0, 0, 0.75f)); losePanel.SetActive(false);

        var hud = canvasGO.AddComponent<HUDController>();
        hud.strawText = strawText.GetComponent<Text>();
        hud.heartsText = heartsText.GetComponent<Text>();
        hud.timerText = timerText.GetComponent<Text>();
        hud.controller = gm;
        hud.winPanel = winPanel;
        hud.losePanel = losePanel;

        System.IO.Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Game.unity");
        EditorUtility.DisplayDialog("Full Prototype", "Scene created at Assets/Scenes/Game.unity\nPlay to test!", "OK");
    }

    static Sprite MakeSolidSprite(int w, int h, Color c)
    {
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        var px = new Color[w * h]; for (int i = 0; i < px.Length; i++) px[i] = c; tex.SetPixels(px); tex.Apply();
        var rect = new Rect(0, 0, w, h); var pivot = new Vector2(0.5f, 0.5f);
        return Sprite.Create(tex, rect, pivot, w);
    }
    static void CreateLaneLine(Transform parent, string name, float y, Color color)
    {
        var go = new GameObject(name); go.transform.SetParent(parent); go.transform.localPosition = new Vector3(0, y, 0);
        var lr = go.AddComponent<LineRenderer>(); lr.positionCount = 2; lr.useWorldSpace = false; lr.widthMultiplier = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default")); lr.startColor = lr.endColor = color; lr.sortingOrder = -2;
        lr.SetPosition(0, new Vector3(-10f, 0, 0)); lr.SetPosition(1, new Vector3(10f, 0, 0));
    }
    static GameObject MakeItemPF(string name, Color c, ItemType type, bool addScaler)
    {
        var go = new GameObject(name);
        var sr = go.AddComponent<SpriteRenderer>(); sr.sprite = MakeSolidSprite(12, 12, c);
        var col = go.AddComponent<BoxCollider2D>(); col.isTrigger = true;
        var mover = go.AddComponent<ItemMover>(); mover.type = type;
        if (addScaler) { var hs = go.AddComponent<HazardScaler>(); hs.myType = type; }
        return go;
    }
    static GameObject CreateUIText(Transform parent, string name, string text, Vector2 anchored, TextAnchor anchor)
    {
        var go = new GameObject(name); go.transform.SetParent(parent);
        var rt = go.AddComponent<RectTransform>(); rt.anchorMin = anchor == TextAnchor.UpperLeft ? new Vector2(0, 1) : new Vector2(1, 1);
        rt.anchorMax = rt.anchorMin; rt.pivot = new Vector2(0.5f, 0.5f); rt.anchoredPosition = anchored; rt.sizeDelta = new Vector2(160, 28);
        var txt = go.AddComponent<Text>(); txt.text = text; txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.alignment = anchor; txt.fontSize = 22; txt.color = Color.white; return go;
    }
    static GameObject CreateUIButton(Transform parent, string label, Vector2 anchored, Vector2 size)
    {
        var go = new GameObject("Button"); go.transform.SetParent(parent);
        var rt = go.AddComponent<RectTransform>(); rt.anchorMin = rt.anchorMax = new Vector2(1, 0); rt.pivot = new Vector2(1, 0);
        rt.anchoredPosition = anchored; rt.sizeDelta = size;
        var img = go.AddComponent<Image>(); img.color = new Color(1, 1, 1, 0.9f);
        var btn = go.AddComponent<Button>();
        var labelGO = new GameObject("Text"); labelGO.transform.SetParent(go.transform);
        var rt2 = labelGO.AddComponent<RectTransform>(); rt2.anchorMin = rt2.anchorMax = rt2.pivot = new Vector2(0.5f, 0.5f); rt2.sizeDelta = size;
        var txt = labelGO.AddComponent<Text>(); txt.text = label; txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.alignment = TextAnchor.MiddleCenter; txt.fontSize = 20; txt.color = Color.black;
        return go;
    }
    static GameObject CreatePanel(Transform parent, string label, Color c)
    {
        var go = new GameObject(label + "Panel"); go.transform.SetParent(parent);
        var rt = go.AddComponent<RectTransform>(); rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        var img = go.AddComponent<Image>(); img.color = c;
        var textGO = new GameObject("Label"); textGO.transform.SetParent(go.transform);
        var tr = textGO.AddComponent<RectTransform>(); tr.anchorMin = tr.anchorMax = tr.pivot = new Vector2(0.5f, 0.5f); tr.sizeDelta = new Vector2(300, 100);
        var txt = textGO.AddComponent<Text>(); txt.text = label; txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.alignment = TextAnchor.MiddleCenter; txt.fontSize = 64; txt.color = Color.white;
        return go;
    }
}
#endif
