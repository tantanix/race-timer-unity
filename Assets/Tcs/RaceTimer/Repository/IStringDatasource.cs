namespace Assets.Tcs.RaceTimer.Repository
{
    public interface IStringDatasource
    {
        float GetFloat(string key, float defaultValue);
        float GetFloat(string key);
        int GetInt(string key, int defaultValue);
        int GetInt(string key);
        string GetString(string key, string defaultValue);
        string GetString(string key);
        bool HasKey(string key);
        void SetFloat(string key, float value);
        void SetInt(string key, int value);
        void SetString(string key, string value);
        void DeleteAll();
        void DeleteKey(string key);
    }
}
