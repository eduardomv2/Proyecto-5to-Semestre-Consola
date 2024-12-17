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

            // Solicitar correo y contraseña
            Console.Write("Ingrese su correo: ");
            string correo = Console.ReadLine();

            Console.Write("Ingrese su contraseña: ");
            string contraseña = Console.ReadLine();

            // Verificar login
            if (VerificarLogin(correo, contraseña))
            {
                Console.WriteLine("Inicio de sesión exitoso.");

                // Obtener el rol del usuario
                int rolId = ObtenerRolUsuario(correo);

                if (rolId == 1) // Rol de Jefe
                {
                    Console.WriteLine("Bienvenido, Jefe. Elige una opción:");
                    MostrarMenuJefe();
                }
                else
                {
                    Console.WriteLine("Acceso denegado: No tienes permisos para realizar esta operación.");
                }
            }
            else
            {
                Console.WriteLine("Correo o contraseña incorrectos.");
            }

            // Pausar la consola
            Console.ReadLine();
        }

        static void VerUsuarios()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Consulta para obtener todos los empleados
                    string query = "SELECT id_empleado, nombre, correo, id_rol, id_turno FROM EMPLEADO";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Console.WriteLine("\n--- Lista de Empleados ---");
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    // Mostrar los datos de cada empleado
                                    Console.WriteLine($"ID: {reader["id_empleado"]}, " +
                                                      $"Nombre: {reader["nombre"]}, " +
                                                      $"Correo: {reader["correo"]}, " +
                                                      $"Rol ID: {reader["id_rol"]}, " +
                                                      $"Turno ID: {reader["id_turno"]}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No hay empleados registrados.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener la lista de empleados: " + ex.Message);
            }
        }



        // Método para verificar el login
        static bool VerificarLogin(string correo, string contraseña)
        {
            try
            {
                string contraseñaEncriptada = EncriptarContraseña(contraseña);

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT id_empleado, nombre, contraseña, id_rol FROM EMPLEADO WHERE correo = @correo";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@correo", correo);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedPassword = reader["contraseña"].ToString();
                                if (storedPassword == contraseñaEncriptada)
                                {
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
            return false;
        }

        // Método para obtener el rol del usuario
        static int ObtenerRolUsuario(string correo)
        {
            int rolId = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT id_rol FROM EMPLEADO WHERE correo = @correo";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@correo", correo);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                rolId = Convert.ToInt32(reader["id_rol"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener el rol del usuario: " + ex.Message);
            }

            return rolId;
        }

        static void VerAutos()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT id_auto, marca, modelo, año, color FROM AUTO";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                Console.WriteLine("\nLista de Autos:");
                                while (reader.Read())
                                {
                                    Console.WriteLine($"ID: {reader["id_auto"]}, Marca: {reader["marca"]}, Modelo: {reader["modelo"]}, Año: {reader["año"]}, Color: {reader["color"]}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("No hay autos registrados.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al mostrar autos: " + ex.Message);
            }
        }

        static void AgregarAuto()
        {
            try
            {
                Console.Write("\nIngrese la marca del auto: ");
                string marca = Console.ReadLine();
                Console.Write("Ingrese el modelo del auto: ");
                string modelo = Console.ReadLine();
                Console.Write("Ingrese el año del auto: ");
                int año = Convert.ToInt32(Console.ReadLine());
                Console.Write("Ingrese el color del auto: ");
                string color = Console.ReadLine();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO AUTO (marca, modelo, año, color) VALUES (@marca, @modelo, @año, @color)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@marca", marca);
                        command.Parameters.AddWithValue("@modelo", modelo);
                        command.Parameters.AddWithValue("@año", año);
                        command.Parameters.AddWithValue("@color", color);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("\nAuto agregado exitosamente.");
                        }
                        else
                        {
                            Console.WriteLine("\nError al agregar el auto.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al agregar el auto: " + ex.Message);
            }
        }

        static void ModificarAuto()
        {
            try
            {
                Console.Write("Ingrese el ID del auto a modificar: ");
                int idAuto = Convert.ToInt32(Console.ReadLine());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Obtener el auto actual
                    string query = "SELECT id_auto, marca, modelo, año, color FROM AUTO WHERE id_auto = @idAuto";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idAuto", idAuto);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Console.WriteLine($"\nAuto encontrado: ");
                                Console.WriteLine($"ID: {reader["id_auto"]}");
                                Console.WriteLine($"Marca: {reader["marca"]}");
                                Console.WriteLine($"Modelo: {reader["modelo"]}");
                                Console.WriteLine($"Año: {reader["año"]}");
                                Console.WriteLine($"Color: {reader["color"]}");
                                reader.Close(); // Cerramos el reader antes de hacer la actualización

                                // Solicitar nuevos datos
                                Console.Write("\nIngrese la nueva marca (deje en blanco para no cambiar): ");
                                string nuevaMarca = Console.ReadLine();
                                Console.Write("Ingrese el nuevo modelo (deje en blanco para no cambiar): ");
                                string nuevoModelo = Console.ReadLine();
                                Console.Write("Ingrese el nuevo año (deje en blanco para no cambiar): ");
                                string nuevoAño = Console.ReadLine();
                                Console.Write("Ingrese el nuevo color (deje en blanco para no cambiar): ");
                                string nuevoColor = Console.ReadLine();

                                // Construir la consulta UPDATE solo con los valores que han cambiado
                                string updateQuery = "UPDATE AUTO SET ";

                                bool tieneCambio = false;
                                if (!string.IsNullOrEmpty(nuevaMarca))
                                {
                                    updateQuery += "marca = @nuevaMarca, ";
                                    tieneCambio = true;
                                }
                                if (!string.IsNullOrEmpty(nuevoModelo))
                                {
                                    updateQuery += "modelo = @nuevoModelo, ";
                                    tieneCambio = true;
                                }
                                if (!string.IsNullOrEmpty(nuevoAño))
                                {
                                    updateQuery += "año = @nuevoAño, ";
                                    tieneCambio = true;
                                }
                                if (!string.IsNullOrEmpty(nuevoColor))
                                {
                                    updateQuery += "color = @nuevoColor, ";
                                    tieneCambio = true;
                                }

                                // Eliminar la última coma
                                if (tieneCambio)
                                {
                                    updateQuery = updateQuery.Substring(0, updateQuery.Length - 2); // Eliminar la última coma
                                    updateQuery += " WHERE id_auto = @idAuto"; // Asegurar que se actualice el auto correcto

                                    // Ejecutar la actualización
                                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                                    {
                                        if (!string.IsNullOrEmpty(nuevaMarca))
                                            updateCommand.Parameters.AddWithValue("@nuevaMarca", nuevaMarca);
                                        if (!string.IsNullOrEmpty(nuevoModelo))
                                            updateCommand.Parameters.AddWithValue("@nuevoModelo", nuevoModelo);
                                        if (!string.IsNullOrEmpty(nuevoAño))
                                            updateCommand.Parameters.AddWithValue("@nuevoAño", Convert.ToInt32(nuevoAño));
                                        if (!string.IsNullOrEmpty(nuevoColor))
                                            updateCommand.Parameters.AddWithValue("@nuevoColor", nuevoColor);
                                        updateCommand.Parameters.AddWithValue("@idAuto", idAuto);

                                        int filasAfectadas = updateCommand.ExecuteNonQuery();
                                        if (filasAfectadas > 0)
                                        {
                                            Console.WriteLine("\nAuto actualizado exitosamente.");
                                        }
                                        else
                                        {
                                            Console.WriteLine("\nNo se pudo actualizar el auto.");
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("\nNo se han realizado cambios.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Auto no encontrado.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al modificar auto: " + ex.Message);
            }
        }



        // Método para mostrar el menú del Jefe
        static void MostrarMenuJefe()
        {
            Console.WriteLine("Seleccione una opción:");
            Console.WriteLine("1. Ver empleados");
            Console.WriteLine("2. Contratar empleado");
            Console.WriteLine("3. Modificar empleado");
            Console.WriteLine("4. Ver autos");
            Console.WriteLine("5. Agregar auto");
            Console.WriteLine("6. Modificar auto");
            Console.WriteLine("7. Salir");
            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    VerUsuarios();  // Ver empleados
                    break;
                case "2":
                    ContratarEmpleado();  // Contratar empleado
                    break;
                case "3":
                    ModificarEmpleado();  // Modificar empleado
                    break;
                case "4":
                    VerAutos();  // Ver autos
                    break;
                case "5":
                    AgregarAuto();  // Agregar auto
                    break;
                case "6":
                    ModificarAuto();  // Modificar auto
                    break;
                case "7":
                    Environment.Exit(0);  // Salir
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }

        static void ModificarEmpleado()
        {
            try
            {
                Console.Write("Ingrese el ID del empleado a modificar: ");
                int idEmpleado = Convert.ToInt32(Console.ReadLine());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Primero, obtener los datos actuales del empleado
                    string query = "SELECT id_empleado, nombre, correo, id_rol, id_turno FROM EMPLEADO WHERE id_empleado = @idEmpleado";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idEmpleado", idEmpleado);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Console.WriteLine($"\nEmpleado encontrado: ");
                                Console.WriteLine($"ID: {reader["id_empleado"]}");
                                Console.WriteLine($"Nombre: {reader["nombre"]}");
                                Console.WriteLine($"Correo: {reader["correo"]}");
                                Console.WriteLine($"Rol ID: {reader["id_rol"]}");
                                Console.WriteLine($"Turno ID: {reader["id_turno"]}");
                                reader.Close(); // Cerrar el reader antes de modificar

                                // Ahora, pedir los nuevos datos para modificar
                                Console.Write("\nIngrese el nuevo nombre (deje en blanco para no cambiar): ");
                                string nuevoNombre = Console.ReadLine();
                                Console.Write("Ingrese el nuevo correo (deje en blanco para no cambiar): ");
                                string nuevoCorreo = Console.ReadLine();
                                Console.Write("Ingrese el nuevo ID de rol (deje en blanco para no cambiar): ");
                                string nuevoRol = Console.ReadLine();
                                Console.Write("Ingrese el nuevo ID de turno (deje en blanco para no cambiar): ");
                                string nuevoTurno = Console.ReadLine();

                                // Crear una consulta de actualización solo con los campos que han cambiado
                                string updateQuery = "UPDATE EMPLEADO SET ";

                                bool tieneCambio = false;
                                if (!string.IsNullOrEmpty(nuevoNombre))
                                {
                                    updateQuery += "nombre = @nuevoNombre, ";
                                    tieneCambio = true;
                                }
                                if (!string.IsNullOrEmpty(nuevoCorreo))
                                {
                                    updateQuery += "correo = @nuevoCorreo, ";
                                    tieneCambio = true;
                                }
                                if (!string.IsNullOrEmpty(nuevoRol))
                                {
                                    updateQuery += "id_rol = @nuevoRol, ";
                                    tieneCambio = true;
                                }
                                if (!string.IsNullOrEmpty(nuevoTurno))
                                {
                                    updateQuery += "id_turno = @nuevoTurno, ";
                                    tieneCambio = true;
                                }

                                // Eliminar la última coma
                                if (tieneCambio)
                                {
                                    updateQuery = updateQuery.Substring(0, updateQuery.Length - 2); // Eliminar la última coma
                                    updateQuery += " WHERE id_empleado = @idEmpleado"; // Asegurar que se actualice el empleado correcto

                                    // Ejecutar la actualización
                                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                                    {
                                        if (!string.IsNullOrEmpty(nuevoNombre))
                                            updateCommand.Parameters.AddWithValue("@nuevoNombre", nuevoNombre);
                                        if (!string.IsNullOrEmpty(nuevoCorreo))
                                            updateCommand.Parameters.AddWithValue("@nuevoCorreo", nuevoCorreo);
                                        if (!string.IsNullOrEmpty(nuevoRol))
                                            updateCommand.Parameters.AddWithValue("@nuevoRol", Convert.ToInt32(nuevoRol));
                                        if (!string.IsNullOrEmpty(nuevoTurno))
                                            updateCommand.Parameters.AddWithValue("@nuevoTurno", Convert.ToInt32(nuevoTurno));
                                        updateCommand.Parameters.AddWithValue("@idEmpleado", idEmpleado);

                                        int filasAfectadas = updateCommand.ExecuteNonQuery();
                                        if (filasAfectadas > 0)
                                        {
                                            Console.WriteLine("\nEmpleado actualizado exitosamente.");
                                        }
                                        else
                                        {
                                            Console.WriteLine("\nNo se pudo actualizar el empleado.");
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("\nNo se han realizado cambios.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Empleado no encontrado.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al modificar empleado: " + ex.Message);
            }
        }


        // Método para contratar un empleado
        static void ContratarEmpleado()
        {
            Console.WriteLine("\n--- Contratar Nuevo Empleado ---");
            Console.Write("Ingrese nombre del empleado: ");
            string nombre = Console.ReadLine();

            Console.Write("Ingrese correo del empleado: ");
            string correo = Console.ReadLine();

            Console.Write("Ingrese contraseña para el empleado: ");
            string contraseña = Console.ReadLine();
            string contraseñaEncriptada = EncriptarContraseña(contraseña);

            Console.Write("Ingrese el rol del empleado (1=Jefe, 2=Vendedor, etc.): ");
            int rol = Convert.ToInt32(Console.ReadLine());

            // Preguntar por el id_turno
            Console.Write("Ingrese el id_turno del empleado: ");
            int idTurno = Convert.ToInt32(Console.ReadLine());  // Valor proporcionado para id_turno

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO EMPLEADO (nombre, correo, contraseña, id_rol, id_turno) " +
                                   "VALUES (@nombre, @correo, @contraseña, @rol, @idTurno)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", nombre);
                        command.Parameters.AddWithValue("@correo", correo);
                        command.Parameters.AddWithValue("@contraseña", contraseñaEncriptada);
                        command.Parameters.AddWithValue("@rol", rol);
                        command.Parameters.AddWithValue("@idTurno", idTurno); // Agregar el id_turno

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Empleado contratado exitosamente.");
                        }
                        else
                        {
                            Console.WriteLine("Error al contratar al empleado.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al contratar empleado: " + ex.Message);
            }
        }

       
        // Método para encriptar la contraseña utilizando SHA256
        static string EncriptarContraseña(string contraseña)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(contraseña);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
    
}
