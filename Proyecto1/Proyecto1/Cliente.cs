using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto1
{
    public class Cliente
    {
        public string Rut { get; set; }
        public string Nombre { get; set; }
        public bool EsEmpresa { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int CantidadFacturas { get; set; }
        public int NumeroUltimaFactura { get; set; }
        public int MontoUltimaFactura { get; set; }
    }

}
