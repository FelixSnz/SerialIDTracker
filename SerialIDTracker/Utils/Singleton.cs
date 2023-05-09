
using Cognex.DataMan.SDK;

public class Singleton
{
    private static Singleton _instance;

    private static readonly object _syncRoot = new object();


    private static ISystemConnector _connector = null;
    private static DataManSystem _system = null;
    private static bool _closing = false;


    public ISystemConnector Connector
    {
        set
        {
            _connector = value;

        }
        get { return _connector; }
    }

    public DataManSystem System
    {
        set { _system = value; }
        get { return _system; }
    }

    public bool IsClosing
    {
        set { _closing = value; }
        get { return _closing; }
    }




    private Singleton()
    {
        // Replace the connection string with your own Access database connection string.


        //_connection = new OleDbConnection(connectionString);
    }

    public static Singleton Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_syncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new Singleton();
                    }
                }
            }
            return _instance;
        }
    }


}