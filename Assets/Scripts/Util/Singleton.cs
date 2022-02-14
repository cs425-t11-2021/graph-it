using UnityEngine;

// Class wrapper for Monobehavior that ensures the behavior is a singleton and throws an error
// if the singletong pattern is violated
public abstract class SingletonBehavior<T> : MonoBehaviour where T : MonoBehaviour {
    private static T singleton = null;
    public static T Singleton {
        get => SingletonBehavior<T>.singleton;
        set {
            if (SingletonBehavior<T>.singleton == null) {
                SingletonBehavior<T>.singleton = value;
            }
            else {
                Logger.Log(Logger.SINGLETON_VIOLATION, value, LogType.ERROR);
            }
        }
    }

    protected SingletonBehavior() {
        SingletonBehavior<T>.Singleton = this as T;
    }
}