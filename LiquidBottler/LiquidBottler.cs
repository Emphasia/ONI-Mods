using System;
using UnityEngine;

public class LiquidBottler : Workable
{
    public Storage storage;

    private LiquidBottler.Controller.Instance smi;

    public bool dropWhenFull = false;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        storedAmount = storage.MassStored();
        base.Subscribe((int)GameHashes.RefreshUserMenu, new Action<object>(OnRefreshUserMenuDelegate));
        this.smi = new LiquidBottler.Controller.Instance(this);
        this.smi.StartSM();
        UpdateStoredItemState();
    }

    protected override void OnCleanUp()
    {
        if (this.smi != null)
        {
            this.smi.StopSM("OnCleanUp");
        }
        base.OnCleanUp();
    }

    private void UpdateStoredItemState()
    {
        foreach (GameObject go in storage.items)
        {
            if (go != null)
            {
                go.Trigger((int)GameHashes.OnStorageInteracted, storage);
            }
        }
    }

    public void OnRefreshUserMenuDelegate(object _)
    {
        Game.Instance.userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo(
            iconName: "action_building_disabled",
            text: dropWhenFull ? "Disable Auto-Drop" : "Enable Auto-Drop",
            tooltipText: dropWhenFull ? "Stop this building from filling when full" : "Allow this building to drop bottles once full",
            on_click: new System.Action(() =>
                {
                    dropWhenFull = !dropWhenFull;
                    smi.GoTo(smi.sm.GetDefaultState());
                })
            )
        );
    }

    protected float storedAmount;
    public enum StorageChangeType { increased, decreased, unchanged };

    public StorageChangeType OnStorageChangedDelegate()
    {
        StorageChangeType storageChangeType;
        if (storage.MassStored() > storedAmount)
            storageChangeType = StorageChangeType.increased;
        else if (storage.MassStored() < storedAmount)
            storageChangeType = StorageChangeType.decreased;
        else storageChangeType = StorageChangeType.unchanged;

        storedAmount = storage.MassStored();
        return storageChangeType;
    }

    private class Controller : GameStateMachine<Controller, Controller.Instance, LiquidBottler>
    {
        public State idle, empty, filling, full, picking;

        public override void InitializeStates(out BaseState default_state)
        {
            default_state = idle;
            idle
                //.ToggleStatusItem("idle", "idle")
                //.PlayAnim("on")
                .EventHandler(GameHashes.OnStorageChange, smi => {
                    switch (smi.master.OnStorageChangedDelegate())
                    {
                        case StorageChangeType.increased:
                            smi.GoTo(filling); break;
                        case StorageChangeType.decreased:
                            smi.GoTo(picking); break;
                    }
                })
                //.EventTransition(GameHashes.OnStorageChange, filling, smi => smi.master.storageChangeType == StorageChangeType.increased)
                //.EventTransition(GameHashes.OnStorageChange, picking, smi => smi.master.storageChangeType == StorageChangeType.decreased)
                .EnterTransition(full, smi => smi.master.storage.IsFull())
                .EnterTransition(empty, smi => smi.master.storage.IsEmpty());
            filling
                //.ToggleStatusItem("filling", "filling")
                //.PlayAnim("working")
                .OnAnimQueueComplete(idle);
                //.ScheduleGoTo(2f, idle);
            full
                //.ToggleStatusItem("full", "full")
                //.EventTransition(GameHashes.OnStorageChange, picking, smi => smi.master.storageChangeType == StorageChangeType.decreased)
                .EventHandler(GameHashes.OnStorageChange, smi => {
                    switch (smi.master.OnStorageChangedDelegate())
                    {
                        case StorageChangeType.decreased:
                            smi.GoTo(picking); break;
                    }
                })
                .EnterTransition(idle, smi => {
                    if (smi.master.dropWhenFull)
                    {
                        smi.master.storage.DropAll(false, false, default, true);
                        smi.master.storedAmount = 0;
                        return true;
                    }
                    return false;
                });
            picking
                //.ToggleStatusItem("picking", "picking")
                //.PlayAnim("pick_up")
                .OnAnimQueueComplete(idle);
                //.ScheduleGoTo(1f, idle);
            empty
                //.ToggleStatusItem("empty", "empty")
                //.PlayAnim("off")
                .EventTransition(GameHashes.OnStorageChange, filling, smi => !smi.master.storage.IsEmpty());
        }

        public new class Instance : GameStateMachine<LiquidBottler.Controller, LiquidBottler.Controller.Instance, LiquidBottler, object>.GameInstance
        {
            public Instance(LiquidBottler master) : base(master) { }
        }
    }
}
