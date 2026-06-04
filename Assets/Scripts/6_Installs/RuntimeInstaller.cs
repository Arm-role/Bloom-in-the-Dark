using UnityEngine;

public class RuntimeInstaller
{
  public void Install(DIContainerBase container, GameSceneInstaller scene)
  {
    // =======================
    // Modal UI stack — สร้างก่อนทุกตัวที่ implement IModalUI (Inventory/Hint/Pause/Trade/Upgrade)
    // wire events (timeScale, OnDismiss) อยู่ด้านล่างหลัง stateMachine fetch
    // =======================
    var modalStack = new ModalUIStack();

    // =======================
    // Spawn
    // =======================

    var pool = container.Get<IAdressablePoolService<GameObject>>();

    var spawner = new GameObjectSpawner(pool, scene.Scriptable.GameObjectLibrary);
    var spawnerHandle = new SpawnerHandle(spawner);
    var particle = new ParticalService(pool, scene.Scriptable.ParticleLibrary);


    var floatTextPool = new AdressablePoolingService("[Damage_Root]");

    var floatingTextService = new FloatingTextService(
        floatTextPool,
        scene.Scriptable.FloatingTextConfig.Prefab,
        scene.Scriptable.FloatingTextConfig.Styles
     );

    // =======================
    // Tag
    // =======================

    TagLibrary.Initialize(scene.Scriptable.TagLibrary);

    var tooltipservice = new TooltipService(scene.TooltipView);

    // =======================
    // Progression
    // =======================

    var thresholdService = new TagUpgradeThresholdService(scene.Scriptable.UpgradeThresholdConfigs);
    var upgradeContainer = new GlobalUpgradeDomain(thresholdService);

    var phaseStatService = new PhaseStatService(
      scene.Scriptable.PhaseStatConfig,
      scene.Scriptable.StatDatabase,
      upgradeContainer
      );

    var playerProgession = new PlayerProgression(
      scene.Scriptable.PhaseStatConfig.LevelStart);

    // =======================
    // Audio
    // =======================
    // AudioBootstrap (DontDestroyOnLoad MonoBehaviour) ต้องอยู่ใน boot scene ก่อน
    // ถ้า scene ไม่มี → audioService = null → downstream ใช้ Get<IAudioService>() จะได้ null + warning
    var audioService = AudioBootstrap.Service;

    // =======================
    // Item
    // =======================

    var itemFactory = new ItemFactory(
      scene.Scriptable.ItemDatabase,
      scene.Scriptable.StatDatabase,
      upgradeContainer
      );

    // =======================
    // Player
    // =======================

    var state = new PlayerState(FacingDirection.Right);

    float maxHP = phaseStatService.GetStat(scene.Scriptable.StatDatabase.MaxHp);
    float maxEnegy = phaseStatService.GetStat(scene.Scriptable.StatDatabase.MaxEnergy);

    var health = new PlayerHealth(maxHP);
    var playerEnergy = new PlayerEnergy(maxEnegy);

    // =======================
    // Animation
    // =======================

    var playerAnimationTagService = new CharacterAnimationTagService(scene.Scriptable.CharacterAnimationConfig);

    var playerAnimationSystem = new CharacterAnimationSystem(
      scene.Scriptable.AnimationLibrary);

    // =======================
    // Cooldown
    // =======================

    var timeSource = new UnityTimeSource();
    var actionLock = new PlayerActionLock(timeSource);
    var cooldownService = new CooldownService(timeSource);

    var playerCooldown = cooldownService.GetOrCreate(scene.PlayerController);

    // =======================
    // Inventory
    // =======================

    var hotbarState = new HotbarState(scene.Scriptable.HotbarSize);

    var hotbarLogic = new InventoryLogic(scene.Scriptable.HotbarSize);
    var mainInventoryLogic = new InventoryLogic(scene.Scriptable.InventorySize);

    var inventory = new PlayerInventory(
      itemFactory.Create(scene.MockSettings.EmptyItem),
      hotbarState,
      hotbarLogic,
      mainInventoryLogic
    );

    var inventoryService = new InventoryService(
      inventory,
      audioService,
      scene.Scriptable.InventorySoundConfig);

    var inventoryController = new InventoryController(
      scene.HotbarInventoryView,
      scene.MainInventoryView,
      hotbarState,
      inventoryService,
      scene.DragGhost,
      scene.Scriptable.ItemDatabase,
      tooltipservice,
      scene.Scriptable.StatDatabase,
      upgradeContainer,
      scene.Scriptable.InteractionCostConfig
    );

    var inventoryCooldownController = new InventoryCooldownController(
      scene.HotbarInventoryView,
      scene.MainInventoryView,
      inventoryService,
      playerCooldown
    );

    var inventoryScreenController = new InventoryScreenController(
      scene.InventoryUI,
      inventoryController,
      modalStack,
      container.Get<GameStateMachine>(),
      audioService,
      scene.Scriptable.InventorySoundConfig
    );

    var hotbarAudioBinder = new HotbarAudioBinder(
      hotbarState,
      audioService,
      scene.Scriptable.InventorySoundConfig);

    var playerAudioBinder = new PlayerAudioBinder(
      scene.PlayerController,
      audioService,
      scene.Scriptable.CombatSoundConfig);

    // EnemyManager เป็น MonoBehaviour singleton — set ใน Awake ก่อน Install (Start) ทำงาน
    // ถ้า scene ไม่มี EnemyManager (e.g., menu scene) → skip binder
    EnemyAudioBinder? enemyAudioBinder = null;
    if (EnemyManager.Instance != null)
    {
      enemyAudioBinder = new EnemyAudioBinder(
        EnemyManager.Instance,
        audioService,
        scene.Scriptable.CombatSoundConfig);
    }

    // =======================
    // Modal UI stack — wire events (creation อยู่ที่ top ของ Install)
    // =======================
    var stateMachine = container.Get<GameStateMachine>();
    modalStack.OnFirstPausingPush += () => Time.timeScale = 0f;
    modalStack.OnLastPausingPop += () => Time.timeScale = 1f;
    scene.InputRender.OnDismiss += () =>
    {
      if (modalStack.HasAny) { modalStack.RouteDismiss(); return; }
      // stack ว่าง + อยู่ใน Gameplay → เปิด PauseMenu (PauseMenu.Open จะ push ตัวเองลง stack)
      if (stateMachine.CurrentState == EGameState.Gameplay)
        scene.PauseMenuController.Open();
    };

    // =======================
    // Hint (H0 Foundation + H1 Popup + H2 Menu)
    // =======================
    var hintState = new HintState();

    // HintPopupController + HintPopupView (H1) — สร้างถ้า scene มี HintLibrary + HintPopupView
    HintPopupController? hintPopupController = null;
    if (scene.Scriptable.HintLibrary != null && scene.HintPopupView != null)
    {
      hintPopupController = new HintPopupController(
        scene.Scriptable.HintLibrary,
        hintState,
        scene.HintPopupView,
        modalStack);
    }

    // HintMenuController + HintMenuView (C4 book + tabs UI)
    HintMenuController? hintMenuController = null;
    if (scene.Scriptable.HintLibrary != null && scene.HintMenuView != null)
    {
      hintMenuController = new HintMenuController(
        scene.Scriptable.HintLibrary,
        hintState,
        scene.HintMenuView,
        stateMachine,
        modalStack,
        scene.InputRender);
    }

    // HintUnlockBinder (H3) — subscribe game events + trigger welcome popup ตอน gameplay Enter
    HintUnlockBinder? hintUnlockBinder = null;
    if (hintPopupController != null)
    {
      hintUnlockBinder = new HintUnlockBinder(
        scene.Scriptable.HintLibrary!,
        hintState,
        hintPopupController,
        health,
        playerEnergy,
        scene.TurnSystem,
        inventory);
    }

    // =======================
    // Wandering trader — event NPC ที่มาทุก N day (config-driven)
    // skip ถ้า scene ไม่มี config (Optional field)
    // =======================
    WanderingTraderController? wanderingTrader = null;
    if (scene.WanderingTraderConfig != null && scene.BaseBuildingController != null)
    {
      wanderingTrader = new WanderingTraderController(
        scene.TurnSystem,
        scene.EnemySpawner,
        spawnerHandle,
        scene.BaseBuildingController.transform,
        scene.WanderingTraderConfig,
        Camera.main);
    }

    // =======================
    // Interactor
    // =======================

    var interactor = new PlayerInteractor(
      health,
      playerEnergy,
      inventory,
      actionLock,
      playerCooldown);

    // =======================
    // Grid
    // =======================

    var gridConverter = new GridConverter(scene.MainTilemap);
    var zoneManager = new WorldZoneManager();

    // =======================
    // TileMap
    // =======================

    var ctx = new CellActionContext
    (
        scene.Scriptable.TileLibrary
    );

    var factory = new GameActionFactory(ctx);

    var cellActionResolver = new DefaultCellActionResolver(
        factory);

    // =======================
    // Interaction
    // =======================

    var handleService = new InteractionHandleService();

    var executor = new WorldInteractionExecutor(
        spawnerHandle,
        playerProgession,
        itemFactory,
        scene.WorldTileManager,
        inventory,
        audioService,
        scene.Scriptable.InteractionSoundConfig);

    var cellPipeline = new CellInteractionPipeline();

    var strategyFactory = new ItemStrategyFactory(
        scene.WorldTileManager,
        spawnerHandle,
        scene.PlayerController,
        cellPipeline,
        scene.PlacementPreviewController,
        scene.AreaCirclePreview,
        scene.ConePreview,
        scene.AreaLinePreview,
        audioService
    );

    // =======================
    // Item census — รวม PlayerInventory + OfferingAltarController slots ทั้ง scene
    // ใช้สำหรับ LootTable GlobalCap filter
    // =======================
    var offeringAltars = UnityEngine.Object.FindObjectsOfType<OfferingAltarController>();
    var itemCensus = new ItemCensus(inventory, offeringAltars);

    // Initialize handler ที่อยู่ใน scene ตั้งแต่ start (plants/buildings ที่ designer วาง)
    // Runtime-spawned (enemies via SpawnerHandle) ถูก inject ใน GameObjectInitializer.Subscribe
    foreach (var lootable in UnityEngine.Object.FindObjectsOfType<LootableObjectHandler>())
      lootable.Initialize(itemCensus);

    var initializer = new GameObjectInitializer(
        scene.TurnSystem,
        spawnerHandle,
        executor,
        floatingTextService,
        itemCensus);

    var interactionRuntime = new InteractionRuntimeState();
    var costResolver = new InteractionCostResolver(
        scene.Scriptable.InteractionCostConfig,
        interactionRuntime
        );

    var worldHover = new WorldHoverResolver(
      scene.Scriptable.DetectionLayer,
      Camera.main
      );

    var uiHover = scene.UIHoverResolver;

    var dragDropController = new DragDropController(
        scene.InputRender,
        modalStack,
        scene.Scriptable.holdThreshold,
        scene.Scriptable.holdMoveTolerance
        );

    // =======================
    // Upgrade
    // =======================

    var upgradeMediator = new ZoneUpgradeMediator(
      scene.Scriptable.StatDatabase,
      thresholdService,
      phaseStatService,
      zoneManager,
      scene.VFXController,
      scene.Scriptable.ZoneUpgradeConfig
    );

    // =======================
    // Text
    // =======================

    container.Register(pool);

    container.Register(spawner);
    container.Register(spawnerHandle);
    container.Register(particle);

    container.Register(upgradeContainer);
    container.Register(phaseStatService);
    container.Register(playerProgession);

    container.Register(itemFactory);

    container.Register(state);

    container.Register(playerAnimationTagService);
    container.Register(playerAnimationSystem);

    container.Register(playerCooldown);

    container.Register(hotbarState);
    container.Register(inventoryService);
    container.Register(inventory);
    container.Register(inventoryController);
    container.Register(inventoryCooldownController);
    container.Register(inventoryScreenController);
    container.Register(hotbarAudioBinder);
    container.Register(playerAudioBinder);
    if (enemyAudioBinder != null)
      container.Register(enemyAudioBinder);

    container.Register<IHintState>(hintState);
    if (scene.Scriptable.HintLibrary != null)
      container.Register<IHintLibrary>(scene.Scriptable.HintLibrary);
    if (hintPopupController != null)
      container.Register(hintPopupController);
    if (hintMenuController != null)
      container.Register(hintMenuController);
    if (hintUnlockBinder != null)
      container.Register(hintUnlockBinder);
    if (wanderingTrader != null)
      container.Register(wanderingTrader);

    container.Register(health);
    container.Register(playerEnergy);
    container.Register(actionLock);
    container.Register(interactor);

    container.Register(gridConverter);
    container.Register(zoneManager);

    container.Register(factory);
    container.Register(cellActionResolver);

    container.Register(handleService);
    container.Register(strategyFactory);
    container.Register(executor);

    container.Register(initializer);
    container.Register<IItemCensus>(itemCensus);

    container.Register(costResolver);

    container.Register(worldHover);
    container.Register(uiHover);
    container.Register(dragDropController);
    container.Register(modalStack);

    if (audioService != null)
      container.Register<IAudioService>(audioService);
  }
}