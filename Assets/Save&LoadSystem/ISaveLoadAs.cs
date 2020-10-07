using Leguar.TotalJSON;

namespace SaveSystem
{
    public interface ISaveLoadAs<T>
    {
        JSON GetSaveData();
        void LoadSaveData(JSON json);
    }
}