using System;
using System.Collections.Generic;
using EugeneC.Mono;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

namespace EugeneC.Singleton {
    
    public abstract class GenericUiManager<TEnum, TMono> : GenericPoolingManager<TEnum, UiHelper, TMono> 
        where TEnum : Enum
        where TMono : MonoBehaviour{
        
        [SerializeField] protected Canvas canvasRef;

        protected RectTransform CanvasPos => canvasRef.transform as RectTransform;
        private List<UiHelper> _openedUi;
        [HideInInspector] public bool isTransitioning;
        
        public event Action OnOpenUi;
        public event Action OnCloseUi;
        
        protected override async void Start() {
            try {
                await Awaitable.NextFrameAsync(Token);
                if (canvasRef is null) throw new Exception("Canvas is not set");

                Pools = new ObjectPool<UiHelper>[poolPrefabs.Length];
                RuntimePools = new RuntimePoolSerialize[poolPrefabs.Length];

                for (var i = 0; i < Pools.Length; i++) {
                    if (poolPrefabs[i].prefab is null) continue;
                    
                    RuntimePools[i].spawn = new UiHelper[poolCount];
                    var i1 = i;
                    Pools[i] = InitPool(() => {
                        var spawn = Instantiate(poolPrefabs[i1].prefab, CanvasPos);
                        spawn.OnSpawn();
                        RuntimePools[i1].spawn[0] = spawn;
                        spawn.gameObject.SetActive(false);
                        return spawn;
                    });
                    
                    for (var j = 0; j < poolCount; j++) {
                        var spawnUi = Pools[0].Get();
                        RuntimePools[0].spawn[j] = spawnUi;
                    }
                }
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }

        protected virtual void OnEnable() {
            _openedUi = ListPool<UiHelper>.Get();
        }

        protected override async void OnDisable() {
            try {
                await CloseAll();
                ListPool<UiHelper>.Release(_openedUi);
                base.OnDisable();
            }
            catch (Exception e) {
                print(e);
            }
        }
        
        public virtual async Awaitable<(UiHelper, bool)> Open(TEnum id) {
            var index = GetPoolIndex(id);
            if (index == -1) return (null, false);

            isTransitioning = true;
            var newUi = RuntimePools[index].spawn[0];
            newUi.gameObject.SetActive(true);
            _openedUi.Add(newUi);

            OnOpenUi?.Invoke();
            var t = newUi.OnStartOpen();
            await Awaitable.WaitForSecondsAsync(math.abs(t), Token);
            newUi.OnEndOpen();
            isTransitioning = false;

            return (newUi, true);
        }

        public virtual async Awaitable<(UiHelper, bool)> Close(TEnum id, float time) {
            var index = GetPoolIndex(id);
            if (index == -1) return (null, false);

            isTransitioning = true;
            var newUi = RuntimePools[index].spawn[0];

            OnCloseUi?.Invoke();
            var t = newUi.OnStartClose();
            await Awaitable.WaitForSecondsAsync(math.abs(t), Token);
            newUi.OnEndClose();

            await Awaitable.NextFrameAsync();
            newUi.gameObject.SetActive(false);
            _openedUi.Remove(newUi);
            isTransitioning = false;

            return (newUi, true);
        }

        public virtual async Awaitable<bool> CloseAll() {
            isTransitioning = true;
            var i = 0f;
            foreach (var ui in _openedUi) {
                OnCloseUi?.Invoke();
                var t = ui.OnStartClose();
                //Get the highest value and delay the said value
                i = i < t ? t : i;
            }

            await Awaitable.WaitForSecondsAsync(i, Token);

            foreach (var ui in _openedUi) {
                ui.OnEndClose();
                await Awaitable.NextFrameAsync(Token);
                ui.gameObject.SetActive(false);
            }

            isTransitioning = false;
            _openedUi.Clear();
            return true;
        }

        public virtual async Awaitable<bool> Replace(TEnum id) {
            var c = await CloseAll();
            if (!c) return false;
            await Open(id);
            return true;
        }
    }
}