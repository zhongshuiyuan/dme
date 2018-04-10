using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Dist.Dme.Base.Utils
{
    /// <summary>
    /// 使用IL来创建泛型对象，比使用Activator.CreateInstance(type)效率高很多
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CreationHelper<T> where T : new()
    {
        private static Func<T> objCreator = null;

        public static T New()
        {
            if (objCreator == null)
            {
                Type objectType = typeof(T);

                ConstructorInfo defaultCtor = objectType.GetConstructor(new Type[] { });

                DynamicMethod dynMethod = new DynamicMethod(
                    name: string.Format("_{0:N}", Guid.NewGuid()),
                    returnType: objectType,
                    parameterTypes: null);

                var gen = dynMethod.GetILGenerator();
                gen.Emit(OpCodes.Newobj, defaultCtor);
                gen.Emit(OpCodes.Ret);

                objCreator = dynMethod.CreateDelegate(typeof(Func<T>)) as Func<T>;
            }

            return objCreator();
        }
    }
}
