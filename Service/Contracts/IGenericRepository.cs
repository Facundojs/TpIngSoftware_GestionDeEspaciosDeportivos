using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    /// <summary>
    /// Generic repository contract defining standard CRUD operations for any entity type.
    /// </summary>
    /// <typeparam name="T">The domain entity type managed by this repository.</typeparam>
    public interface IGenericRepository<T>
    {
        /// <summary>Persists a new entity.</summary>
        /// <param name="obj">The entity to insert.</param>
        void Add(T obj);

        /// <summary>Applies changes to an existing entity.</summary>
        /// <param name="obj">The entity with updated values.</param>
        void Update(T obj);

        /// <summary>Deletes an entity by its primary key.</summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        void Remove(Guid id);

        /// <summary>Retrieves a single entity by its primary key.</summary>
        /// <param name="id">The unique identifier to look up.</param>
        /// <returns>The matching entity, or <c>null</c> if not found.</returns>
        T GetById(Guid id);

        /// <summary>Retrieves all entities of this type.</summary>
        /// <returns>A list of all stored entities; empty list if none exist.</returns>
        List<T> GetAll();
    }
}
