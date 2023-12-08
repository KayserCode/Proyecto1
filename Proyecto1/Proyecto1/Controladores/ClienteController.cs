using Proyecto1;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Proyecto1.Form1;

namespace Practico1.Controladores
{
    public class ClienteController
    {
      
        private List<Cliente> listaClientes = new List<Cliente>();

        public bool AgregarCliente(string rut, string nombre, bool esEmpresa, string telefono, string direccion, DateTime fechaRegistro, int cantidadFacturas, int numeroFactura, int montoFactura)
        {
            if (ClienteExistente(rut))
            {
                return false;
            }

            Cliente nuevoCliente = new Cliente
            {
                Rut = rut,
                Nombre = nombre,
                EsEmpresa = esEmpresa,
                Telefono = telefono,
                Direccion = direccion,
                FechaRegistro = fechaRegistro,
                CantidadFacturas = cantidadFacturas,
                NumeroUltimaFactura = numeroFactura,
                MontoUltimaFactura = montoFactura
            };

            listaClientes.Add(nuevoCliente);
            return true;
        }

        public bool EditarCliente(string rut, string nombre, bool esEmpresa, string telefono, string direccion, DateTime fechaRegistro, int cantidadFacturas, int numeroFactura, int montoFactura, int filaSeleccionada)
        {
            if (filaSeleccionada >= 0 && filaSeleccionada < listaClientes.Count)
            {
                // Validar que el nuevo RUT no pertenezca a otro cliente existente
                if (ClienteExistente(rut) && listaClientes[filaSeleccionada].Rut != rut)
                {
                    return false; // Cliente con el mismo RUT ya existe
                }

                Cliente clienteExistente = listaClientes[filaSeleccionada];
                clienteExistente.Rut = rut;
                clienteExistente.Nombre = nombre;
                clienteExistente.EsEmpresa = esEmpresa;
                clienteExistente.Telefono = telefono;
                clienteExistente.Direccion = direccion;
                clienteExistente.FechaRegistro = fechaRegistro;
                clienteExistente.CantidadFacturas = cantidadFacturas;
                clienteExistente.NumeroUltimaFactura = numeroFactura;
                clienteExistente.MontoUltimaFactura = montoFactura;

                return true; // Cliente editado exitosamente
            }

            return false; // Índice de fila no válido
        }



        public List<Cliente> ObtenerListaClientes()
        {
            return listaClientes;
        }

        public void EliminarCliente(int filaSeleccionada)
        {
            if (filaSeleccionada >= 0 && filaSeleccionada < listaClientes.Count)
            {
                listaClientes.RemoveAt(filaSeleccionada);
            }
        }

        private bool ClienteExistente(string rut)
        {
            return listaClientes.Exists(c => c.Rut == rut);
        }


    }
}
