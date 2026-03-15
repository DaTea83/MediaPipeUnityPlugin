using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace EugeneC.Singleton {
    
    public abstract class GenericPoolingManager<TEnum, TObj, TMono> : GenericSingleton<TMono>
        where TEnum : Enum
        where TObj : Component
        where TMono : MonoBehaviour {
        
        [Serializable]
        public struct InitialPoolSerialize {
            public TObj prefab;
            public TEnum id;
        }

        [Serializable]
        public struct RuntimePoolSerialize {
            public TObj[] spawn;
            public int currentIndex;
            public int previousIndex;
        }

        [SerializeField] protected InitialPoolSerialize[] poolPrefabs;
        [Tooltip("Determines if you want to spawn all prefabs as child on start")]
        [SerializeField] protected bool initializeOnStart;
        [SerializeField] protected bool collectionCheck = false;
        [SerializeField] protected byte poolCount = 32;
        
        protected ObjectPool<TObj>[] Pools;
        protected RuntimePoolSerialize[] RuntimePools;
        protected List<int> PauseIndexes;
        
        protected virtual async void Start() {
            try {
                await Awaitable.NextFrameAsync(Token);
                Pools = new ObjectPool<TObj>[poolPrefabs.Length];
                RuntimePools = new RuntimePoolSerialize[poolPrefabs.Length];

                for (var i = 0; i < Pools.Length; i++) {
                    if (poolPrefabs[i].prefab is null) continue;
                    
                    RuntimePools[i].spawn = new TObj[poolCount];
                    Pools[i] = InitPool(poolPrefabs[i].prefab);
                    
                    if (!initializeOnStart) continue;
                    for (var j = 0; j < poolCount; j++) {
                        var spawnObj = Pools[i].Get();
                        RuntimePools[i].spawn[j] = spawnObj;
                    }
                }
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }

        protected override void OnDestroy() {
            foreach (var pool in Pools) {
                pool?.Clear();
                pool?.Dispose();
            }
            base.OnDestroy();
        }

        protected virtual int GetPoolIndex(TEnum id) {
            if (!Enum.IsDefined(typeof(TEnum), id)) return -1;
            return Array.FindIndex(poolPrefabs, i => EqualityComparer<TEnum>.Default.Equals(i.id, id));
        }

        protected virtual ObjectPool<TObj> InitPool(Component prefab) {
            return InitPool(() => {
                _ = Instantiate(prefab.gameObject, transform)
                    .TryGetComponent<TObj>(out var component) 
                    ? component 
                    : throw new Exception("Prefab must have a component of type " + typeof(TObj));
                return component;
            });
        }
        
        protected virtual ObjectPool<TObj> InitPool(Func<TObj> createFunc) {
            
            var pool = new ObjectPool<TObj>(
                createFunc: createFunc,
                actionOnGet: obj => obj.gameObject.SetActive(true),
                actionOnRelease: obj => obj.gameObject.SetActive(false),
                actionOnDestroy: Destroy,
                collectionCheck: collectionCheck,
                defaultCapacity: poolPrefabs.Length,
                maxSize: poolCount
            );
            return pool;
        }
    }
}