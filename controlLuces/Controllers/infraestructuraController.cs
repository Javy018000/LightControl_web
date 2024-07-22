using controlLuces.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace controlLuces.Controllers
{
    public class infraestructuraController : Controller
    {
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;

        void connectionString()
        {
            //con.ConnectionString = "data source = luminaria.mssql.somee.com; initial catalog = luminaria; user id = hhjhhshsgsg_SQLLogin_2; pwd = cdrf7hhrrl";
            con.ConnectionString = "data source=tadeo.colombiahosting.com.co\\MSSQLSERVER2019;initial catalog=lightcon_luminaria;user id=lightcon_lumin;pwd=luminaria2024*";
        }

        public async Task<ActionResult> BuscarInfraestructura(string tipoBusqueda, string codigo, string barrio)
        {
            connectionString();
            await con.OpenAsync();
            com.Connection = con;

            List<infraestructuraModel> resultados = new List<infraestructuraModel>();

            try
            {
                switch (tipoBusqueda)
                {
                    case "codigo":
                        com.CommandText = "SELECT * FROM infraestructura WHERE codigo = @codigo";
                        com.Parameters.AddWithValue("@codigo", codigo);
                        break;
                    case "barrio":
                        com.CommandText = "SELECT * FROM infraestructura WHERE barrio = @barrio";
                        com.Parameters.AddWithValue("@barrio", barrio);
                        break;
                    default:
                        break;
                }

                SqlDataReader dr = await com.ExecuteReaderAsync();

                while (await dr.ReadAsync())
                {
                    infraestructuraModel infraestructura = new infraestructuraModel
                    {
                        codigo = dr["codigo"].ToString(),
                        latitud = Convert.ToSingle(dr["latitud"]),
                        longitud = Convert.ToSingle(dr["longitud"]),
                        direccion = dr["direccion"].ToString(),
                        configuracion = dr["configuracion"].ToString(),
                        fabricante = dr["fabricante"].ToString(),
                        linea = dr["linea"].ToString(),
                        barrio = dr["barrio"].ToString(),
                        potencia = dr["potencia"].ToString(),
                        tipo = dr["tipo"].ToString()
                    };
                    resultados.Add(infraestructura);
                }

                con.Close();
            }
            catch (Exception ex)
            {
                // Manejar la excepción
            }

            return View("infraestructura", resultados);
        }

        public ActionResult verinfraestructura()
        {
            return View();
        }

        public ActionResult infraestructura()
        {
            return View();
        }

        public async Task<ActionResult> VerInfo(string id)
        {
            infraestructuraModel infra = new infraestructuraModel();

            connectionString();
            await con.OpenAsync();
            com.Connection = con;

            com.CommandText = "SELECT * from infraestructura WHERE codigo = @id";
            com.Parameters.AddWithValue("@id", id);

            try
            {
                SqlDataReader dr = await com.ExecuteReaderAsync();

                if (await dr.ReadAsync())
                {
                    infra.codigo = dr["codigo"].ToString();
                    infra.latitud = Convert.ToSingle(dr["latitud"]);
                    infra.longitud = Convert.ToSingle(dr["longitud"]);
                    infra.barrio = dr["barrio"].ToString();
                    infra.direccion = dr["direccion"].ToString();
                    infra.configuracion = dr["configuracion"].ToString();
                    infra.linea = dr["linea"].ToString();
                    infra.potencia = dr["potencia"].ToString();
                    infra.fabricante = dr["fabricante"].ToString();
                    infra.tipo = dr["tipo"].ToString();
                }

                con.Close();
            }
            catch (Exception ex)
            {
                // Manejar la excepción
            }

            return View(infra);
        }

        public async Task<ActionResult> vertodainfraestructura()
        {
            connectionString();
            await con.OpenAsync();
            com.Connection = con;
            com.CommandText = "SELECT * FROM infraestructura";
            SqlDataReader dr = await com.ExecuteReaderAsync();

            List<infraestructuraModel> infraestructuraList = new List<infraestructuraModel>();

            while (await dr.ReadAsync())
            {
                infraestructuraModel infraestructura = new infraestructuraModel
                {
                    codigo = dr["codigo"] != DBNull.Value ? dr["codigo"].ToString() : string.Empty,
                    latitud = dr["latitud"] != DBNull.Value ? Convert.ToSingle(dr["latitud"]) : 0,
                    longitud = dr["longitud"] != DBNull.Value ? Convert.ToSingle(dr["longitud"]) : 0,
                    direccion = dr["direccion"] != DBNull.Value ? dr["direccion"].ToString() : string.Empty,
                    configuracion = dr["configuracion"] != DBNull.Value ? dr["configuracion"].ToString() : string.Empty,
                    fabricante = dr["fabricante"] != DBNull.Value ? dr["fabricante"].ToString() : string.Empty,
                    linea = dr["linea"] != DBNull.Value ? dr["linea"].ToString() : string.Empty,
                    barrio = dr["barrio"] != DBNull.Value ? dr["barrio"].ToString() : string.Empty,
                    potencia = dr["potencia"] != DBNull.Value ? dr["potencia"].ToString() : string.Empty,
                    tipo = dr["tipo"] != DBNull.Value ? dr["tipo"].ToString() : string.Empty
                };
                infraestructuraList.Add(infraestructura);
            }

            con.Close();

            var infraestructuraListOrdenada = infraestructuraList.OrderByDescending(i => i.codigo).ToList();

            return View(infraestructuraListOrdenada);
        }

        public async Task<ActionResult> VerInfoDetallada(string id)
        {
            connectionString();
            await con.OpenAsync();
            com.Connection = con;
            com.CommandText = "SELECT * FROM infraestructura WHERE codigo = @codigo";
            com.Parameters.AddWithValue("@codigo", id);
            SqlDataReader dr = await com.ExecuteReaderAsync();

            infraestructuraModel infraestructura = null;

            if (await dr.ReadAsync())
            {
                infraestructura = new infraestructuraModel
                {
                    codigo = dr["codigo"] != DBNull.Value ? dr["codigo"].ToString() : string.Empty,
                    latitud = dr["latitud"] != DBNull.Value ? Convert.ToSingle(dr["latitud"]) : 0,
                    longitud = dr["longitud"] != DBNull.Value ? Convert.ToSingle(dr["longitud"]) : 0,
                    direccion = dr["direccion"] != DBNull.Value ? dr["direccion"].ToString() : string.Empty,
                    configuracion = dr["configuracion"] != DBNull.Value ? dr["configuracion"].ToString() : string.Empty,
                    fabricante = dr["fabricante"] != DBNull.Value ? dr["fabricante"].ToString() : string.Empty,
                    linea = dr["linea"] != DBNull.Value ? dr["linea"].ToString() : string.Empty,
                    barrio = dr["barrio"] != DBNull.Value ? dr["barrio"].ToString() : string.Empty,
                    potencia = dr["potencia"] != DBNull.Value ? dr["potencia"].ToString() : string.Empty,
                    tipo = dr["tipo"] != DBNull.Value ? dr["tipo"].ToString() : string.Empty
                };
            }

            con.Close();

            return View("verinfodetallada", infraestructura);
        }
    }
}
