using controlLuces.Models;
using controlLuces.permisos;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Font = iTextSharp.text.Font;

namespace controlLuces.Controllers
{
    public class OrdenesController : Controller
    {
        // GET: Ordenes

        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;


        void connectionString()
        {
            //con.ConnectionString = "data source =luminarias.mssql.somee.com ;initial catalog=luminarias;user id=JonathanFLF_SQLLogin_1;pwd=zgya9wozpl";
            //con.ConnectionString = "data source =LAPTOP-VHK1MAKD\\CONEXION ;DataBase=luminarias; integrated security=SSPI";
            con.ConnectionString = "data source=tadeo.colombiahosting.com.co\\MSSQLSERVER2019;initial catalog=lightcon_luminaria;user id=lightcon_lumin;pwd=luminaria2024*";
            //con.ConnectionString = "data source = luminaria.mssql.somee.com; initial catalog = luminaria; user id = hhjhhshsgsg_SQLLogin_2; pwd = cdrf7hhrrl";
            //con.ConnectionString = "data source =JONATHAN-PC\\SQLEXPRESS ;DataBase=luminarias; integrated security=SSPI";
        }

        public ActionResult monitorear()
        {
            return View();
        }

        public ActionResult verinfo(int id)
        {

            OrdenModel Orden = new OrdenModel();


            connectionString();
            con.Open();
            com.Connection = con;

            // Consultar la información del PQRS según el ID proporcionado
            // Consultar la información de la orden según el ID proporcionado
            com.CommandText = "SELECT ordenes_de_servicio.*, Ordenes_Cerradas.Descripcion AS DescripcionCerrada, Ordenes_Cerradas.Recursos AS RecursosCerrados, Estado.Nombre AS EstadoNombre FROM ordenes_de_servicio INNER JOIN Estado ON ordenes_de_servicio.IdEstado = Estado.IdEstado LEFT JOIN Ordenes_Cerradas ON ordenes_de_servicio.id_orden = Ordenes_Cerradas.Id_Orden_Servicio WHERE ordenes_de_servicio.id_orden = @Idorden";
            com.Parameters.AddWithValue("@Idorden", id);


            try
            {
                // Ejecutar la consulta y obtener un lector de datos
                SqlDataReader dr = com.ExecuteReader();

                // Verificar si se encontraron resultados
                if (dr.Read())
                {


                    // Obtener los valores de las columnas y asignarlos al modelo PQRS
                    Orden.IdOrden = Convert.ToInt32(dr["id_orden"]);

                    Orden.IdEstado = Convert.ToInt32(dr["IdEstado"]);
                    Orden.CodigoDeElemento = dr["codigo_de_elemento"].ToString();
                    Orden.ElementoRelacionado = dr["elemento_relacionado"].ToString();
                    Orden.ProblemaRelacionado = dr["problema_relacionado"].ToString();
                    Orden.ProblemaValidado = dr["problema_validado"].ToString();

                    Orden.TipoDeElemento = dr["Tipo_de_elemento"].ToString();
                    Orden.TipoDeSolucion = dr["tipo_de_Solucion"].ToString();
                    Orden.TipoDeOrden = dr["tipo_de_orden"].ToString();
                    Orden.Cuadrilla = dr["cuadrilla"].ToString();
                    Orden.ObraRelacionada = dr["obra_relacionada"].ToString();
                    Orden.OrdenPrioridad = dr["Orden_prioridad"].ToString();
                    Orden.ClaseDeOrden = dr["clase_de_orden"].ToString();
                    Orden.EstadoNombre = dr["EstadoNombre"].ToString();
                    Orden.FechaARealizar = Convert.ToDateTime(dr["fecha_a_realizar"]);
                    Orden.Descripcion = dr["DescripcionCerrada"].ToString();
                    Orden.Recursos = (byte[])dr["RecursosCerrados"];

                    string imagenDataUrl = Convert.ToBase64String(Orden.Recursos);
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


            return View(Orden);

        }
        [PermisosRol(Rol.Administrador, Rol.Tecnico)]
        public ActionResult verOrdenes()
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "SELECT * from ordenes_de_servicio";
            SqlDataReader dr = com.ExecuteReader();

            // Lista para almacenar las órdenes de servicio
            List<OrdenModel> OrdenesList = new List<OrdenModel>();

            // Leer los datos y añadirlos a la lista
            while (dr.Read())
            {
                OrdenModel Orden = new OrdenModel
                {
                    IdOrden = dr["id_orden"] != DBNull.Value ? Convert.ToInt32(dr["id_orden"]) : 0,
                    TipoDeElemento = dr["Tipo_de_elemento"] != DBNull.Value ? dr["Tipo_de_elemento"].ToString() : string.Empty,
                    CodigoDeElemento = dr["codigo_de_elemento"] != DBNull.Value ? dr["codigo_de_elemento"].ToString() : string.Empty,
                    ElementoRelacionado = dr["elemento_relacionado"] != DBNull.Value ? dr["elemento_relacionado"].ToString() : string.Empty,
                    CodigoOrden = dr["codigo_orden"] != DBNull.Value ? dr["codigo_orden"].ToString() : string.Empty,
                    ProblemaRelacionado = dr["problema_relacionado"] != DBNull.Value ? dr["problema_relacionado"].ToString() : string.Empty,
                    ProblemaValidado = dr["problema_validado"] != DBNull.Value ? dr["problema_validado"].ToString() : string.Empty,
                    PrioridadDeRuta = dr["prioridad_de_ruta"] != DBNull.Value ? Convert.ToInt32(dr["prioridad_de_ruta"]) : 0,
                    FechaARealizar = dr["fecha_a_realizar"] != DBNull.Value ? Convert.ToDateTime(dr["fecha_a_realizar"]) : DateTime.MinValue,
                    Cuadrilla = dr["cuadrilla"] != DBNull.Value ? dr["cuadrilla"].ToString() : string.Empty,
                    TipoDeOrden = dr["tipo_de_orden"] != DBNull.Value ? dr["tipo_de_orden"].ToString() : string.Empty,
                    TipoDeSolucion = dr["tipo_de_Solucion"] != DBNull.Value ? dr["tipo_de_Solucion"].ToString() : string.Empty,
                    ClaseDeOrden = dr["clase_de_orden"] != DBNull.Value ? dr["clase_de_orden"].ToString() : string.Empty,
                    ObraRelacionada = dr["obra_relacionada"] != DBNull.Value ? dr["obra_relacionada"].ToString() : string.Empty,
                    OrdenPrioridad = dr["orden_prioridad"] != DBNull.Value ? dr["orden_prioridad"].ToString() : string.Empty
                };

                OrdenesList.Add(Orden);
            }

            con.Close();

            // Ordenar la lista de órdenes por FechaARealizar de menor a mayor
            OrdenesList = OrdenesList.OrderBy(o => o.FechaARealizar).ToList();

            // Debug: Imprimir las fechas para verificar la ordenación
            foreach (var orden in OrdenesList)
            {
                Console.WriteLine(orden.FechaARealizar.ToString("yyyy-MM-dd"));
            }

            // Pasar la lista de órdenes a la vista
            return View(OrdenesList);
        }


        public ActionResult DescargarOrdenesPDF()
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "SELECT * from ordenes_de_servicio";
            SqlDataReader dr = com.ExecuteReader();

            // Lista para almacenar las órdenes de servicio
            List<OrdenModel> OrdenesList = new List<OrdenModel>();

            // Leer los datos y añadirlos a la lista
            while (dr.Read())
            {
                OrdenModel Orden = new OrdenModel
                {
                    IdOrden = Convert.ToInt32(dr["id_orden"]),
                    TipoDeElemento = dr["Tipo_de_elemento"].ToString(),
                    CodigoDeElemento = dr["codigo_de_elemento"].ToString(),
                    ElementoRelacionado = dr["elemento_relacionado"].ToString(),
                    CodigoOrden = dr["codigo_orden"].ToString(),
                    ProblemaRelacionado = dr["problema_relacionado"].ToString(),
                    ProblemaValidado = dr["problema_validado"].ToString(),
                    PrioridadDeRuta = Convert.ToInt32(dr["prioridad_de_ruta"]),
                    FechaARealizar = Convert.ToDateTime(dr["fecha_a_realizar"]),
                    Cuadrilla = dr["cuadrilla"].ToString(),
                    TipoDeOrden = dr["tipo_de_orden"].ToString(),
                    TipoDeSolucion = dr["tipo_de_Solucion"].ToString(),
                    ClaseDeOrden = dr["clase_de_orden"].ToString(),
                    ObraRelacionada = dr["obra_relacionada"].ToString(),
                    OrdenPrioridad = dr["orden_prioridad"].ToString()
                };

                OrdenesList.Add(Orden);
            }

            con.Close();

            // Generar el PDF
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Título del documento
                Paragraph title = new Paragraph("Listado de Órdenes de Servicio", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 18, Font.BOLD));
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 20;
                document.Add(title);

                // Tabla con las órdenes de servicio
                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 1, 2, 3, 2, 2, 2 });

                // Encabezados de la tabla
                table.AddCell("Consecutivo");
                table.AddCell("Fecha");
                table.AddCell("Descripción");
                table.AddCell("Pqrs Relacionada");
                table.AddCell("Cuadrilla");
                table.AddCell("Prioridad");

                // Datos de la tabla
                foreach (var orden in OrdenesList)
                {
                    table.AddCell(orden.IdOrden.ToString());
                    table.AddCell(orden.FechaARealizar.ToString("dd/MM/yyyy"));
                    table.AddCell(orden.ProblemaRelacionado);
                    table.AddCell(orden.ElementoRelacionado);
                    table.AddCell(orden.Cuadrilla);
                    table.AddCell(orden.OrdenPrioridad);
                }

                document.Add(table);
                document.Close();
                writer.Close();

                // Descargar el PDF
                byte[] fileBytes = memoryStream.ToArray();
                return File(fileBytes, "application/pdf", "OrdenesDeServicio.pdf");
            }

        }
        [PermisosRol(Rol.Administrador, Rol.Tecnico)]
        
        public ActionResult insertarOrdenMan(OrdenModel orden)
        {
            orden.ClaseDeOrden = "Mantenimiento";

            try
            {
                connectionString();
                con.Open();
                com.Connection = con;

                // Convertir la fecha a UTC antes de insertarla
                DateTime fechaUTC = orden.FechaARealizar.ToUniversalTime();

                string query = @"INSERT INTO ordenes_de_servicio 
        (Tipo_de_elemento, codigo_de_elemento, elemento_relacionado, 
        problema_relacionado, problema_validado, prioridad_de_ruta, Orden_prioridad,
        fecha_a_realizar, cuadrilla, tipo_de_orden, tipo_de_Solucion, clase_de_orden, IdEstado) 
        VALUES 
        (@TipoDeElemento, @CodigoDeElemento, @ElementoRelacionado, 
        @ProblemaRelacionado, @ProblemaValidado, @PrioridadDeRuta, @OrdenPrioridad,
        @FechaARealizar, @Cuadrilla, @TipoDeOrden, @TipoDeSolucion, @ClaseDeOrden, @Idestado);
        SELECT SCOPE_IDENTITY();"; // Obtener el ID de la orden generada

                com.CommandText = query;
                com.Parameters.AddWithValue("@TipoDeElemento", orden.TipoDeElemento);
                com.Parameters.AddWithValue("@CodigoDeElemento", orden.CodigoDeElemento);
                com.Parameters.AddWithValue("@ElementoRelacionado", orden.ElementoRelacionado);
                com.Parameters.AddWithValue("@ProblemaRelacionado", orden.ProblemaRelacionado);
                com.Parameters.AddWithValue("@ProblemaValidado", orden.ProblemaValidado);
                com.Parameters.AddWithValue("@OrdenPrioridad", orden.OrdenPrioridad);
                com.Parameters.AddWithValue("@PrioridadDeRuta", orden.PrioridadDeRuta);
                com.Parameters.AddWithValue("@FechaARealizar", fechaUTC);
                com.Parameters.AddWithValue("@Cuadrilla", orden.Cuadrilla);
                com.Parameters.AddWithValue("@TipoDeOrden", orden.TipoDeOrden);
                com.Parameters.AddWithValue("@TipoDeSolucion", orden.TipoDeSolucion);
                com.Parameters.AddWithValue("@ClaseDeOrden", orden.ClaseDeOrden);
                com.Parameters.AddWithValue("@idestado", 2);

                if (orden.PrioridadDeRuta == 0)
                {
                    com.Parameters["@PrioridadDeRuta"].Value = 0;
                }
                else
                {
                    com.Parameters["@PrioridadDeRuta"].Value = orden.PrioridadDeRuta;
                }

                // Ejecutar la consulta SQL de inserción y obtener el ID de la orden generada
                var ordenId = com.ExecuteScalar();

                // Consulta para actualizar el estado en la tabla pqrs
                com.CommandText = "UPDATE pqrs SET Estado = @NuevoEstado WHERE Idpqrs = @idpqrs";
                com.Parameters.AddWithValue("@NuevoEstado", 2); // Cambia el valor 2 según corresponda
                com.Parameters.AddWithValue("@idpqrs", orden.ElementoRelacionado);

                int rowsUpdated = com.ExecuteNonQuery();

                if (rowsUpdated > 0)
                {
                    return Json(new { success = true, ordenId = ordenId }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Error al actualizar el estado en la tabla pqrs" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al crear la orden de servicio: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult generar()
        {
            return View();
        }
        [PermisosRol(Rol.Administrador, Rol.Tecnico)]

        public ActionResult insertarOrdenMon(OrdenModel orden)
        {
            orden.ClaseDeOrden = "Montaje";

            try
            {
                // Abrir la conexión
                connectionString();
                con.Open();
                com.Connection = con;

                // Construir la consulta SQL INSERT parametrizada para prevenir la inyección de SQL
                string query = @"INSERT INTO ordenes_de_servicio 
                 (Tipo_de_elemento, codigo_de_elemento, elemento_relacionado, 
                  problema_relacionado, problema_validado, prioridad_de_ruta, Orden_prioridad,
                  fecha_a_realizar, cuadrilla, obra_relacionada, clase_de_orden, IdEstado) 
                 VALUES 
                 (@TipoDeElemento, @CodigoDeElemento, @ElementoRelacionado, 
                  @ProblemaRelacionado, @ProblemaValidado, @PrioridadDeRuta, @OrdenPrioridad,
                  @FechaARealizar, @Cuadrilla, @ObraRelacionada, @ClaseDeOrden, @IdEstado);
                 SELECT CAST(scope_identity() AS int);";

                // Configurar los parámetros de la consulta
                com.CommandText = query;
                com.Parameters.Clear();
                com.Parameters.AddWithValue("@TipoDeElemento", orden.TipoDeElemento);
                com.Parameters.AddWithValue("@CodigoDeElemento", orden.CodigoDeElemento);
                com.Parameters.AddWithValue("@ElementoRelacionado", orden.ElementoRelacionado);
                com.Parameters.AddWithValue("@ProblemaRelacionado", orden.ProblemaRelacionado);
                com.Parameters.AddWithValue("@ProblemaValidado", orden.ProblemaValidado);
                com.Parameters.AddWithValue("@OrdenPrioridad", orden.OrdenPrioridad);
                com.Parameters.AddWithValue("@PrioridadDeRuta", orden.PrioridadDeRuta == 0 ? (object)DBNull.Value : orden.PrioridadDeRuta);
                com.Parameters.AddWithValue("@FechaARealizar", orden.FechaARealizar);
                com.Parameters.AddWithValue("@Cuadrilla", orden.Cuadrilla);
                com.Parameters.AddWithValue("@ObraRelacionada", orden.ObraRelacionada);
                com.Parameters.AddWithValue("@ClaseDeOrden", orden.ClaseDeOrden);
                com.Parameters.AddWithValue("@IdEstado", 2);

                // Ejecutar la consulta y obtener el ID de la orden insertada
                int newOrderId = (int)com.ExecuteScalar();

                if (newOrderId > 0)
                {
                    // Limpiar los parámetros antes de la siguiente consulta
                    com.Parameters.Clear();
                    com.CommandText = "UPDATE pqrs SET Estado = @NuevoEstado WHERE Idpqrs = @idpqrs";
                    com.Parameters.AddWithValue("@NuevoEstado", 2);
                    com.Parameters.AddWithValue("@idpqrs", orden.ElementoRelacionado);
                    com.ExecuteNonQuery();

                    return Json(new { success = true, ordenId = newOrderId });
                }
                else
                {
                    return Json(new { success = false, message = "Error al crear la orden de servicio" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al crear la orden de servicio: " + ex.Message });
            }
            finally
            {
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }


        public ActionResult CrearOrdenDeServicio(int idPqrs, string descripcionAfectacion)
        {
            ViewBag.IdPqrs = idPqrs;
            ViewBag.DescripcionAfectacion = descripcionAfectacion;
            return View();
            // return RedirectToAction("ObtenerFormulario", new { opcion = "mantenimiento", idPqrs = idPqrs, descripcionAfectacion = descripcionAfectacion });
        }


        public ActionResult crearCuadrilla()
        {



            return View();
        }

        public ActionResult eliminarOrden(int id, int idpqrs)
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "DELETE FROM ordenes_de_servicio WHERE id_orden = @Idorden";
            com.Parameters.AddWithValue("@Idorden", id);

            try
            {
                int rowsAffected = com.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    com.CommandText = "UPDATE pqrs SET Estado = @NuevoEstado WHERE Idpqrs = @idpqrs";

                    // Proporcionar los parámetros necesarios para la actualización del estado
                    com.Parameters.AddWithValue("@NuevoEstado", 1); // Cambia el valor 2 según corresponda
                    com.Parameters.AddWithValue("@idpqrs", idpqrs);

                    // Ejecutar la consulta de actualización del estado
                    int rowsUpdated = com.ExecuteNonQuery();

                    // Usuario eliminado exitosamente
                    return Json(new { success = true, message = "Orden eliminada correctamente." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // No se encontró ninguna orden con el id proporcionado
                    return Json(new { success = false, message = "No se encontró ninguna Orden." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción
                return Json(new { success = false, message = "Error al intentar eliminar la orden: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult crearNuevaCuadrilla(CuadrillaModel cuadrilla)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Establecer la conexión con la base de datos
                    connectionString();
                    con.Open();
                    com.Connection = con;

                    // Consulta SQL para insertar la nueva cuadrilla
                    com.CommandText = "INSERT INTO Cuadrillas (Nombre, Municipio,Clave) VALUES (@Nombre, @Municipio,@Clave)";
                    com.Parameters.AddWithValue("@Nombre", cuadrilla.Nombre);
                    com.Parameters.AddWithValue("@Municipio", cuadrilla.Municipio);
                    com.Parameters.AddWithValue("@Clave", cuadrilla.Clave);

                    // Ejecutar la consulta
                    com.ExecuteNonQuery();

                    // Cerrar la conexión
                    con.Close();

                    // Redireccionar a la acción MostrarCuadrillas
                    ViewBag.MostrarMensajeExito = true;
                    return PartialView("_MensajeExito");
                }
                catch (Exception ex)
                {
                    // Manejar cualquier error y mostrar un mensaje de error en la vista
                    ViewBag.ErrorMessage = "Error al crear la cuadrilla: " + ex.Message;
                    return View(cuadrilla);
                }
            }
            else
            {
                // El modelo no es válido, volver a mostrar el formulario de creación con los errores de validación
                return View(cuadrilla);
            }
        }


        [HttpPost]
        public ActionResult verificarElemento(float codigo)
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "select * from infraestructura where codigo ='" + codigo + "'";
            dr = com.ExecuteReader();
            if (dr.Read())
            {


                con.Close();
                return Json(new { success = true, message = "El elemento existe." });

            }
            else
            {
                con.Close();
                return Json(new { success = false, message = "El elemento no existe." });

            }



        }

        public ActionResult ObtenerCuadrilla()
        {
            List<CuadrillaModel> cuadrillas = new List<CuadrillaModel>();

            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "SELECT * FROM Cuadrillas";

            dr = com.ExecuteReader();
            while (dr.Read())
            {
                CuadrillaModel cuadrilla = new CuadrillaModel();
                cuadrilla.Id_Cuadrilla = Convert.ToInt32(dr["id_cuadrilla"]);
                cuadrilla.Nombre = dr["Nombre"].ToString();
                cuadrilla.Municipio = dr["Municipio"].ToString();

                cuadrillas.Add(cuadrilla);
            }

            dr.Close();
            con.Close();


            return View("ObtenerFormulario", cuadrillas);
        }


        [HttpPost]
        public ActionResult BuscarOrden(string tipoBusqueda, string desde, string hasta, string IdOrden)
        {
            List<OrdenModel> ordenes = new List<OrdenModel>();

            connectionString();
            con.Open();
            com.Connection = con;

            if (tipoBusqueda == "consecutivo")
            {
                // Búsqueda por número de consecutivo
                com.CommandText = "SELECT * FROM ordenes_de_servicio WHERE id_orden = @IdOrden";
                com.Parameters.AddWithValue("@IdOrden", IdOrden);
            }
            else if (tipoBusqueda == "fecha")
            {
                // Búsqueda por rango de fechas
                com.CommandText = "SELECT * FROM ordenes_de_servicio WHERE fecha_a_realizar BETWEEN @Desde AND @Hasta";
                com.Parameters.AddWithValue("@Desde", desde);
                com.Parameters.AddWithValue("@Hasta", hasta);
            }

            SqlDataReader dr = com.ExecuteReader();

            while (dr.Read())
            {
                OrdenModel orden = new OrdenModel();
                orden.IdOrden = Convert.ToInt32(dr["id_orden"]);
                orden.TipoDeElemento = dr["Tipo_de_elemento"].ToString();
                orden.CodigoDeElemento = dr["Codigo_de_elemento"].ToString();
                orden.ElementoRelacionado = dr["elemento_relacionado"].ToString();
                orden.CodigoOrden = dr["codigo_orden"].ToString();
                orden.ProblemaRelacionado = dr["problema_relacionado"].ToString();
                orden.ProblemaValidado = dr["problema_validado"].ToString();
                orden.OrdenPrioridad = dr["Orden_prioridad"].ToString();
                orden.PrioridadDeRuta = Convert.ToInt32(dr["prioridad_de_ruta"]);
                orden.FechaARealizar = Convert.ToDateTime(dr["fecha_a_realizar"]);
                orden.Cuadrilla = dr["cuadrilla"].ToString();
                orden.TipoDeOrden = dr["tipo_de_orden"].ToString();
                orden.TipoDeSolucion = dr["tipo_de_Solucion"].ToString();
                orden.ClaseDeOrden = dr["clase_de_orden"].ToString();
                orden.ObraRelacionada = dr["obra_relacionada"].ToString();

                ordenes.Add(orden);
            }

            con.Close();

            return View("monitorear", ordenes); // Redirige al Index con los resultados de la búsqueda
        }

        public ActionResult ObtenerFormulario(string opcion, string idPqrs, string descripcionAfectacion)
        {
            ObtenerCuadrilla();


            // Dependiendo de la opción seleccionada, se carga la vista parcial correspondiente
            switch (opcion)
            {
                case "mantenimiento":
                    ViewBag.IdPqrs = idPqrs;
                    ViewBag.descripcionAfectacion = descripcionAfectacion;
                    return PartialView("_FormularioMantenimiento");
                case "expansion":
                    ViewBag.IdPqrs = idPqrs;
                    ViewBag.descripcionAfectacion = descripcionAfectacion;
                    return PartialView("_FormularioMontaje");
                case "modernizacion":
                    ViewBag.IdPqrs = idPqrs;
                    ViewBag.descripcionAfectacion = descripcionAfectacion;
                    return PartialView("_FormularioModernizacion");
                default:
                    // Si la opción no es válida, se muestra un mensaje de error o se redirige a una vista de error
                    return Content("Opción de formulario no válida");
            }
        }

        public ActionResult Graficas()
        {

            return View();
        }

    }
}