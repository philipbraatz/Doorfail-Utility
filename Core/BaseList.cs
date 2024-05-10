using Doorfail.Core.Extensions;
using System.Data.Entity;
using System.Reflection;

namespace Doorfail.Core.Data
{
    public class BaseList<Tcrud, TEntity, Tid> : List<Tcrud>
           where TEntity : Entity<Tid>//database table
           where Tcrud : DataModel<TEntity, Tid>//BL version
    {
        protected PropertyInfo[] properties { get; set; }

        public BaseList()
        {
            Type type = typeof(TEntity);
            properties = type.GetProperties();
        }

        //public AbsList(bool loadAll) {
        //    Type type = typeof(TEntity);
        //    properties = type.GetProperties();
        //}

        //TODO needs testing
        public void LoadAll(DbSet<TEntity> table)
        {
            this.Clear();
            if (table.ToList().Count != 0)
                foreach (var c in table.ToList())
                {
                    Tcrud tinstance = (Tcrud)Activator.CreateInstance(typeof(Tcrud), new object[] { });
                    typeof(Tcrud).GetConstructor(new Type[] { typeof(TEntity) }).Invoke(tinstance, new object[] { c });//create a BL version of the PL class
                    base.Add(tinstance);//converting problem
                }
        }
    }

    public class SingleSortList<Tcrud, TEntity, TId> : BaseList<Tcrud, TEntity, TId>
    where TEntity : Entity<TId>
    where Tcrud : DataModel<TEntity, TId>
    {
        private object sortID { get; set; }
        private int sortCol { get; set; }

        //permanently delete entries
        public void DeleteAllMatching(DbSet<TEntity> table)
        { throw new NotImplementedException(); }

        //keep all that match filter
        public void KeepMatching(DbSet<TEntity> table, object matchingID)
        { throw new NotImplementedException(); }

        //remove all that match filter
        public void RemoveMatching(DbSet<TEntity> table, object matchingID)
        { throw new NotImplementedException(); }

        public void Add(DbContext dc, DbSet<TEntity> table, TEntity entry)
        { throw new NotImplementedException(); }

        public void Remove(DbContext dc, DbSet<TEntity> table, TEntity entry)
        { throw new NotImplementedException(); }
    }

    public class AbsListJoin<Tcrud, TEntity, TForignEntity, Tid, Fid> : BaseList<Tcrud, TEntity, Tid>
        where Tcrud : DataModel<TEntity, Tid>
        where TEntity : Entity<Tid>
        where TForignEntity : ForignEntity<Fid, Tid>
    {
        protected object joinGroupingID;
        private string joinForeignIDname;
        private string joinGroupingIDname;
        private PropertyInfo[] joinTable_Properties { get; set; }

        //does not load list without calling load
        public AbsListJoin(string GroupingIDname, object GroupingID, string foreignIDname)
        {
            Type type = typeof(Tcrud);
            properties = type.GetProperties();

            Type typejoin = typeof(TForignEntity);
            joinTable_Properties = typejoin.GetProperties();
            joinGroupingID = GroupingID;
            joinForeignIDname = foreignIDname;
            joinGroupingIDname = GroupingIDname;
        }

        public new void Clear()
        {
            joinGroupingID = null;
            base.Clear();
        }

        //TODO reimpliment LoadWithJoin using template below
        //PRES
        public void LoadWithJoin(DbSet<TEntity> entities, DbSet<TForignEntity> joinTable,
                                 object joinid)
        {
            ///This is what this code is doing
            ///memberID = member_id;
            ///if (dc.MemberActions.ToList().Count != 0)
            ///    foreach (var col in dc.MemberActions)
            ///        if (col.MemberActionMember_ID == memberID)
            ///            foreach (var entry in dc.HelpingActions)
            ///                if (entry.HelpingActionID == col.MemberActionActionID)
            ///                    base.Add(new AbsHelpingAction(entry));

            ///Actual implimentation
            //this.Clear();
            //joinTable_ID = join_id;
            //if (entities.ToList().Count != 0)
            //    foreach (var col in join_table)
            //    {
            //        //instance = where( entry in the table == this ID)
            //        object colJoin_ID = Utils.getValue(col, joinTable_ID_Prop_Name);
            //        object colentity_ID = Utils.getValue(col, joinProp_name);
            //        if (joinTable_ID.Equals(colJoin_ID))
            //        {
            //            foreach (var entry in entities)
            //            {
            //                try
            //                {
            //                    //Converting problem
            //                    Tcrud crud = ColumnEntry<TEntity>.ConvertToBL<Tcrud>(entry);//from Tentity to Tcrud
            //                    if (colentity_ID.Equals(Utils.getValue(entry, properties[0].Name)))
            //                        base.Add(crud);

            //                }
            //                catch (Exception e)
            //                {
            //
            //                    throw new Exception("AbstractionCRUD LoadWithJoin Adding "+
            //                        entry.GetType().Name+" as ("+typeof(Tcrud)+"): "+e.Message);
            //                }
            //            }
            //        }
            //    }
        }

        public void DeleteAllPreferences(DbContext dc, DbSet<TForignEntity> joinTable)
        {
            foreach (var col in joinTable)
                if (joinGroupingID.Equals(col.GetValue<TForignEntity>(joinGroupingIDname)))
                    joinTable.Remove(col);
            dc.SaveChanges();
            this.Clear();
        }

        public void Add(DbContext dc, DbSet<TForignEntity> joinTable, TForignEntity joinInstance, Tcrud entry)
        {
            //Set new ID based on type
            if (joinTable_Properties[0].GetValue(joinInstance) is Guid)
                joinInstance.SetValue<TForignEntity>(joinTable_Properties[0].Name, Guid.NewGuid());//instance_PK = newID using ("join_Property_ID")
            else if (joinTable_Properties[0].GetValue(joinInstance) is int)//gets type int
            {
                int newID = 0;

                //get new id if int
                if (joinTable.ToList().Count != 0)
                {
                    int max = 0;
                    foreach (var t in joinTable)
                    {
                        int comp = (int)t.GetValue<TForignEntity>(joinTable_Properties[0].Name);
                        if (comp > max)
                            max = comp;
                    }
                    newID = max + 1;
                }
                //set primary key
                joinInstance.SetValue<TForignEntity>(joinTable_Properties[0].Name, newID);//instance_PK = newID using ("join_Property_ID")
            }
            //if its not a guid or int the ID has to be pre-set before adding

            //set ID for group
            joinInstance.SetValue<TForignEntity>(joinGroupingIDname, joinGroupingID);//instance_Grouping_ID = join_Grouping_ID using ("join_Grouping_ID")
            //set ID of entry being added
            joinInstance.SetValue<TForignEntity>(joinForeignIDname,           //instance_FK using("join_FK") =
                entry.GetValue<Tcrud>(properties[0].Name)); // entry_ID using("entry_ID")

            joinTable.Add(joinInstance);//add
            dc.SaveChanges();
            base.Add(entry);
        }

        public void Remove(DbContext dc, DbSet<TForignEntity> joinTable, Tcrud entry)
        {
            foreach (var join in joinTable)
            {
                if (joinGroupingID.Equals(join.GetValue<TForignEntity>(joinGroupingIDname)) &&//ID for this group matches
                    entry.GetValue<Tcrud>(properties[0].Name).Equals(
                        join.GetValue<TForignEntity>(joinForeignIDname))
                    )//ID for this entry matches
                    joinTable.Remove(join);
            }
            dc.SaveChanges();
            base.Remove(entry);
        }
    }
}