using System;
using UnityEngine;

public class LiquidBottler : Workable
{
    public Storage storage;

    private LiquidBottler.Controller.Instance smi;

    protected override void OnSpawn()
    {
        base.OnSpawn();
        this.smi = new LiquidBottler.Controller.Instance(this);
        this.smi.StartSM();
        this.UpdateStoredItemState();
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
        //this.storage.allowItemRemoval = (this.smi != null && this.smi.GetCurrentState() == this.smi.sm.ready);
        foreach (GameObject gameObject in this.storage.items)
        {
            if (gameObject != null)
            {
                gameObject.Trigger((int)GameHashes.OnStorageInteracted, this.storage);
            }
        }
    }

    private class Controller : GameStateMachine<LiquidBottler.Controller, LiquidBottler.Controller.Instance, LiquidBottler>
    {
        public override void InitializeStates(out StateMachine.BaseState default_state)
        {
            default_state = this.empty;
            this.empty.PlayAnim("off").EventTransition(GameHashes.OnStorageChange, this.filling, (LiquidBottler.Controller.Instance smi) => smi.master.storage.IsFull());
            this.filling.PlayAnim("working").OnAnimQueueComplete(this.ready);
            this.ready.EventTransition(GameHashes.OnStorageChange, this.pickup, (LiquidBottler.Controller.Instance smi) => !smi.master.storage.IsFull())
                .Enter(delegate (LiquidBottler.Controller.Instance smi)
                {
                    //smi.master.storage.allowItemRemoval = true;
                    foreach (GameObject go in smi.master.storage.items)
                    {
                        go.Trigger((int)GameHashes.OnStorageInteracted, smi.master.storage);
                    }
                })
                .Exit(delegate (LiquidBottler.Controller.Instance smi)
                {
                    //smi.master.storage.allowItemRemoval = false;
                    foreach (GameObject go in smi.master.storage.items)
                    {
                        go.Trigger((int)GameHashes.OnStorageInteracted, smi.master.storage);
                    }
                });
            this.pickup.PlayAnim("pick_up").OnAnimQueueComplete(this.empty);
        }

        public GameStateMachine<LiquidBottler.Controller, LiquidBottler.Controller.Instance, LiquidBottler, object>.State empty;
        public GameStateMachine<LiquidBottler.Controller, LiquidBottler.Controller.Instance, LiquidBottler, object>.State filling;
        public GameStateMachine<LiquidBottler.Controller, LiquidBottler.Controller.Instance, LiquidBottler, object>.State ready;
        public GameStateMachine<LiquidBottler.Controller, LiquidBottler.Controller.Instance, LiquidBottler, object>.State pickup;

        public new class Instance : GameStateMachine<LiquidBottler.Controller, LiquidBottler.Controller.Instance, LiquidBottler, object>.GameInstance
        {
            public Instance(LiquidBottler master) : base(master)
            {
            }
        }
    }
}
