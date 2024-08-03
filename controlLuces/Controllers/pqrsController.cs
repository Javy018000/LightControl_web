
using controlLuces.Models;
using controlLuces.permisos;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace controlLuces.Controllers
{
    //[Authorize] //se comenta por que no me esta permitiendo acceder al controlador sin la autorizacion pero no realiza la autorizacion
    public class pqrsController : Controller
    {
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;

        void connectionString()
        {
            //con.ConnectionString = "data source =luminarias.mssql.somee.com ;initial catalog=luminarias;user id=JonathanFLF_SQLLogin_1;pwd=zgya9wozpl";
            //con.ConnectionString = "data source =DESKTOP-CREDOOS\\SQLEXPRESS ;DataBase=luminarias; integrated security=SSPI";
            //con.ConnectionString = "data source =JONATHAN-PC\\SQLEXPRESS ;DataBase=luminarias; integrated security=SSPI";
            //con.ConnectionString = "data source =LAPTOP-VHK1MAKD\\CONEXION ;DataBase=luminarias; integrated security=SSPI";
            con.ConnectionString = "data source=tadeo.colombiahosting.com.co\\MSSQLSERVER2019;initial catalog=lightcon_luminaria;user id=lightcon_lumin;pwd=luminaria2024*";
            //con.ConnectionString = "data source = luminaria.mssql.somee.com; initial catalog = luminaria; user id = hhjhhshsgsg_SQLLogin_2; pwd = cdrf7hhrrl";
        }

        // GET: pqrs
        public ActionResult pqrs()
        {
            return View();
        }

        public ActionResult EliminarPqrs(int id)
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "DELETE FROM pqrs WHERE Idpqrs = @Idpqrs";
            com.Parameters.AddWithValue("@Idpqrs", id);

            try
            {
                int rowsAffected = com.ExecuteNonQuery();
                con.Close();

                if (rowsAffected > 0)
                {
                    // PQRS eliminado exitosamente
                    ViewBag.DeleteMessage = "PQRS eliminado correctamente.";
                    return RedirectToAction("MostrarPqrs");
                }
                else
                {
                    // No se encontró ningún PQRS con el Id proporcionado
                    ViewBag.DeleteMessage = "No se encontró ningún PQRS con el Id proporcionado.";
                    return RedirectToAction("MostrarPqrs");
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción
                ViewBag.DeleteMessage = "Error al intentar eliminar el PQRS: " + ex.Message;
                return RedirectToAction("MostrarPqrs");
            }
        }

        [HttpPost]
        public ActionResult Enviarpqrs(PqrsModel pqrs, HttpPostedFileBase imagen)
        {
            try
            {
                byte[] data = null;

                // Verificar si se cargó una imagen
                if (imagen != null && imagen.ContentLength > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        imagen.InputStream.CopyTo(ms);
                        data = ms.ToArray();
                    }
                }
                else
                {
                    // Mensaje cuando no se adjunta una imagen
                    data = Encoding.UTF8.GetBytes("Sin recursos de imagen");
                }

                // Obtener la fecha y hora actual del sistema en la zona horaria de Colombia
                DateTime fechaRegistro = DateTime.UtcNow; // Obtener fecha y hora en UTC
                TimeZoneInfo tzColombia = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
                fechaRegistro = TimeZoneInfo.ConvertTimeFromUtc(fechaRegistro, tzColombia);

                connectionString();
                con.Open();
                com.Connection = con;

                com.CommandText = @"INSERT INTO pqrs (FechaRegistro, Tipopqrs, Canal, Nombre, Apellido, TipoDoc, Documento, Telefono, Correo, Referencia, DireccionAfectacion, BarrioAfectacion, TipoAlumbrado, DescripcionAfectacion, Imagen, Estado) 
                    VALUES (@FechaRegistro, @Tipopqrs, @Canal, @Nombre, @Apellido, @TipoDoc, @Documento, @Telefono, @Correo, @Referencia, @DireccionAfectacion, @BarrioAfectacion, @TipoAlumbrado, @DescripcionAfectacion, @Imagen, @Estado);
                    SELECT SCOPE_IDENTITY();";

                // Agregar parámetro para FechaRegistro
                com.Parameters.AddWithValue("@FechaRegistro", fechaRegistro);

                // Agregar demás parámetros según tu modelo PqrsModel
                com.Parameters.AddWithValue("@Tipopqrs", pqrs.Tipopqrs);
                com.Parameters.AddWithValue("@Canal", pqrs.Canal);
                com.Parameters.AddWithValue("@Nombre", pqrs.Nombre);
                com.Parameters.AddWithValue("@Apellido", pqrs.Apellido);
                com.Parameters.AddWithValue("@TipoDoc", pqrs.TipoDoc);
                com.Parameters.AddWithValue("@Documento", pqrs.Documento);
                com.Parameters.AddWithValue("@Telefono", pqrs.Telefono ?? (object)DBNull.Value);
                com.Parameters.AddWithValue("@Correo", pqrs.Correo ?? (object)DBNull.Value);
                com.Parameters.AddWithValue("@Referencia", string.IsNullOrEmpty(pqrs.Referencia) ? (object)DBNull.Value : pqrs.Referencia);
                com.Parameters.AddWithValue("@DireccionAfectacion", pqrs.DireccionAfectacion ?? (object)DBNull.Value);
                com.Parameters.AddWithValue("@BarrioAfectacion", pqrs.BarrioAfectacion ?? (object)DBNull.Value);
                com.Parameters.AddWithValue("@TipoAlumbrado", pqrs.TipoAlumbrado ?? (object)DBNull.Value);
                com.Parameters.AddWithValue("@DescripcionAfectacion", pqrs.DescripcionAfectacion ?? (object)DBNull.Value);
                com.Parameters.AddWithValue("@Imagen", data);
                com.Parameters.AddWithValue("@Estado", 1);

                int id = Convert.ToInt32(com.ExecuteScalar());

                con.Close();

                string consecutivo = "CH2024" + id.ToString();

                var response = new
                {
                    consecutivo = consecutivo
                };

                return Json(response);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error en el registro: " + ex.Message;
                return RedirectToAction("Inicio", "Login");
            }
        }



        public ActionResult MostrarPqrs()
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "SELECT pqrs.*, Estado.Nombre AS EstadoNombre FROM pqrs INNER JOIN Estado ON pqrs.Estado = Estado.IdEstado WHERE pqrs.Estado IN (1) ";
            SqlDataReader dr = com.ExecuteReader();

            // Lista para almacenar las pqrs
            List<PqrsModel> pqrsList = new List<PqrsModel>();

            // Obtener la zona horaria de Colombia
            TimeZoneInfo tzColombia = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

            // Leer los datos y añadirlos a la lista
            while (dr.Read())
            {
                PqrsModel pqrs = new PqrsModel();
                pqrs.Idpqrs = Convert.ToInt32(dr["Idpqrs"]);
                pqrs.Consecutivo = "CH2024" + pqrs.Idpqrs.ToString(); // Añadir el prefijo
                DateTime fechaRegistroUtc = Convert.ToDateTime(dr["FechaRegistro"]); // Fecha y hora en UTC desde la base de datos
                DateTime fechaRegistroColombia = TimeZoneInfo.ConvertTime(fechaRegistroUtc, tzColombia); // Convertir a la zona horaria de Colombia
                pqrs.FechaRegistro = fechaRegistroColombia.ToString(); // Convertir a string para mostrar en la vista
                pqrs.Tipopqrs = dr["Tipopqrs"].ToString();
                pqrs.Canal = dr["Canal"].ToString();
                pqrs.Nombre = dr["Nombre"].ToString();
                pqrs.Apellido = dr["Apellido"].ToString();
                pqrs.TipoDoc = dr["TipoDoc"].ToString();
                pqrs.Documento = dr["Documento"].ToString();
                pqrs.BarrioUsuario = dr["BarrioUsuario"].ToString();
                pqrs.Telefono = dr["Telefono"].ToString();
                pqrs.DireccionUsuario = dr["DireccionUsuario"].ToString();
                pqrs.Correo = dr["Correo"].ToString();
                pqrs.Referencia = dr["Referencia"].ToString();
                pqrs.DireccionAfectacion = dr["DireccionAfectacion"].ToString();
                pqrs.BarrioAfectacion = dr["BarrioAfectacion"].ToString();
                pqrs.TipoAlumbrado = dr["TipoAlumbrado"].ToString();
                pqrs.DescripcionAfectacion = dr["DescripcionAfectacion"].ToString();
                pqrs.EstadoNombre = dr["EstadoNombre"].ToString();

                // Manejar la imagen
                if (dr["Imagen"] != DBNull.Value)
                {
                    byte[] imageBytes = (byte[])dr["Imagen"];
                    pqrs.ImagenDataUrl = "data:image/png;base64," + Convert.ToBase64String(imageBytes);
                }
                else
                {
                    pqrs.ImagenDataUrl = "Sin recursos de imagen";
                }

                pqrsList.Add(pqrs);
            }

            con.Close();

            // Pasar la lista de pqrs a la vista
            return View(pqrsList);
        }

        public ActionResult pqrssinasignar_usuario()
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "SELECT pqrs.*, Estado.Nombre AS EstadoNombre FROM pqrs INNER JOIN Estado ON pqrs.Estado = Estado.IdEstado WHERE pqrs.Estado IN (1)";
            SqlDataReader dr = com.ExecuteReader();

            // Lista para almacenar las pqrs
            List<PqrsModel> pqrsList = new List<PqrsModel>();

            // Leer los datos y añadirlos a la lista
            while (dr.Read())
            {
                PqrsModel pqrs = new PqrsModel();
                pqrs.Idpqrs = Convert.ToInt32(dr["Idpqrs"]);
                pqrs.Consecutivo = "CH2024" + pqrs.Idpqrs.ToString(); // Añadir el prefijo
                pqrs.FechaRegistro = dr["FechaRegistro"].ToString();
                pqrs.Tipopqrs = dr["Tipopqrs"].ToString();
                pqrs.Canal = dr["Canal"].ToString();
                pqrs.Nombre = dr["Nombre"].ToString();
                pqrs.Apellido = dr["Apellido"].ToString();
                pqrs.TipoDoc = dr["TipoDoc"].ToString();
                pqrs.Documento = dr["Documento"].ToString();
                pqrs.BarrioUsuario = dr["BarrioUsuario"].ToString();
                pqrs.Telefono = dr["Telefono"].ToString();
                pqrs.DireccionUsuario = dr["DireccionUsuario"].ToString();
                pqrs.Correo = dr["Correo"].ToString();
                pqrs.Referencia = dr["Referencia"].ToString();
                pqrs.DireccionAfectacion = dr["DireccionAfectacion"].ToString();
                pqrs.BarrioAfectacion = dr["BarrioAfectacion"].ToString();
                pqrs.TipoAlumbrado = dr["TipoAlumbrado"].ToString();
                pqrs.DescripcionAfectacion = dr["DescripcionAfectacion"].ToString();
                pqrs.EstadoNombre = dr["EstadoNombre"].ToString();
                pqrsList.Add(pqrs);
            }

            con.Close();

            // Pasar la lista de pqrs filtradas a la vista
            return View(pqrsList);
        }

        public ActionResult pqrssinasignar()
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "SELECT pqrs.*, Estado.Nombre AS EstadoNombre FROM pqrs INNER JOIN Estado ON pqrs.Estado = Estado.IdEstado WHERE pqrs.Estado IN (1)";
            SqlDataReader dr = com.ExecuteReader();

            // Lista para almacenar las pqrs
            List<PqrsModel> pqrsList = new List<PqrsModel>();

            // Leer los datos y añadirlos a la lista
            while (dr.Read())
            {
                PqrsModel pqrs = new PqrsModel();
                pqrs.Idpqrs = Convert.ToInt32(dr["Idpqrs"]);
                pqrs.Consecutivo = "CH2024" + pqrs.Idpqrs.ToString(); // Añadir el prefijo
                pqrs.FechaRegistro = dr["FechaRegistro"].ToString();
                pqrs.Tipopqrs = dr["Tipopqrs"].ToString();
                pqrs.Canal = dr["Canal"].ToString();
                pqrs.Nombre = dr["Nombre"].ToString();
                pqrs.Apellido = dr["Apellido"].ToString();
                pqrs.TipoDoc = dr["TipoDoc"].ToString();
                pqrs.Documento = dr["Documento"].ToString();
                pqrs.BarrioUsuario = dr["BarrioUsuario"].ToString();
                pqrs.Telefono = dr["Telefono"].ToString();
                pqrs.DireccionUsuario = dr["DireccionUsuario"].ToString();
                pqrs.Correo = dr["Correo"].ToString();
                pqrs.Referencia = dr["Referencia"].ToString();
                pqrs.DireccionAfectacion = dr["DireccionAfectacion"].ToString();
                pqrs.BarrioAfectacion = dr["BarrioAfectacion"].ToString();
                pqrs.TipoAlumbrado = dr["TipoAlumbrado"].ToString();
                pqrs.DescripcionAfectacion = dr["DescripcionAfectacion"].ToString();
                pqrs.EstadoNombre = dr["EstadoNombre"].ToString();
                pqrsList.Add(pqrs);
            }

            con.Close();

            // Pasar la lista de pqrs filtradas a la vista
            return View(pqrsList);
        }

        public ActionResult pqrsasignada()
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "SELECT pqrs.*, Estado.Nombre AS EstadoNombre FROM pqrs INNER JOIN Estado ON pqrs.Estado = Estado.IdEstado WHERE pqrs.Estado IN (2)";
            SqlDataReader dr = com.ExecuteReader();

            // Lista para almacenar las pqrs
            List<PqrsModel> pqrsList = new List<PqrsModel>();

            // Leer los datos y añadirlos a la lista
            while (dr.Read())
            {
                PqrsModel pqrs = new PqrsModel();
                pqrs.Idpqrs = Convert.ToInt32(dr["Idpqrs"]);
                pqrs.Consecutivo = "CH2024" + pqrs.Idpqrs.ToString(); // Añadir el prefijo
                pqrs.FechaRegistro = dr["FechaRegistro"].ToString();
                pqrs.Tipopqrs = dr["Tipopqrs"].ToString();
                pqrs.Canal = dr["Canal"].ToString();
                pqrs.Nombre = dr["Nombre"].ToString();
                pqrs.Apellido = dr["Apellido"].ToString();
                pqrs.TipoDoc = dr["TipoDoc"].ToString();
                pqrs.Documento = dr["Documento"].ToString();
                pqrs.BarrioUsuario = dr["BarrioUsuario"].ToString();
                pqrs.Telefono = dr["Telefono"].ToString();
                pqrs.DireccionUsuario = dr["DireccionUsuario"].ToString();
                pqrs.Correo = dr["Correo"].ToString();
                pqrs.Referencia = dr["Referencia"].ToString();
                pqrs.DireccionAfectacion = dr["DireccionAfectacion"].ToString();
                pqrs.BarrioAfectacion = dr["BarrioAfectacion"].ToString();
                pqrs.TipoAlumbrado = dr["TipoAlumbrado"].ToString();
                pqrs.DescripcionAfectacion = dr["DescripcionAfectacion"].ToString();
                pqrs.EstadoNombre = dr["EstadoNombre"].ToString();
                pqrsList.Add(pqrs);
            }

            con.Close();

            // Pasar la lista de pqrs filtradas a la vista
            return View(pqrsList);
        }

        public ActionResult pqrsresuelta()
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "SELECT pqrs.*, Estado.Nombre AS EstadoNombre FROM pqrs INNER JOIN Estado ON pqrs.Estado = Estado.IdEstado WHERE pqrs.Estado IN (3)";
            SqlDataReader dr = com.ExecuteReader();

            // Lista para almacenar las pqrs
            List<PqrsModel> pqrsList = new List<PqrsModel>();

            // Leer los datos y añadirlos a la lista
            while (dr.Read())
            {
                PqrsModel pqrs = new PqrsModel();
                pqrs.Idpqrs = Convert.ToInt32(dr["Idpqrs"]);
                pqrs.Consecutivo = "CH2024" + pqrs.Idpqrs.ToString(); // Añadir el prefijo
                pqrs.FechaRegistro = dr["FechaRegistro"].ToString();
                pqrs.Tipopqrs = dr["Tipopqrs"].ToString();
                pqrs.Canal = dr["Canal"].ToString();
                pqrs.Nombre = dr["Nombre"].ToString();
                pqrs.Apellido = dr["Apellido"].ToString();
                pqrs.TipoDoc = dr["TipoDoc"].ToString();
                pqrs.Documento = dr["Documento"].ToString();
                pqrs.BarrioUsuario = dr["BarrioUsuario"].ToString();
                pqrs.Telefono = dr["Telefono"].ToString();
                pqrs.DireccionUsuario = dr["DireccionUsuario"].ToString();
                pqrs.Correo = dr["Correo"].ToString();
                pqrs.Referencia = dr["Referencia"].ToString();
                pqrs.DireccionAfectacion = dr["DireccionAfectacion"].ToString();
                pqrs.BarrioAfectacion = dr["BarrioAfectacion"].ToString();
                pqrs.TipoAlumbrado = dr["TipoAlumbrado"].ToString();
                pqrs.DescripcionAfectacion = dr["DescripcionAfectacion"].ToString();
                pqrs.EstadoNombre = dr["EstadoNombre"].ToString();
                pqrsList.Add(pqrs);
            }

            con.Close();

            // Pasar la lista de pqrs filtradas a la vista
            return View(pqrsList);
        }

        public ActionResult BuscarPqrs(string tipoBusqueda, string desde, string hasta, string Idpqrs)
        {
            connectionString();
            con.Open();
            com.Connection = con;

            // Lógica para buscar PQRS según los parámetros recibidos
            if (tipoBusqueda == "consecutivo")
            {
                // Si la búsqueda es por consecutivo
                com.CommandText = "SELECT pqrs.*, Estado.Nombre AS EstadoNombre FROM pqrs INNER JOIN Estado ON pqrs.Estado = Estado.IdEstado WHERE pqrs.Idpqrs = @Idpqrs";
                com.Parameters.AddWithValue("@Idpqrs", Idpqrs);
            }
            else if (tipoBusqueda == "fecha")
            {
                // Si la búsqueda es por fecha
                com.CommandText = "SELECT pqrs.*, Estado.Nombre AS EstadoNombre FROM pqrs INNER JOIN Estado ON pqrs.Estado = Estado.IdEstado WHERE pqrs.FechaRegistro BETWEEN @Desde AND @Hasta";
                com.Parameters.AddWithValue("@Desde", desde);
                com.Parameters.AddWithValue("@Hasta", hasta);
            }

            SqlDataReader dr = com.ExecuteReader();

            // Lista para almacenar los resultados de la búsqueda
            List<PqrsModel> resultados = new List<PqrsModel>();

            // Leer los datos y añadirlos a la lista de resultados
            while (dr.Read())
            {
                PqrsModel pqrs = new PqrsModel();
                pqrs.Idpqrs = Convert.ToInt32(dr["Idpqrs"]);
                pqrs.Consecutivo = "CH2024" + pqrs.Idpqrs.ToString(); // Añadir el prefijo
                pqrs.FechaRegistro = dr["FechaRegistro"].ToString();
                pqrs.Tipopqrs = dr["Tipopqrs"].ToString();
                pqrs.Canal = dr["Canal"].ToString();
                pqrs.Nombre = dr["Nombre"].ToString();
                pqrs.Apellido = dr["Apellido"].ToString();
                pqrs.TipoDoc = dr["TipoDoc"].ToString();
                pqrs.Documento = dr["Documento"].ToString();
                pqrs.BarrioUsuario = dr["BarrioUsuario"].ToString();
                pqrs.Telefono = dr["Telefono"].ToString();
                pqrs.DireccionUsuario = dr["DireccionUsuario"].ToString();
                pqrs.Correo = dr["Correo"].ToString();
                pqrs.Referencia = dr["Referencia"].ToString();
                pqrs.DireccionAfectacion = dr["DireccionAfectacion"].ToString();
                pqrs.BarrioAfectacion = dr["BarrioAfectacion"].ToString();
                pqrs.TipoAlumbrado = dr["TipoAlumbrado"].ToString();
                pqrs.DescripcionAfectacion = dr["DescripcionAfectacion"].ToString();
                pqrs.EstadoNombre = dr["EstadoNombre"].ToString();
                resultados.Add(pqrs);
            }

            con.Close();

            // Pasar la lista de resultados a la vista
            return View("archivopqrs", resultados);
        }

        // Método para generar el PDF
        public ActionResult GenerarPdf()
        {
            // Obtener el listado de PQRS desde el modelo
            List<PqrsModel> pqrsList = ObtenerPqrs();

            // Crear un documento PDF
            Document doc = new Document(PageSize.A4, 10, 10, 10, 10);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            // Abrir el documento para escritura
            doc.Open();

            // Agregar logo
            string logoPath = Server.MapPath("~/Content/img/logo.jpg");
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
            logo.ScaleToFit(100, 100);
            logo.Alignment = Element.ALIGN_LEFT;
            doc.Add(logo);

            // Agregar hora de descarga
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var timestampFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            doc.Add(new Paragraph("Hora de descarga: " + timestamp, timestampFont));

            // Agregar título
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 24);
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            doc.Add(new Paragraph("LIGHT CONTROL", titleFont));
            doc.Add(new Paragraph(" ", bodyFont)); // Agregar un espacio

            // Crear una tabla con el número de columnas necesarias
            PdfPTable table = new PdfPTable(5); // Cambia el número de columnas según tus necesidades
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 1, 2, 2, 3, 2 }); // Ajustar los anchos de columna si es necesario

            // Agregar encabezados a la tabla
            PdfPCell cell = new PdfPCell(new Phrase("ID", headerFont));
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Fecha Registro", headerFont));
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Tipo PQRS", headerFont));
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Nombre", headerFont));
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Estado", headerFont));
            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            // Agregar filas a la tabla
            foreach (var pqrs in pqrsList)
            {
                table.AddCell(new Phrase(pqrs.Idpqrs.ToString(), bodyFont));
                table.AddCell(new Phrase(pqrs.FechaRegistro, bodyFont));
                table.AddCell(new Phrase(pqrs.Tipopqrs, bodyFont));
                table.AddCell(new Phrase($"{pqrs.Nombre} {pqrs.Apellido}", bodyFont));
                table.AddCell(new Phrase(pqrs.EstadoNombre, bodyFont));
            }

            // Agregar la tabla al documento
            doc.Add(table);

            // Cerrar el documento
            doc.Close();

            byte[] file = ms.ToArray();
            ms.Close();

            // Devolver el archivo PDF como una respuesta de descarga
            return File(file, "application/pdf", "ReportePQRS.pdf");
        }

        private List<PqrsModel> ObtenerPqrs()
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "SELECT pqrs.*, Estado.Nombre AS EstadoNombre FROM pqrs INNER JOIN Estado ON pqrs.Estado = Estado.IdEstado";
            SqlDataReader dr = com.ExecuteReader();

            List<PqrsModel> pqrsList = new List<PqrsModel>();
            while (dr.Read())
            {
                PqrsModel pqrs = new PqrsModel();
                pqrs.Idpqrs = Convert.ToInt32(dr["Idpqrs"]);
                pqrs.FechaRegistro = dr["FechaRegistro"].ToString();
                pqrs.Tipopqrs = dr["Tipopqrs"].ToString();
                pqrs.Canal = dr["Canal"].ToString();
                pqrs.Nombre = dr["Nombre"].ToString();
                pqrs.Apellido = dr["Apellido"].ToString();
                pqrs.TipoDoc = dr["TipoDoc"].ToString();
                pqrs.Documento = dr["Documento"].ToString();
                pqrs.BarrioUsuario = dr["BarrioUsuario"].ToString();
                pqrs.Telefono = dr["Telefono"].ToString();
                pqrs.DireccionUsuario = dr["DireccionUsuario"].ToString();
                pqrs.Correo = dr["Correo"].ToString();
                pqrs.Referencia = dr["Referencia"].ToString();
                pqrs.DireccionAfectacion = dr["DireccionAfectacion"].ToString();
                pqrs.BarrioAfectacion = dr["BarrioAfectacion"].ToString();
                pqrs.TipoAlumbrado = dr["TipoAlumbrado"].ToString();
                pqrs.DescripcionAfectacion = dr["DescripcionAfectacion"].ToString();
                pqrs.EstadoNombre = dr["EstadoNombre"].ToString();
                pqrsList.Add(pqrs);
            }
            con.Close();
            return pqrsList;

        }
        public ActionResult VerInfo(int id)
        {
            // Crear una instancia del modelo PQRS para almacenar la información
            PqrsModel pqrs = new PqrsModel();

            // Establecer la conexión con la base de datos
            connectionString();
            con.Open();
            com.Connection = con;

            // Consultar la información del PQRS según el ID proporcionado
            com.CommandText = "SELECT pqrs.*, Estado.Nombre AS EstadoNombre ,pqrs.Imagen AS Imagen FROM pqrs INNER JOIN Estado ON pqrs.Estado = Estado.IdEstado WHERE pqrs.Idpqrs = @Idpqrs";
            com.Parameters.AddWithValue("@Idpqrs", id);

            try
            {
                // Ejecutar la consulta y obtener un lector de datos
                SqlDataReader dr = com.ExecuteReader();

                // Verificar si se encontraron resultados
                if (dr.Read())
                {
                    // Obtener los valores de las columnas y asignarlos al modelo PQRS
                    pqrs.Idpqrs = Convert.ToInt32(dr["Idpqrs"]);
                    pqrs.FechaRegistro = dr["FechaRegistro"].ToString();
                    pqrs.Tipopqrs = dr["Tipopqrs"].ToString();
                    pqrs.Canal = dr["Canal"].ToString();
                    pqrs.Nombre = dr["Nombre"].ToString();
                    pqrs.Apellido = dr["Apellido"].ToString();
                    pqrs.TipoDoc = dr["TipoDoc"].ToString();
                    pqrs.Documento = dr["Documento"].ToString();
                    pqrs.BarrioUsuario = dr["BarrioUsuario"].ToString();
                    pqrs.Telefono = dr["Telefono"].ToString();
                    pqrs.DireccionUsuario = dr["DireccionUsuario"].ToString();
                    pqrs.Correo = dr["Correo"].ToString();
                    pqrs.Referencia = dr["Referencia"].ToString();
                    pqrs.DireccionAfectacion = dr["DireccionAfectacion"].ToString();
                    pqrs.BarrioAfectacion = dr["BarrioAfectacion"].ToString();
                    pqrs.TipoAlumbrado = dr["TipoAlumbrado"].ToString();
                    pqrs.DescripcionAfectacion = dr["DescripcionAfectacion"].ToString();
                    pqrs.Estado = Convert.ToInt32(dr["Estado"]);
                    pqrs.EstadoNombre = dr["EstadoNombre"].ToString();

                    pqrs.img = (byte[])dr["Imagen"];

                    string imagenDataUrl = Convert.ToBase64String(pqrs.img);
                    ViewBag.ImagenDataUrl = "data:image/jpeg;base64," + imagenDataUrl;

                }
                else
                {

                }

                // Cerrar la conexión con la base de datos
                con.Close();
            }
            catch (Exception ex)
            {

            }


            return View(pqrs);
        }

        public ActionResult VerInfo_usuario(int id)
        {
            // Crear una instancia del modelo PQRS para almacenar la información
            PqrsModel pqrs = new PqrsModel();

            // Establecer la conexión con la base de datos
            connectionString();
            con.Open();
            com.Connection = con;

            // Consultar la información del PQRS según el ID proporcionado
            com.CommandText = "SELECT pqrs.*, Estado.Nombre AS EstadoNombre ,pqrs.Imagen AS Imagen FROM pqrs INNER JOIN Estado ON pqrs.Estado = Estado.IdEstado WHERE pqrs.Idpqrs = @Idpqrs";
            com.Parameters.AddWithValue("@Idpqrs", id);

            try
            {
                // Ejecutar la consulta y obtener un lector de datos
                SqlDataReader dr = com.ExecuteReader();

                // Verificar si se encontraron resultados
                if (dr.Read())
                {
                    // Obtener los valores de las columnas y asignarlos al modelo PQRS
                    pqrs.Idpqrs = Convert.ToInt32(dr["Idpqrs"]);
                    pqrs.FechaRegistro = dr["FechaRegistro"].ToString();
                    pqrs.Tipopqrs = dr["Tipopqrs"].ToString();
                    pqrs.Canal = dr["Canal"].ToString();
                    pqrs.Nombre = dr["Nombre"].ToString();
                    pqrs.Apellido = dr["Apellido"].ToString();
                    pqrs.TipoDoc = dr["TipoDoc"].ToString();
                    pqrs.Documento = dr["Documento"].ToString();
                    pqrs.BarrioUsuario = dr["BarrioUsuario"].ToString();
                    pqrs.Telefono = dr["Telefono"].ToString();
                    pqrs.DireccionUsuario = dr["DireccionUsuario"].ToString();
                    pqrs.Correo = dr["Correo"].ToString();
                    pqrs.Referencia = dr["Referencia"].ToString();
                    pqrs.DireccionAfectacion = dr["DireccionAfectacion"].ToString();
                    pqrs.BarrioAfectacion = dr["BarrioAfectacion"].ToString();
                    pqrs.TipoAlumbrado = dr["TipoAlumbrado"].ToString();
                    pqrs.DescripcionAfectacion = dr["DescripcionAfectacion"].ToString();
                    pqrs.Estado = Convert.ToInt32(dr["Estado"]);
                    pqrs.EstadoNombre = dr["EstadoNombre"].ToString();

                    pqrs.img = (byte[])dr["Imagen"];

                    string imagenDataUrl = Convert.ToBase64String(pqrs.img);
                    ViewBag.ImagenDataUrl = "data:image/jpeg;base64," + imagenDataUrl;

                }
                else
                {

                }

                // Cerrar la conexión con la base de datos
                con.Close();
            }
            catch (Exception ex)
            {

            }


            return View(pqrs);
        }

        public ActionResult GraficoTipoPqrs()
        {
            // Obtener los datos de PQRS desde el modelo
            List<PqrsModel> pqrsList = ObtenerPqrs();

            // Contar el número de PQRS por tipo
            var tipoPqrsCounts = pqrsList.GroupBy(p => p.Tipopqrs)
                                         .Select(g => new { TipoPqrs = g.Key, Count = g.Count() })
                                         .ToList();

            // Convertir los datos en un formato adecuado para el gráfico de barras
            var labels = tipoPqrsCounts.Select(x => x.TipoPqrs).ToArray();
            var data = tipoPqrsCounts.Select(x => x.Count).ToArray();

            // Pasar los datos al modelo de vista
            ViewBag.TipoPqrsLabels = labels;
            ViewBag.TipoPqrsData = data;

            return View("GraficoPqrs");
        }

        public ActionResult GraficoTipoBarrio()
        {
            // Obtener los datos de PQRS desde el modelo
            List<PqrsModel> pqrsList = ObtenerPqrs();

            // Contar el número de PQRS por tipo de barrio
            var tipoBarrioCounts = pqrsList.GroupBy(p => p.BarrioAfectacion)
                                           .Select(g => new { TipoBarrio = g.Key, Count = g.Count() })
                                           .ToList();

            // Convertir los datos en un formato adecuado para el gráfico de barras
            var labels = tipoBarrioCounts.Select(x => x.TipoBarrio).ToArray();
            var data = tipoBarrioCounts.Select(x => x.Count).ToArray();

            // Pasar los datos al modelo de vista
            ViewBag.TipoBarrioLabels = labels;
            ViewBag.TipoBarrioData = data;

            return View("GraficoPqrs");
        }

        public ActionResult GraficoTipoCanal()
        {
            // Obtener los datos de PQRS desde el modelo
            List<PqrsModel> pqrsList = ObtenerPqrs();

            // Contar el número de PQRS por tipo de canal
            var tipoCanalCounts = pqrsList.GroupBy(p => p.Canal)
                                          .Select(g => new { TipoCanal = g.Key, Count = g.Count() })
                                          .ToList();

            // Convertir los datos en un formato adecuado para el gráfico de barras
            var labels = tipoCanalCounts.Select(x => x.TipoCanal).ToArray();
            var data = tipoCanalCounts.Select(x => x.Count).ToArray();

            // Pasar los datos al modelo de vista
            ViewBag.TipoCanalLabels = labels;
            ViewBag.TipoCanalData = data;

            return View("GraficoPqrs");
        }

        public ActionResult GraficoPorMes()
        {
            // Obtener los datos de PQRS desde el modelo
            List<PqrsModel> pqrsList = ObtenerPqrs();

            // Contar el número de PQRS por mes
            //var pqrsPorMes = pqrsList.GroupBy(p => new { Mes = p.FechaRegistro.Month, Anio = p.FechaRegistro.Year })
            //.Select(g => new { Mes = $"{g.Key.Mes}-{g.Key.Anio}", Count = g.Count() })
            //   .ToList();

            // Convertir los datos en un formato adecuado para el gráfico de barras
            //var labels = pqrsPorMes.Select(x => x.Mes).ToArray();
            //var data = pqrsPorMes.Select(x => x.Count).ToArray();

            // Pasar los datos al modelo de vista
            //  ViewBag.PqrsPorMesLabels = labels;
            // ViewBag.PqrsPorMesData = data;

            return View("GraficoPqrs");
        }

        public ActionResult GraficoPqrs()
        {
            // Obtener los datos para el gráfico por tipo de PQRS
            var tiposPqrs = ObtenerDatosPorTipoPqrs();

            // Obtener los datos para el gráfico por barrio de afectación
            var barriosAfectacion = ObtenerDatosPorBarrioAfectacion();

            // Obtener los datos para el gráfico por canal
            var canales = ObtenerDatosPorCanal();

            // Obtener los datos para el gráfico por fecha de registro
            var fechasRegistro = ObtenerDatosPorFechaRegistro();

            // Pasar los datos a la vista
            ViewBag.TiposPqrs = JsonConvert.SerializeObject(tiposPqrs);
            ViewBag.BarriosAfectacion = JsonConvert.SerializeObject(barriosAfectacion);
            ViewBag.Canales = JsonConvert.SerializeObject(canales);
            ViewBag.FechasRegistro = JsonConvert.SerializeObject(fechasRegistro);

            return View();
        }

        private List<object> ObtenerDatosPorTipoPqrs()
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "SELECT Tipopqrs, COUNT(*) AS Total FROM pqrs GROUP BY Tipopqrs";
            SqlDataReader dr = com.ExecuteReader();

            List<object> data = new List<object>();
            while (dr.Read())
            {
                var item = new
                {
                    TipoPqrs = dr["Tipopqrs"].ToString(),
                    Total = Convert.ToInt32(dr["Total"])
                };
                data.Add(item);
            }

            con.Close();
            return data;
        }

        private List<object> ObtenerDatosPorBarrioAfectacion()
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "SELECT BarrioAfectacion, COUNT(*) AS Total FROM pqrs GROUP BY BarrioAfectacion";
            SqlDataReader dr = com.ExecuteReader();

            List<object> data = new List<object>();
            while (dr.Read())
            {
                var item = new
                {
                    BarrioAfectacion = dr["BarrioAfectacion"].ToString(),
                    Total = Convert.ToInt32(dr["Total"])
                };
                data.Add(item);
            }

            con.Close();
            return data;
        }

        private List<object> ObtenerDatosPorCanal()
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "SELECT Canal, COUNT(*) AS Total FROM pqrs GROUP BY Canal";
            SqlDataReader dr = com.ExecuteReader();

            List<object> data = new List<object>();
            while (dr.Read())
            {
                var item = new
                {
                    Canal = dr["Canal"].ToString(),
                    Total = Convert.ToInt32(dr["Total"])
                };
                data.Add(item);
            }

            con.Close();
            return data;
        }

        private List<object> ObtenerDatosPorFechaRegistro()
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "SELECT CONVERT(date, FechaRegistro) AS Fecha, COUNT(*) AS Total FROM pqrs GROUP BY CONVERT(date, FechaRegistro)";
            SqlDataReader dr = com.ExecuteReader();

            List<object> data = new List<object>();
            while (dr.Read())
            {
                var item = new
                {
                    Fecha = Convert.ToDateTime(dr["Fecha"]).ToString("yyyy-MM-dd"),
                    Total = Convert.ToInt32(dr["Total"])
                };
                data.Add(item);
            }

            con.Close();
            return data;
        }

        public ActionResult ArchivoPqrs()
        {
            return View("archivopqrs");
        }
    }
}
