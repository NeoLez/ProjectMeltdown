using System;
using UnityEngine;

namespace Root.Factories {
    public abstract class ItemStateInitializer : ScriptableObject {
        public abstract Type StateType { get; }
        public abstract void Apply(ItemState state, in ItemSpawnContext ctx);
        public abstract bool SupportsType(ItemState state);
    }

    public abstract class ItemStateInitializer<TState> : ItemStateInitializer where TState : ItemState {
        public sealed override Type StateType => typeof(TState);

        public sealed override void Apply(ItemState state, in ItemSpawnContext ctx) {
            if (state is not TState typed) {
                Debug.LogError($"{name} expects {typeof(TState).Name} but got {state?.GetType().Name} ({state?.ItemSo?.name})", this);
                return;
            }
            ApplyTo(typed, ctx);
        }
        
        protected abstract void ApplyTo(TState state, in ItemSpawnContext ctx);
        
        public sealed override bool SupportsType(ItemState state) => state is TState;
    }
}