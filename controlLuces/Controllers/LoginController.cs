using controlLuces.Models;
using controlLuces.permisos;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using iTextSharp.text.pdf.qrcode;


namespace controlLuces.Controllers
{

    public class LoginController : Controller
    {

        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;


        void connectionString()
        {
            //con.ConnectionString = "data source =luminarias.mssql.somee.com ;initial catalog=luminarias;user id=JonathanFLF_SQLLogin_1;pwd=zgya9wozpl";
            //con.ConnectionString = "data source =LAPTOP-VHK1MAKD\\CONEXION ;DataBase=luminarias; integrated security=SSPI";
            //con.ConnectionString = "data source =DESKTOP-CREDOOS\\SQLEXPRESS ;DataBase=luminarias; integrated security=SSPI";
            //con.ConnectionString = "data source =JONATHAN-PC\\SQLEXPRESS ;DataBase=luminarias; integrated security=SSPI";
            con.ConnectionString = "data source=tadeo.colombiahosting.com.co\\MSSQLSERVER2019;initial catalog=lightcon_luminaria;user id=lightcon_lumin;pwd=luminaria2024*";
            //con.ConnectionString = "data source = luminaria.mssql.somee.com; initial catalog = luminaria; user id = hhjhhshsgsg_SQLLogin_2; pwd = cdrf7hhrrl";
        }
        // GET: Login
        public ActionResult Login()
        {

            con.Close();
            return View();
        }

        public ActionResult NuevaContra()
        {

           




            return View();


        }

        public ActionResult RestablecerContraseña(String nuevaContraseña)
        {
            string correo = Session["destinatario"] as string;
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "UPDATE usuarios SET  Clave = @Clave WHERE Correo ='" + correo + "'";

          
            com.Parameters.AddWithValue("@Clave", nuevaContraseña);
            
          

            try
            {
                int rowsAffected = com.ExecuteNonQuery();
                con.Close();

                if (rowsAffected > 0)
                {
                    // Usuario editado exitosamente
                    ViewBag.EditMessage = "contraseña Cambiada Correctamente";
                    return RedirectToAction("Login");
                }
                else
                {
                    // No se encontró ningún usuario con el IdUsuario proporcionado
                    ViewBag.EditMessage = "No se pudo cambiar";
                   
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción
                ViewBag.EditMessage = "Error al intentar editar el usuario: " + ex.Message;
                
            }

            return null;
        }
        
        public ActionResult ingresarCodigo(string destinatario)
        {






            return View();


        }
       
        public ActionResult VerificarCodigo(string codigo)
        {

            string codigoEnviado = Session["CodigoEnviado"] as string;


            if (codigo == codigoEnviado)
            {
                // El código ingresado es correcto, puedes redirigir a una página de éxito o realizar otras acciones
                return View("NuevaContra");
            }
            else
            {
                // El código ingresado es incorrecto, muestra un mensaje de error o realiza otras acciones
                ViewBag.ErrorMessage = "El código ingresado no es válido.";
                return View("ingresarCodigo");
            }

           


        }

        public ActionResult EnviarCodigo(string destinatario)
        {
            connectionString();

            
            string query = "SELECT Correo FROM usuarios WHERE Correo = @Correo";
            com.Connection = con;
            com.CommandText = query;
            com.Parameters.AddWithValue("@Correo", destinatario);

            try
            {
                con.Open();
                string correoEncontrado = (string)com.ExecuteScalar();
                if (correoEncontrado != null)
                {
                    // Correo encontrado en la base de datos, enviar el código aquí
                    EnviarCorreo(destinatario);
                    return View("ingresarCodigo");
                }
                else
                {


                    ViewBag.ErrorMessage = "El correo electrónico no está registrado";
                    return View("recuperarcontra");
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier error de conexión
                ViewBag.ErrorMessage = "Error al conectar con la base de datos: " + ex.Message;
                return View("recuperarcontra");
            }
            finally
            {
                con.Close();

                
            }
            return null;
        }

        public ActionResult EnviarCorreo(string destinatario)



        {

            
            string urlDomain = "https://localhost:44360/";
            string EmailOrigen = "ferney5585@gmail.com";
            string Password = "jjak vuuq ciyd dyze";
            string url = urlDomain;

            // Generar código de 4 dígitos aleatorio
            Random random = new Random();
            int codigo = random.Next(1000, 9999);
            Session["CodigoEnviado"] = codigo.ToString();
            Session["destinatario"] = destinatario.ToString();
            // NOMBRE MENSAJE
            string Nombre = "Parking";

            string Cuerpo = $"Hola,<br/><br/>Su código de verificación es: <strong>{codigo}</strong><br/><br/>" +
                            $"Puede ingresar el código en el siguiente enlace: <a href='{url}'>Restablecer contraseña</a>";

            string Asunto = "Reestablecer Contraseña";

            var mail = new MailMessage()
            {
                From = new MailAddress(EmailOrigen, Nombre),
                Subject = Asunto,
                Body = Cuerpo,
                BodyEncoding = System.Text.Encoding.UTF8,
                SubjectEncoding = System.Text.Encoding.Default,
                IsBodyHtml = true,
            };
            mail.To.Add(destinatario.ToLower().Trim());

            var client = new SmtpClient()
            {
                EnableSsl = true,
                Port = 587,
                Host = "smtp.gmail.com",
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(EmailOrigen, Password)
            };

            try
            {
                // Enviar correo
                client.Send(mail);
                return RedirectToAction("ingresarCodigo");
                //return Json(new { success = true, message = "Correo electrónico enviado exitosamente", codigo = codigo });
            }
            catch (Exception ex)
            {
                return View("Error");
                //return Json(new { success = false, message = "No se pudo enviar el correo electrónico: " + ex.Message });
            }
            finally
            {
                client.Dispose();
            }
        }

        public ActionResult recuperarcontra()
        {
            return View();

        }

        public ActionResult inicio(PqrsModel pqrs, OrdenModel ordenes_de_servicio)
        {
            int cantidadPqrsEstado1 = 0;
            int cantidadPqrsEstado2 = 0;
            int cantidadPqrsEstado3 = 0;
            int cantidadOrdenesEstado2 = 0;


            connectionString();
            con.Open();
            com.Connection = con;

            // Consultar la cantidad de PQRS con estado "Sin Asignar"
            com.CommandText = "SELECT COUNT(*) FROM pqrs WHERE Estado = 1";
            cantidadPqrsEstado1 = (int)com.ExecuteScalar();

            // Consultar la cantidad de PQRS con estado 2
            com.CommandText = "SELECT COUNT(*) FROM pqrs WHERE Estado = 2";
            cantidadPqrsEstado2 = (int)com.ExecuteScalar();

            // Consultar la cantidad de PQRS con estado 3
            com.CommandText = "SELECT COUNT(*) FROM pqrs WHERE Estado = 3";
            cantidadPqrsEstado3 = (int)com.ExecuteScalar();

            // Consultar la cantidad de órdenes con estado 2
            com.CommandText = "SELECT COUNT(*) FROM ordenes_de_servicio WHERE IdEstado = 2";
            cantidadOrdenesEstado2 = (int)com.ExecuteScalar();



            con.Close();

            // Pasar las cantidades de PQRS y órdenes a la vista
            ViewBag.CantidadPqrsEstado1 = cantidadPqrsEstado1;
            ViewBag.CantidadPqrsEstado2 = cantidadPqrsEstado2;
            ViewBag.CantidadPqrsEstado3 = cantidadPqrsEstado3;
            ViewBag.CantidadOrdenesEstado2 = cantidadOrdenesEstado2;


            return View();
        }

        [System.Web.Http.HttpPost]
        public ActionResult verificar(LoginModel modelo)
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "select * from usuarios where Correo ='" + modelo.Correo + "'and Clave='" + modelo.Clave + "'";

            dr = com.ExecuteReader();
            if (dr.Read())
            {
                // Crear el objeto UsuarioModel
                UsuarioModel usuario = new UsuarioModel
                {
                    IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                    Nombre = dr["Nombre"].ToString(),
                    Apellido = dr["Apellido"].ToString(),
                    Correo = dr["Correo"].ToString(),
                    IdRol = (Rol)dr["IdRol"],
                    Clave = dr["Clave"].ToString()
                };

                // Guardar el usuario en la sesión
                Session["Usuario"] = usuario;

                con.Close();
                return RedirectToAction("inicio", "Login");
            }
            else
            {
                con.Close();
                ViewBag.ErrorMessage = "Credenciales inválidas. Inténtelo de nuevo.";
                return View("Login");
            }
        }

        public ActionResult CerrarSesion()
        {
            // Elimina la sesión actual del usuario
            con.Close(); // Limpia todas las variables de sesión
            Session.Clear();
            // Redirige a la página de inicio de sesión
            return RedirectToAction("Login", "Login");
        }

        [PermisosRol(Rol.Administrador, Rol.Tecnico)]
        public ActionResult Usuarios()
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "SELECT * FROM usuarios"; // Query para seleccionar todos los usuarios
            SqlDataReader dr = com.ExecuteReader();

            // Lista para almacenar los usuarios
            List<UsuarioModel> usuarios = new List<UsuarioModel>();

            // Leer los datos y añadirlos a la lista
            while (dr.Read())
            {
                UsuarioModel usuario = new UsuarioModel();
                usuario.IdUsuario = Convert.ToInt32(dr["IdUsuario"]);
                usuario.Nombre = dr["Nombre"].ToString();
                usuario.Apellido = dr["Apellido"].ToString();
                usuario.Correo = dr["Correo"].ToString();
                usuario.IdRol = (Rol)dr["IdRol"];
                usuario.Clave = dr["Clave"].ToString();
                usuarios.Add(usuario);
            }

            con.Close();

            // Pasar la lista de usuarios a la vista
            return View(usuarios);


        }

        public ActionResult Registro()
        {
            return View();

        }

        public ActionResult EditarUsuarios(int id)
        {


            UsuarioModel usuario = ObtenerUsuarioPorId(id);


            return View(usuario);

        }

        public ActionResult Registrousuarios()
        {



            return View();

        }

        public ActionResult EditarUsuario(int id, UsuarioModel registro)
        {

            int iduser = id;
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "UPDATE usuarios SET  IdUsuario = @IdUsuario, Nombre = @Nombre, Clave = @Clave,Rol=@IdRol, Apellido = @Apellido, Correo = @Correo WHERE IdUsuario ='" + iduser + "'";

            com.Parameters.AddWithValue("@Nombre", registro.Nombre);
            com.Parameters.AddWithValue("@Clave", registro.Clave);
            com.Parameters.AddWithValue("@Apellido", registro.Apellido);
            com.Parameters.AddWithValue("@IdRol", registro.IdRol);
            com.Parameters.AddWithValue("@Correo", registro.Correo);
            com.Parameters.AddWithValue("@IdUsuario", registro.IdUsuario);

            try
            {
                int rowsAffected = com.ExecuteNonQuery();
                con.Close();

                if (rowsAffected > 0)
                {
                    // Usuario editado exitosamente
                    ViewBag.EditMessage = "Usuario editado correctamente.";
                    return RedirectToAction("Usuarios");
                }
                else
                {
                    // No se encontró ningún usuario con el IdUsuario proporcionado
                    ViewBag.EditMessage = "No se encontró ningún usuario con el IdUsuario proporcionado.";
                    return RedirectToAction("Usuarios");
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción
                ViewBag.EditMessage = "Error al intentar editar el usuario: " + ex.Message;
                return RedirectToAction("Usuarios");
            }
        }

        public ActionResult RegistrarUsuarios(UsuarioModel registro)
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "INSERT INTO usuarios (IdUsuario, Nombre, Clave, Apellido, Correo, IdRol) " +
                   "VALUES ('" + registro.IdUsuario + "','" + registro.Nombre + "','" + registro.Clave + "', '" +
                   registro.Apellido + "', '" + registro.Correo + "', 3)";

            try
            {
                int rowsAffected = com.ExecuteNonQuery();
                con.Close();

                if (rowsAffected > 0)
                {
                    TempData["RegistroExitoso"] = true;
                    return RedirectToAction("Login");
                }
                else
                {
                    // Error en el registro
                    ViewBag.ErrorMessage = "Error en el registro";
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción
                ViewBag.ErrorMessage = "Error en el registro: " + ex.Message;
                return View("Error");
            }
        }





        public ActionResult EliminarUsuario(int id)
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "DELETE FROM usuarios WHERE IdUsuario = @IdUsuario";
            com.Parameters.AddWithValue("@IdUsuario", id);

            try
            {
                int rowsAffected = com.ExecuteNonQuery();
                con.Close();

                if (rowsAffected > 0)
                {
                    // Usuario eliminado exitosamente
                    ViewBag.DeleteMessage = "Usuario eliminado correctamente.";
                    return RedirectToAction("Usuarios");
                }
                else
                {
                    // No se encontró ningún usuario con el IdUsuario proporcionado
                    ViewBag.DeleteMessage = "No se encontró ningún usuario con el IdUsuario proporcionado.";
                    return RedirectToAction("Usuarios");
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción
                ViewBag.DeleteMessage = "Error al intentar eliminar el usuario: " + ex.Message;
                return RedirectToAction("Usuarios");
            }
        }

        public UsuarioModel ObtenerUsuarioPorId(int id)
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "SELECT * FROM usuarios WHERE IdUsuario = @IdUsuario";
            com.Parameters.AddWithValue("@IdUsuario", id);

            UsuarioModel usuario = null;
            try
            {
                dr = com.ExecuteReader();
                if (dr.Read())
                {
                    usuario = new UsuarioModel
                    {
                        IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                        Nombre = dr["Nombre"].ToString(),
                        Apellido = dr["Apellido"].ToString(),
                        Correo = dr["Correo"].ToString(),
                        IdRol = (Rol)dr["Rol"],
                        Clave = dr["Clave"].ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción
                ViewBag.ErrorMessage = "Error en el registro: " + ex.Message;

            }
            finally
            {
                con.Close();
            }


            return usuario;
        }

        [System.Web.Http.HttpPost]
        public ActionResult Registrar(UsuarioModel regis)
        {
            connectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "INSERT INTO usuarios (IdUsuario, Nombre, Clave, Apellido, Correo, IdRol) VALUES (@IdUsuario, @Nombre, @Clave, @Apellido, @Correo, @IdRol)";
            com.Parameters.AddWithValue("@IdUsuario", regis.IdUsuario);
            com.Parameters.AddWithValue("@Nombre", regis.Nombre);
            com.Parameters.AddWithValue("@Clave", regis.Clave);
            com.Parameters.AddWithValue("@Apellido", regis.Apellido);
            com.Parameters.AddWithValue("@Correo", regis.Correo);
            com.Parameters.AddWithValue("@IdRol", regis.IdRol);

            try
            {
                int rowsAffected = com.ExecuteNonQuery();
                con.Close();

                if (rowsAffected > 0)
                {
                    // Registro exitoso
                    return RedirectToAction("inicio", "Login");
                }
                else
                {
                    // Error en el registro
                    ViewBag.ErrorMessage = "Error en el registro";
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción
                ViewBag.ErrorMessage = "Error en el registro: " + ex.Message;
                return View("Error");
            }
        }


    }
}
