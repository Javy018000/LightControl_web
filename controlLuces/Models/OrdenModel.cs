using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace controlLuces.Models
{
    public class OrdenModel
    {
        // GET: OrdenModel
        public int IdOrden { get; set; }
        public string TipoDeElemento { get; set; }
        public string CodigoDeElemento { get; set; }
        public string ElementoRelacionado { get; set; }
        public string CodigoOrden { get; set; }
        public string ProblemaRelacionado { get; set; }
        public string ProblemaValidado { get; set; }
        public string OrdenPrioridad { get; set; }
        public int PrioridadDeRuta { get; set; }
        public DateTime FechaARealizar { get; set; }
        public string Cuadrilla { get; set; }
        public string TipoDeOrden { get; set; }
        public string TipoDeSolucion { get; set; }

        public string  ClaseDeOrden{ get; set; }
        public string ObraRelacionada { get; set; }

        public int IdEstado { get; set; }
        public string EstadoNombre { get; set; }
        public string Descripcion { get; set; }
        //public byte[] Recursos { get; set; } 
                                             
        public int Idpqrs { get; set; }

        public string observaciones { get; set; }

        public string Trabajos { get; set; }

    }
    

}