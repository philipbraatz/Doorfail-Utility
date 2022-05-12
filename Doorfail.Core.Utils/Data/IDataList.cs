namespace Doorfail.Core.Utils.Data
{
    public interface IDataList
    {
        int DeleteAll();
        int InsertAll();
        void LoadAll();
        int UpdateAll();
    }
}