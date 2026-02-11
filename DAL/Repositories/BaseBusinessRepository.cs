using Service.Helpers;
using Service.Impl;

namespace DAL.Repositories
{
    public class BaseBusinessRepository : BaseRepository
    {
        public BaseBusinessRepository() : base(ConnectionManager.BusinessConnectionName)
        {
        }
    }
}
