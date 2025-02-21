using Common.DTO;
using Dapper;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Context
{
    public class AutoMapper
    {

        public void SetMappings()
        {
            Map<TodoTask>();

            //SqlMapper.AddTypeMap(typeof(MyTaskItem.Systems), System.Data.DbType.Int32);
        }

        private void Map<T>() where T : class
        {
            SqlMapper.SetTypeMap(typeof(T), new CustomPropertyTypeMap(
                typeof(T), (type, columnName) => type.GetProperties().FirstOrDefault(prop =>
                {
                    return prop.Name.ToLower() == columnName.ToLower() || prop.GetCustomAttributes(false)
                    .OfType<ColumnAttribute>()
                    .Any(attr => attr?.Name?.ToLower() == columnName.ToLower());
                }
                )));
        }

    }


}
