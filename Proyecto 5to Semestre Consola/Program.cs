using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;
using System.Data.SqlClient;

namespace Proyecto_5to_Semestre_Consola
{
    internal class Program
    {
        // Cadena de conexión
        static string connectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=TESTING_DB;Integrated Security=True";

        static void Main(string[] args)
        {

            Console.WriteLine("Bienvenido al sistema de login.");

            // Solicitar el correo y la contraseña
            Console.Write("Ingrese su correo: ");
            string correo = Console.ReadLine();

            Console.Write("Ingrese su contraseña: ");
            string contraseña = Console.ReadLine();

            // Verificar las credenciales
            if (VerificarLogin(correo, contraseña))
            {
                Console.WriteLine("Inicio de sesión exitoso.");
                // Aquí puedes agregar lo que se debe hacer después de un login exitoso
                // Ejemplo: Redirigir a otra parte de la aplicación
            }
            else
            {
                Console.WriteLine("Correo o contraseña incorrectos.");
            }

            // Pausar la consola para ver el resultado
            Console.ReadLine();
        }

        // Método para verificar el login
        static bool VerificarLogin(string correo, string contraseña)
        {
            try
            {
                // Encriptamos la contraseña ingresada por el usuario
                string contraseñaEncriptada = EncriptarContraseña(contraseña);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Abrimos la conexión
                    connection.Open();

                    // Consulta SQL para obtener la contraseña del empleado basado en su correo
                    string query = "SELECT id_empleado, nombre, contraseña FROM EMPLEADO WHERE correo = @correo";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@correo", correo);

                        // Ejecutar la consulta y leer los resultados
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Recuperamos la contraseña almacenada en la base de datos
                                string storedPassword = reader["contraseña"].ToString();

                                // Verificamos si la contraseña encriptada coincide
                                if (storedPassword == contraseñaEncriptada)
                                {
                                    // Si las contraseñas coinciden, retornamos true (login exitoso)
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al verificar el login: " + ex.Message);
            }

            // Si no se encontró el correo o las contraseñas no coinciden, retornamos false
            return false;
        }

        // Método para encriptar la contraseña usando SHA256
        static string EncriptarContraseña(string contraseña)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convertimos la contraseña a un array de bytes
                byte[] bytes = Encoding.UTF8.GetBytes(contraseña);

                // Aplicamos el algoritmo SHA256 para obtener el hash
                byte[] hashBytes = sha256.ComputeHash(bytes);

                // Convertimos el array de bytes en una cadena hexadecimal
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));  // "x2" convierte el byte a hexadecimal
                }

                return sb.ToString();  // Devolvemos la contraseña encriptada
            }


        }
    }
}
