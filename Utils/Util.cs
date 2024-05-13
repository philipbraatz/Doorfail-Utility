namespace Doorfail.Utils
{
    public static class Util
    {
        private static Random rng = new();

        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while(n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T CopyTo<T>(this object obj)
        {
            var ret = Activator.CreateInstance<T>();
            obj.CopyTo(ret);
            return ret;
        }
        public static void CopyTo<T>(this object from, T to)
        {
            if(from is null || to is null)
                throw new ArgumentNullException(from is null ? nameof(from) : nameof(to));
            var type = from.GetType();
            foreach(var prop in type.GetProperties())
            {
                var prop2 = typeof(T).GetProperty(prop.Name);//, System.Reflection.BindingFlags.SetField | System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Public);
                if(prop2.SetMethod is not null)
                    prop2.SetValue(to, prop.GetValue(from));
            }
        }
    }
}