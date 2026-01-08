using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Service;
using Service.Domain;
using Service.Impl;
using Service.Impl.SqlServer;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class Form1: Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TestRepositorios();
        }

        public void TestRepositorios()
        {
            // 1. Instanciar Repositorios
            var patenteRepo = new PatenteRepository();
            var familiaRepo = new FamiliaRepository();

            Console.WriteLine("--- Iniciando Test de Repositorios ---");
            
                // 2. Crear una Patente Aleatoria
                var nuevaPatente = new Patente
                {
                    Id = Guid.NewGuid(),
                    Nombre = "Permiso_Test_" + Path.GetRandomFileName().Substring(0, 4),
                    TipoAcceso = 1,
                    DataKey = "btnTest_" + new Random().Next(100, 999)
                };

                patenteRepo.Add(nuevaPatente);
                Console.WriteLine($"[OK] Patente creada: {nuevaPatente.Nombre}");

                // 3. Crear una Familia Aleatoria
                var nuevaFamilia = new Familia
                {
                    Id = Guid.NewGuid(),
                    Nombre = "Rol_Test_" + Path.GetRandomFileName().Substring(0, 4)
                };

                familiaRepo.Add(nuevaFamilia);
                Console.WriteLine($"[OK] Familia creada: {nuevaFamilia.Nombre}");

                // 4. Asociar Patente a Familia y Actualizar (Test de Transacción)
                nuevaFamilia.Agregar(nuevaPatente);
                familiaRepo.Update(nuevaFamilia);
                Console.WriteLine($"[OK] Patente asociada a Familia correctamente.");

                // 5. Recuperar de BD para verificar integridad
                var familiaRecuperada = familiaRepo.GetById(nuevaFamilia.Id);

                if (familiaRecuperada != null && familiaRecuperada.Accesos.Any(h => h.Id == nuevaPatente.Id))
                {
                    Console.WriteLine("--- TEST EXITOSO ---");
                    Console.WriteLine($"Familia: {familiaRecuperada.Nombre}");
                    Console.WriteLine($"Hijo encontrado: {familiaRecuperada.Accesos.First().Nombre}");
                }
                else
                {
                    Console.WriteLine("--- TEST FALLIDO: No se recuperaron los hijos ---");
                }

        }
    }
}
