namespace Doorfail.Core.Models;

public interface IDataList
{
    int DeleteAll();

    int InsertAll();

    void LoadAll();

    int UpdateAll();
}