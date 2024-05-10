using Doorfail.Core.Extensions;
using System.Data.Entity;

namespace Doorfail.Core.Data
{
    public class DataModel<TEntity, TId> : Entity<TId>, IDataModel where TEntity : Entity<TId>
    {

        #region Properties
        public override TId ID
        {
            get => Self.ID;
            set => Self.ID = value;
        }

        public override TId CreatedBy
        {
            get => Self.CreatedBy;
            set => Self.CreatedBy = value;
        }

        public override TId UpdatedBy
        {
            get => Self.UpdatedBy;
            set => Self.UpdatedBy = value;
        }

        public override DateTime CreatedDate
        {
            get => Self.CreatedDate;
            set => Self.CreatedDate = value;
        }
        public override DateTime UpdatedDate
        {
            get => Self.UpdatedDate;
            set => Self.UpdatedDate = value;
        }

        public override bool Active
        {
            get => Self.Active;
            set => Self.Active = value;
        }

        //holds actual entry class instance
        //also used to check type
        private TEntity Self;

        private bool preloaded;

        private DbContext context;

        private List<PropertyDB_Info<TEntity>> Properties { get; set; }

        #endregion Properties

        //usefull functions
        //Name              - parameter name in code
        //GetValue(dummy)   - actual value
        //PropertyType      - actual values 

        #region Constructor

        protected void CreateInstance(DbContext context) => CreateInstance(context, (TEntity)Activator.CreateInstance(typeof(TEntity), Array.Empty<object>()));

        protected void CreateInstance(DbContext context, TEntity entry)
        {
            Type type = typeof(TEntity);//maybe make property
            Self = entry;
            Properties = new List<PropertyDB_Info<TEntity>>();

            type.GetProperties().ToList().ForEach(c =>
            {
                PropertyDB_Info<TEntity> test = new PropertyDB_Info<TEntity>(c, context, Self);
                Properties.Add(test);
            });
        }

        //new instance
        public DataModel(DbContext context) => CreateInstance(context);

        //create from exist instances
        public DataModel(DbContext context, TEntity entry) => CreateInstance(context, entry);

        //load from database
        public DataModel(DbContext context, DbSet<TEntity> table, TId id, bool preload = false, string load_AlternativeField = "")
        {
            try
            {
                CreateInstance(context);
            }
            catch (Exception e)
            {
                if (e.Message != "The underlying provider failed on Open.")
                    throw e;
                else
                    throw new Exception("Could not connect to database: " + e.InnerException.Message);
            }
            if (!preload)
                LoadId(table, id, load_AlternativeField);
        }

        //public BaseModel(DbSet<Credential> credential, Guid id, bool preloaded)
        //{
        //    this.Credential = credential;
        //    ID = id;
        //    this.preloaded = preloaded;
        //}

        #endregion Constructor

        #region Data Access

        protected int Delete(DbContext dc, DbSet<TEntity> table)
        {
            //Log.GetLogger.Information("Deleting {instance}", instance);
            try
            {
                foreach (var col in table)
                    //instance = where( entry in the table == this ID)
                    if (ID.Equals(col.GetValue(Properties[0].Info.Name)))
                        table.Remove(col);
                int changes = dc.SaveChanges();
                if (changes == 0)
                    throw new Exception("Could not delete value " + Properties[0].Info.Name + " = " + Properties[0].Info.GetValue(Self));
                else
                    return dc.SaveChanges();
            }
            catch (Exception e) { throw new Exception($"Failed to delete {nameof(TEntity)}", e); }
        }

        protected static bool Exists(DbSet<TEntity> table, DataModel<TEntity, TId> instance)
        {
            foreach (var col in table)
                if (instance.ID.Equals(col.GetValue(instance.Properties[0].Info.Name)))
                    return true;
            return false;
        }

        protected int Insert(DbContext dc, DbSet<TEntity> table)
        {
            //Log.GetLogger.Information("Inserting {instance}", instance);
            try
            {
                table.Add(Self);
                return dc.SaveChanges();
            }
            catch (Exception e) { throw new Exception($"Failed to insert {nameof(TEntity)}", e); }
        }

        protected void LoadId(DbSet<TEntity> table, TId id, string load_AlternativeField = "")
        {
            //Log.GetLogger.Information("Loading {instance}", instance);
            if (load_AlternativeField == string.Empty)
            {
                ID = id;
                LoadId(table, (string)Properties[0].Info.Name);
            }
            else
            {
                SetProperty(load_AlternativeField, id);
                LoadId(table, load_AlternativeField);
            }
        }

        protected TEntity LoadId(DbSet<TEntity> table, string propName = "_Default")
        {
            if (propName == "_Default")
                propName = (string)Properties[0].Info.Name;
            //Log.GetLogger.Information("Loading {instance}", propName);
            try
            {
                object tempID = Self.GetValue(propName);
                bool found = false;

                //dont cound the first thing you find
                //it always thinks the first id matches current id.
                //FIRST entry in database is skipped
                bool fixFirstFound = false;
                TEntity firstObject = null;
                //List<TEntity> tDebugVarible = table.ToList();
                foreach (var col in table)
                {
                    if (fixFirstFound)
                    {
                        //instance = where( entry in the table == this ID)
                        if ((col.GetValue(propName).ToString()) == (tempID.ToString()))
                        {
                            Self = col;//sets all properties
                            found = true;
                            break;
                        }
                    }
                    else//keep first entry to check later
                    {
                        firstObject = col;
                        fixFirstFound = true;
                    }
                }
                //check first entry
                if (firstObject != null && (firstObject.GetValue(propName).ToString()) == (tempID.ToString()))
                {
                    Self = firstObject;
                    found = true;
                }

                if (!found)
                    throw new Exception(Self.GetType().Name + " could not be found with ID = " + ID);

                CleanNulls();
                return Self;
            }
            catch (Exception e) { throw new Exception($"Failed to load {nameof(TEntity)}", e); }
        }

        protected int Update(DbContext dc, DbSet<TEntity> table)
        {
            //Log.GetLogger.Information("Updating {instance}", instance);
            try
            {
                if (!Exists(table, this))
                    throw new Exception("ID does not exist in table");

                foreach (var col in table)
                    if (ID.Equals(col.ID))
                    {
                        foreach (var c in Properties)
                            col.SetValue(c.Info.Name,            //set table column
                                Self.GetValue(c.Info.Name)  //to current instance
                            );
                    }
                return dc.SaveChanges();
            }
            catch (Exception e) { throw new Exception($"Failed to update {nameof(TEntity)}", e); }
        }

        #endregion Data Access

        public void Clear()
        {
            foreach (var c in Properties)
                Self.SetValue(c.Info.Name, default);
        }

        protected void SetProperty(string propertyName, object value)
        {
            try
            {
                PropertyDB_Info<TEntity> propinf = Properties.Where(c => c.Info.Name == propertyName).FirstOrDefault();
                if (propinf == null)
                    throw new PropertyException(typeof(TEntity), propertyName);

                Type propType = propinf.Info.PropertyType;

                if (propinf != null)
                {
                    propinf.Info.SetValue(Self, value);
                    if (value == null)
                        propinf.Info.SetValue(Self, null);//deal with null right away
                    else if (propType.Name == "Nullable`1")
                    {
                        Type[] propGeneric = propType.GenericTypeArguments;

                        if (propGeneric.Length > 0)
                        {
                            if (propGeneric[0].Name == "DateTime")
                                propinf.Info.SetValue(Self, (DateTime)value);
                            else if (propGeneric[0].Name == "Guid")
                                propinf.Info.SetValue(Self, (Guid)value);
                            else
                                propinf.Info.SetValue(Self, value);
                        }
                        else
                            propinf.Info.SetValue(Self, value);
                    }
                    else if (propType.Name == "String" &&
                             ((string)value).Length > propinf.Max)//get property index equal to current property to compare sized
                    {
                        propinf.Info.SetValue(Self, ((string)value));//.Substring(0, propinf.max - 1));//cut of larger values (zero based)
                    }
                    else
                        propinf.Info.SetValue(Self, value);
                }
                else
                    throw new PropertyException(typeof(TEntity), propertyName);
            }
            catch (Exception e)
            {
                throw new Exception(typeof(TEntity) + ": " + e.Message);
            }
        }

        //any property that cant be null should be put in here
        //TODO make abstract list of nonnullable values
        private void CleanNulls()
        {
            foreach (var p in Properties)
                if (p.Info.GetValue(Self) == null)
                {
                    switch (p.Info.Name)
                    {
                        case "DateOfBirth":
                            p.Info.SetValue(Self, new DateTime());
                            break;
                        default:
                            p.Info.SetValue(Self, default);
                            break;
                    }
                }
        }

    }
}