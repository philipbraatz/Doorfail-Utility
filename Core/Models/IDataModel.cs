namespace Doorfail.Core.Models
{
    public interface IDataModel<TId>
    {
        bool Active { get; set; }
        TId CreatedBy { get; set; }
        DateTime CreatedDate { get; set; }
        TId ID { get; set; }
        TId UpdatedBy { get; set; }
        DateTime UpdatedDate { get; set; }

        void Clear();

        int Delete();

        int Insert();

        int Update();

        void Load();

        void Load(TId id);

        string ToString();
    }
}