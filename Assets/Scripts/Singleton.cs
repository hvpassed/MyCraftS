public class Singleton<T> where T: class, new()
{
    private static T _instance=null;
    private static object _lock=new object();

    public static T Instance
    {
        get
        {
            if (_instance == null) {
                lock (_lock)
                {
                    if (_instance == null) _instance=new T();

                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static T GetInstance()
    {
        
        return Instance;
    }


}
 