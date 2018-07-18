using SqlSugar;
using System;
using System.Collections.Generic;
using static SqlSugarDemo.Busines.Base.SugarBase;

namespace SqlSugarDemo.Busines
{
    public class HomeManager
    {
        public bool DBTest()
        {
            return DB.Ado.ExecuteCommand("insert into TB_User values(@Name,@Gender,@Phone,@CreateDate)", new List<SugarParameter>
            {
                new SugarParameter("@Name", "Dos.Orm"),
                new SugarParameter("@Gender", false),
                new SugarParameter("@Phone", "18888888888"),
                new SugarParameter("@CreateDate", DateTime.Now)
            }) > 0;
        }

        /// <summary>
        /// 不自动释放
        /// </summary>
        /// <returns></returns>
        public bool NotAutoDisposeTest()
        {
            using(var db = GetIntance())
            {
                var id = db.Ado.GetInt("Select ID from TB_User where Name = @Name", new SugarParameter("@Name", "SqlSugar"));
                var result = db.Ado.ExecuteCommand("update TB_User set Name = @Name where ID = @ID", new SugarParameter("@Name", "SqlSugar_SqlSugar"), new SugarParameter("@ID", id));
                return result > 0;
            }
        }

        /// <summary>
        /// Exec方法
        /// </summary>
        /// <returns></returns>
        public bool ExecTest()
        {
            return Exec(db =>
            {
                var id = db.Ado.GetInt("Select ID from TB_User where Name = @Name", new SugarParameter("@Name", "SqlSugar_SqlSugar"));
                var result = db.Ado.ExecuteCommand("update TB_User set Name = @Name where ID = @ID", new SugarParameter("@Name", "SqlSugar_Sugar"), new SugarParameter("@ID", id));
                return result > 0;
            });
        }
    }
}