namespace Doorfail.Core.Util
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
            var type = obj.GetType();
            var ret = Activator.CreateInstance<T>();
            foreach(var prop in type.GetProperties())
            {
                var prop2 = typeof(T).GetProperty(prop.Name);
                prop2?.SetValue(ret, prop.GetValue(obj));
            }
            return ret;
        }
    }
}