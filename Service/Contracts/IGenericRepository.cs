using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    /// <summary>
    /// Interfaz genérica que define operaciones CRUD para cualquier entidad.
    /// </summary>
    /// <typeparam name="T">Tipo de entidad que se manejará.</typeparam>
    public interface IGenericRepository<T>
    {
        /// <summary>
        /// Agrega una nueva entidad.
        /// </summary>
        /// <param name="obj">La entidad a agregar.</param>
        void Add(T obj);

        /// <summary>
        /// Actualiza una entidad existente.
        /// </summary>
        /// <param name="obj">La entidad con los datos actualizados.</param>
        void Update(T obj);

        /// <summary>
        /// Elimina una entidad por su identificador.
        /// </summary>
        /// <param name="id">El identificador de la entidad a eliminar.</param>
        void Remove(Guid id);

        /// <summary>
        /// Obtiene una entidad por su identificador.
        /// </summary>
        /// <param name="id">El identificador de la entidad a obtener.</param>
        /// <returns>La entidad correspondiente al identificador.</returns>
        T GetById(Guid id);

        /// <summary>
        /// Obtiene todas las entidades.
        /// </summary>
        /// <returns>Una lista de todas las entidades.</returns>
        List<T> GetAll();
    }
}
