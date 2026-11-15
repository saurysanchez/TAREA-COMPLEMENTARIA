// Saury Salvador Sanchez Betltre Matricula: 20261000
using Microsoft.Data.SqlClient;

string connectionString = "Server=localhost; Database=ClinicaDB; Trusted_Connection=True; TrustServerCertificate=True;";

bool salir = false;

while (!salir)
{
    Console.WriteLine("SISTEMA DE REGISTRO DE PACIENTES CLINICA");
    Console.WriteLine("1. Agregar Paciente");
    Console.WriteLine("2. Buscar Paciente");
    Console.WriteLine("3. Modificar Paciente");
    Console.WriteLine("4. Eliminar Paciente");
    Console.WriteLine("5. Listar Pacientes");
    Console.WriteLine("0. SALIR");
    Console.Write("Opcion: ");
    try
    {
        string opcion = Console.ReadLine();
        switch (opcion)
        {
            case "1": AgregarPaciente(); break;
            case "2": BuscarPaciente(); break;
            case "3": ModificarPaciente(); break;
            case "4": EliminarPaciente(); break;
            case "5": ListarPacientes(); break;
            case "0": salir = true; break;
            default: Console.WriteLine("Opcion invalida"); break;
        }
    }
    catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
}
Console.WriteLine("Saliendo del sistema...");

void AgregarPaciente()
{
    Paciente paciente = new Paciente();
    paciente.Cedula = LeerTexto("Cedula: ");

    using (SqlConnection conexion = new SqlConnection(connectionString))
    {
        conexion.Open();
        string queryCheck = "SELECT COUNT(1) FROM Pacientes WHERE Cedula = @Cedula";
        using (SqlCommand cmdCheck = new SqlCommand(queryCheck, conexion))
        {
            cmdCheck.Parameters.AddWithValue("@Cedula", paciente.Cedula);
            int existe = (int)cmdCheck.ExecuteScalar();
            if (existe > 0)
            {
                Console.WriteLine("Ya existe un paciente con esa cedula.");
                return;
            }
        }

        paciente.Nombre = LeerTexto("Nombre completo: ");
        paciente.Telefono = LeerTexto("Telefono: ");
        paciente.Edad = LeerEdad("Edad: ");

        string queryInsert = "INSERT INTO Pacientes (Cedula, Nombre, Telefono, Edad) VALUES (@Cedula, @Nombre, @Telefono, @Edad)";
        using (SqlCommand cmdInsert = new SqlCommand(queryInsert, conexion))
        {
            cmdInsert.Parameters.AddWithValue("@Cedula", paciente.Cedula);
            cmdInsert.Parameters.AddWithValue("@Nombre", paciente.Nombre);
            cmdInsert.Parameters.AddWithValue("@Telefono", paciente.Telefono);
            cmdInsert.Parameters.AddWithValue("@Edad", paciente.Edad);
            cmdInsert.ExecuteNonQuery();
        }
    }
    Console.WriteLine("Paciente agregado exitosamente.");
}

void BuscarPaciente()
{
    string texto = LeerTexto("Ingrese parte del nombre o cedula a buscar: ");
    bool encontrado = false;

    using (SqlConnection conexion = new SqlConnection(connectionString))
    {
        conexion.Open();
        string query = "SELECT Id, Cedula, Nombre, Telefono, Edad FROM Pacientes WHERE LOWER(Nombre) LIKE @Texto OR Cedula LIKE @Texto";
        using (SqlCommand cmd = new SqlCommand(query, conexion))
        {
            cmd.Parameters.AddWithValue("@Texto", "%" + texto.ToLower() + "%");
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Paciente p = new Paciente();
                    p.Id = reader.GetInt32(0);
                    p.Cedula = reader.GetString(1);
                    p.Nombre = reader.GetString(2);
                    p.Telefono = reader.GetString(3);
                    p.Edad = reader.GetInt32(4);

                    MostrarPaciente(p);
                    encontrado = true;
                }
            }
        }
    }

    if (!encontrado)
        Console.WriteLine("No se encontraron pacientes con ese criterio.");
}

void ModificarPaciente()
{
    string cedula = LeerTexto("Ingrese la cedula del paciente a modificar: ");
    Paciente pacienteActual = null;

    using (SqlConnection conexion = new SqlConnection(connectionString))
    {
        conexion.Open();
        string querySelect = "SELECT Id, Cedula, Nombre, Telefono, Edad FROM Pacientes WHERE Cedula = @Cedula";
        using (SqlCommand cmd = new SqlCommand(querySelect, conexion))
        {
            cmd.Parameters.AddWithValue("@Cedula", cedula);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    pacienteActual = new Paciente();
                    pacienteActual.Id = reader.GetInt32(0);
                    pacienteActual.Cedula = reader.GetString(1);
                    pacienteActual.Nombre = reader.GetString(2);
                    pacienteActual.Telefono = reader.GetString(3);
                    pacienteActual.Edad = reader.GetInt32(4);
                }
            }
        }

        if (pacienteActual == null)
        {
            Console.WriteLine("No existe un paciente registrado con esa cedula.");
            return;
        }

        Console.WriteLine("(Presiona Enter para dejar el valor actual)");
        string nuevoNombre = LeerConActual("Nombre", pacienteActual.Nombre);
        string nuevoTelefono = LeerConActual("Telefono", pacienteActual.Telefono);
        int nuevaEdad = LeerEdadConActual("Edad", pacienteActual.Edad);

        string queryUpdate = "UPDATE Pacientes SET Nombre = @Nombre, Telefono = @Telefono, Edad = @Edad WHERE Cedula = @Cedula";
        using (SqlCommand cmdUpdate = new SqlCommand(queryUpdate, conexion))
        {
            cmdUpdate.Parameters.AddWithValue("@Nombre", nuevoNombre);
            cmdUpdate.Parameters.AddWithValue("@Telefono", nuevoTelefono);
            cmdUpdate.Parameters.AddWithValue("@Edad", nuevaEdad);
            cmdUpdate.Parameters.AddWithValue("@Cedula", cedula);
            cmdUpdate.ExecuteNonQuery();
        }
    }
    Console.WriteLine("Datos del paciente modificados correctamente.");
}

void EliminarPaciente()
{
    string cedula = LeerTexto("Ingrese la cedula del paciente a eliminar: ");

    using (SqlConnection conexion = new SqlConnection(connectionString))
    {
        conexion.Open();
        string queryCheck = "SELECT COUNT(1) FROM Pacientes WHERE Cedula = @Cedula";
        using (SqlCommand cmdCheck = new SqlCommand(queryCheck, conexion))
        {
            cmdCheck.Parameters.AddWithValue("@Cedula", cedula);
            int existe = (int)cmdCheck.ExecuteScalar();
            if (existe == 0)
            {
                Console.WriteLine("No existe un paciente con esa cedula.");
                return;
            }
        }

        Console.WriteLine("Esta seguro de eliminar este paciente? si/no");
        string respuesta = Console.ReadLine()?.Trim().ToLower();

        if (respuesta == "si")
        {
            string queryDelete = "DELETE FROM Pacientes WHERE Cedula = @Cedula";
            using (SqlCommand cmdDelete = new SqlCommand(queryDelete, conexion))
            {
                cmdDelete.Parameters.AddWithValue("@Cedula", cedula);
                cmdDelete.ExecuteNonQuery();
            }
            Console.WriteLine("Paciente eliminado exitosamente.");
        }
        else
        {
            Console.WriteLine("Operacion cancelada.");
        }
    }
}

void ListarPacientes()
{
    bool hayRegistros = false;

    using (SqlConnection conexion = new SqlConnection(connectionString))
    {
        conexion.Open();
        string query = "SELECT Id, Cedula, Nombre, Telefono, Edad FROM Pacientes";
        using (SqlCommand cmd = new SqlCommand(query, conexion))
        {
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Paciente p = new Paciente();
                    p.Id = reader.GetInt32(0);
                    p.Cedula = reader.GetString(1);
                    p.Nombre = reader.GetString(2);
                    p.Telefono = reader.GetString(3);
                    p.Edad = reader.GetInt32(4);

                    MostrarPaciente(p);
                    hayRegistros = true;
                }
            }
        }
    }

    if (!hayRegistros)
        Console.WriteLine("No hay pacientes registrados.");
}

void MostrarPaciente(Paciente p)
{
    Console.WriteLine($"[ID: {p.Id}] Cedula: {p.Cedula} | Nombre: {p.Nombre} | Telefono: {p.Telefono} | Edad: {p.Edad}");
}

string LeerTexto(string mensaje)
{
    while (true)
    {
        Console.Write(mensaje);
        string texto = (Console.ReadLine() ?? "").Trim();
        if (texto != "") return texto;
        Console.WriteLine("El campo no puede estar vacio.");
    }
}

string LeerConActual(string campo, string actual)
{
    Console.Write($"{campo} [{actual}]: ");
    string texto = (Console.ReadLine() ?? "").Trim();
    if (texto == "") return actual;
    return texto;
}

int LeerEdad(string mensaje)
{
    while (true)
    {
        Console.Write(mensaje);
        if (int.TryParse(Console.ReadLine(), out int edad) && edad > 0 && edad < 120)
            return edad;
        Console.WriteLine("Edad invalida. Ingrese un numero entre 1 y 119.");
    }
}

int LeerEdadConActual(string campo, int actual)
{
    while (true)
    {
        Console.Write($"{campo} [{actual}]: ");
        string entrada = Console.ReadLine()?.Trim();
        if (entrada == "") return actual;
        if (int.TryParse(entrada, out int edad) && edad > 0 && edad < 120)
            return edad;
        Console.WriteLine("Edad invalida. Ingrese un numero entre 1 y 119.");
    }
}

class Paciente
{
    public int Id { get; set; }
    public string Cedula { get; set; }
    public string Nombre { get; set; }
    public string Telefono { get; set; }
    public int Edad { get; set; }
}