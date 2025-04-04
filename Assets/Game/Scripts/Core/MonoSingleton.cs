using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;
    private static readonly object _lock = new();
    private static readonly string MonoSingletonRoot = "MonoSingletonRoot";

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock) // 确保线程安全
                {
                    if (_instance == null) // 双重检查锁定
                    {
                        _instance = FindAnyObjectByType<T>();
                        if (_instance == null)
                        {
                            // 创建根 GameObject
                            GameObject rootObject = GameObject.Find(MonoSingletonRoot);
                            if (rootObject == null)
                            {
                                rootObject = new GameObject(MonoSingletonRoot);
                                DontDestroyOnLoad(rootObject); // 防止场景切换时销毁
                            }

                            // 创建单例对象并挂载到根 GameObject 下
                            GameObject singletonObject = new(typeof(T).Name);
                            singletonObject.transform.SetParent(rootObject.transform);
                            _instance = singletonObject.AddComponent<T>();
                        }
                    }
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// 子类禁止直接使用Awak，使用Init钩子函数
    /// </summary>
    protected virtual void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this as T;
        DontDestroyOnLoad(gameObject);
        OnInit();
    }

    protected virtual void OnInit() { }
}
