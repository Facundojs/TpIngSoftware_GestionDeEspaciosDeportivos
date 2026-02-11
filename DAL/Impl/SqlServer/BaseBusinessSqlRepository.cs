using Service.Helpers;
using Service.Impl;

namespace DAL.Impl
{
    public class BaseBusinessSqlRepository : BaseRepository
    {
        public BaseBusinessSqlRepository() : base(ConnectionManager.BusinessConnectionName)
        {
        }
    }
}
