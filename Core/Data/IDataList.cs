namespace Doorfail.Core.Data;

public interface IDataList
{
    int DeleteAll();

    int InsertAll();

    void LoadAll();

    int UpdateAll();
}