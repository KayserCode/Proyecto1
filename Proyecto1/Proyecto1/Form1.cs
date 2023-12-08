using Practico1.Controladores;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace Proyecto1
{


    public partial class Form1 : Form
    {
        private DataTable table;
        private ClienteController clienteController = new ClienteController();
        private int filaSeleccionada = -1;

        public Form1()
        {
            InitializeComponent();
            try
            {
                IniciarTablaCliente();
                MostrarListaCliente();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se produjo un error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private async void ObtenerIndicadoresEconomicos()
        {
            try
            {
                // Obtener valores de indicadores económicos
                string valorUf = await clienteController.ObtenerValorUf();
                string valorDolar = await clienteController.ObtenerValorDolar();

                // Puedes hacer algo con estos valores, como mostrarlos en etiquetas, etc.
                UF.Text = $"Valor UF: {valorUf}";
                Dolar.Text = $"Valor Dólar: {valorDolar}";
            }
            catch (Exception ex)
            {
                // Manejar errores, por ejemplo, mostrar un mensaje de error
                MessageBox.Show($"Error al obtener indicadores económicos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ...

        // Llama a ObtenerIndicadoresEconomicos en algún evento, como al cargar el formulario
        private void Form1_Load(object sender, EventArgs e)
        {
            ObtenerIndicadoresEconomicos();
        }

        private void IniciarTablaCliente()
        {
            table = new DataTable();
            table.Columns.Add("Rut", typeof(string));
            table.Columns.Add("Nombre", typeof(string));
            table.Columns.Add("Es Empresa", typeof(bool));
            table.Columns.Add("Telefono", typeof(string));
            table.Columns.Add("Direccion", typeof(string));
            table.Columns.Add("Fecha Registro", typeof(DateTime));
            table.Columns.Add("Cantidad Facturas", typeof(int));
            table.Columns.Add("Numero Ultima Factura", typeof(int));
            table.Columns.Add("Monto Ultima Factura", typeof(int));

            Lista_Cliente.DataSource = table;
        }

        private void MostrarListaCliente()
        {
            IniciarTablaCliente();

            List<Cliente> clientes = clienteController.ObtenerListaClientes();

            if (clientes != null)
            {
                foreach (var cliente in clientes)
                {
                    DataRow row = table.NewRow();
                    row["Rut"] = cliente.Rut;
                    row["Nombre"] = cliente.Nombre;
                    row["Es Empresa"] = cliente.EsEmpresa;
                    row["Telefono"] = cliente.Telefono;
                    row["Direccion"] = cliente.Direccion;
                    row["Fecha Registro"] = cliente.FechaRegistro.ToString("dd/MM/yyyy");
                    row["Cantidad Facturas"] = cliente.CantidadFacturas;
                    row["Numero Ultima Factura"] = cliente.NumeroUltimaFactura;
                    row["Monto Ultima Factura"] = cliente.MontoUltimaFactura;

                    table.Rows.Add(row);
                }
            }
        }

        private HashSet<string> rutsExistentes = new HashSet<string>();

        private void AgregarCliente_Click(object sender, EventArgs e)
        {
            try
            {
                string rut = Rut.Text.Trim();
                string nombre = Nombre.Text.Trim();
                bool esEmpresa = Si.Checked;
                string telefono = Telefono.Text.Trim();
                string direccion = Direccion.Text.Trim();
                DateTime fechaRegistro = Fecha_Registro.Value;

                // Validar que solo se ingresen letras en el campo de Nombre
                if (!Regex.IsMatch(nombre, "^[a-zA-Z ]+$"))
                {
                    MessageBox.Show("Ingrese solo letras en el campo de Nombre", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int cantidadFacturas, numeroFactura, montoFactura;

                // Validar que solo se ingresen números en los campos correspondientes
                if (!int.TryParse(CantidadFacturas.Text, out cantidadFacturas) || cantidadFacturas < 0)
                {
                    MessageBox.Show("Ingrese un valor numérico válido y mayor o igual a cero en el campo de Cantidad de Facturas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!int.TryParse(NumeroFactura.Text, out numeroFactura) || numeroFactura < 0)
                {
                    MessageBox.Show("Ingrese un valor numérico válido y mayor o igual a cero en el campo de Número Última Factura", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!int.TryParse(MontoFactura.Text, out montoFactura) || montoFactura < 0)
                {
                    MessageBox.Show("Ingrese un valor numérico válido y mayor o igual a cero en el campo de Monto Última Factura", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validar el formato del campo Rut
                if (!Regex.IsMatch(rut, @"^[0-9-]+$"))
                {
                    MessageBox.Show("Ingrese un valor válido en el campo de Rut (solo números y guiones)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Validar que el Rut no esté repetido
                if (rutsExistentes.Contains(rut))
                {
                    MessageBox.Show("Ya existe un cliente con el mismo Rut", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Agregar el Rut al conjunto de Ruts existentes
                rutsExistentes.Add(rut);

                if (clienteController.AgregarCliente(rut, nombre, esEmpresa, telefono, direccion, fechaRegistro, cantidadFacturas, numeroFactura, montoFactura))
                {
                    MessageBox.Show("Cliente agregado exitosamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Limpiar las cajas de texto después de agregar el cliente
                    LimpiarCamposCliente();
                    MostrarListaCliente();
                }
                else
                {
                    MessageBox.Show("Ocurrió un error al agregar el cliente", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se produjo un error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditarCliente_Click(object sender, EventArgs e)
        {
            try
            {
                if (filaSeleccionada >= 0 && filaSeleccionada < table.Rows.Count)
                {
                    string rut = Rut.Text.Trim();
                    string nombre = Nombre.Text.Trim();
                    bool esEmpresa = Si.Checked;
                    string telefono = Telefono.Text.Trim();
                    string direccion = Direccion.Text.Trim();
                    DateTime fechaRegistro = Fecha_Registro.Value;
                    int cantidadFacturas, numeroFactura, montoFactura;

                    // Validar que solo se ingresen letras en el campo de Nombre
                    if (!Regex.IsMatch(nombre, "^[a-zA-Z ]+$"))
                    {
                        MessageBox.Show("Ingrese solo letras en el campo de Nombre", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Validar que solo se ingresen números en los campos correspondientes
                    if (!int.TryParse(CantidadFacturas.Text, out cantidadFacturas) || cantidadFacturas < 0)
                    {
                        MessageBox.Show("Ingrese un valor numérico válido y mayor o igual a cero en el campo de Cantidad de Facturas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!int.TryParse(NumeroFactura.Text, out numeroFactura) || numeroFactura < 0)
                    {
                        MessageBox.Show("Ingrese un valor numérico válido y mayor o igual a cero en el campo de Número Última Factura", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!int.TryParse(MontoFactura.Text, out montoFactura) || montoFactura < 0)
                    {
                        MessageBox.Show("Ingrese un valor numérico válido y mayor o igual a cero en el campo de Monto Última Factura", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Obtener los valores originales de la fila seleccionada
                    string rutOriginal = table.Rows[filaSeleccionada]["Rut"].ToString();
                    string nombreOriginal = table.Rows[filaSeleccionada]["Nombre"].ToString();
                    bool esEmpresaOriginal = (bool)table.Rows[filaSeleccionada]["Es Empresa"];
                    string telefonoOriginal = table.Rows[filaSeleccionada]["Telefono"].ToString();
                    string direccionOriginal = table.Rows[filaSeleccionada]["Direccion"].ToString();
                    DateTime fechaRegistroOriginal = DateTime.Parse(table.Rows[filaSeleccionada]["Fecha Registro"].ToString());
                    int cantidadFacturasOriginal = int.Parse(table.Rows[filaSeleccionada]["Cantidad Facturas"].ToString());
                    int numeroFacturaOriginal = int.Parse(table.Rows[filaSeleccionada]["Numero Ultima Factura"].ToString());
                    int montoFacturaOriginal = int.Parse(table.Rows[filaSeleccionada]["Monto Ultima Factura"].ToString());

                    // Verificar si ha habido cambios
                    bool cambiosRealizados = rut != rutOriginal ||
                                             nombre != nombreOriginal ||
                                             esEmpresa != esEmpresaOriginal ||
                                             telefono != telefonoOriginal ||
                                             direccion != direccionOriginal ||
                                             fechaRegistro != fechaRegistroOriginal ||
                                             cantidadFacturas != cantidadFacturasOriginal ||
                                             numeroFactura != numeroFacturaOriginal ||
                                             montoFactura != montoFacturaOriginal;

                    if (cambiosRealizados)
                    {
                        clienteController.EditarCliente(rut, nombre, esEmpresa, telefono, direccion, fechaRegistro, cantidadFacturas, numeroFactura, montoFactura, filaSeleccionada);
                        MessageBox.Show("Cliente modificado exitosamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MostrarListaCliente();
                        LimpiarCamposCliente();
                    }
                    else
                    {
                        MessageBox.Show("No se realizaron cambios", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Seleccione un cliente antes de editar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se produjo un error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void Eliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (filaSeleccionada >= 0 && filaSeleccionada < table.Rows.Count)
                {
                    // Obtener el RUT del cliente antes de eliminarlo
                    string rutClienteAEliminar = table.Rows[filaSeleccionada]["Rut"].ToString();

                    // Eliminar el cliente
                    clienteController.EliminarCliente(filaSeleccionada);

                    // Eliminar el RUT del conjunto de Ruts existentes
                    rutsExistentes.Remove(rutClienteAEliminar);

                    MessageBox.Show("Cliente eliminado exitosamente", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MostrarListaCliente();
                    LimpiarCamposCliente();
                }
                else
                {
                    MessageBox.Show("Seleccione un cliente antes de eliminar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Se produjo un error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Lista_Cliente_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            filaSeleccionada = e.RowIndex;
            if (filaSeleccionada >= 0 && filaSeleccionada < table.Rows.Count)
            {
                Rut.Text = table.Rows[filaSeleccionada]["Rut"].ToString();
                Nombre.Text = table.Rows[filaSeleccionada]["Nombre"].ToString();
                Si.Checked = (bool)table.Rows[filaSeleccionada]["Es Empresa"];
                Telefono.Text = table.Rows[filaSeleccionada]["Telefono"].ToString();
                Direccion.Text = table.Rows[filaSeleccionada]["Direccion"].ToString();
                Fecha_Registro.Value = DateTime.Parse(table.Rows[filaSeleccionada]["Fecha Registro"].ToString());
                CantidadFacturas.Text = table.Rows[filaSeleccionada]["Cantidad Facturas"].ToString();
                NumeroFactura.Text = table.Rows[filaSeleccionada]["Numero Ultima Factura"].ToString();
                MontoFactura.Text = table.Rows[filaSeleccionada]["Monto Ultima Factura"].ToString();
            }
        }

        private void LimpiarCamposCliente()
        {
            Rut.Text = "";
            Nombre.Text = "";
            Si.Checked = false;
            Telefono.Text = "";
            Direccion.Text = "";
            Fecha_Registro.Value = DateTime.Now;
            CantidadFacturas.Text = "";
            NumeroFactura.Text = "";
            MontoFactura.Text = "";

        }       
    }
}