using lib_entidades.Modelos;
using lib_presentaciones.Interfaces;
using lib_utilidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.Reflection.Metadata;

namespace asp_presentacion.Pages
{
    public class HistoriaClinicaModel : PageModel
    {
        private IHistoriaClinicaPresentacion? iPresentacion = null;

        public HistoriaClinicaModel(IHistoriaClinicaPresentacion iPresentacion)
        {
            try
            {
                this.iPresentacion = iPresentacion;
                Filtro = new HistoriaClinica()
                {
                    Paciente = new Paciente() { Citas = new List<Cita> { new Cita() { Medico = new Medico() } } },
                    Diagnosticos = new List<Diagnostico> { new Diagnostico() },
                    Tratamientos = new List<Tratamiento> { new Tratamiento() },
                    Formulas = new List<Formula> { new Formula() { Medicamentos = new List<Medicamento> { new Medicamento()} } }

                };
   
                }
            catch (Exception ex)
            {
                LogConversor.Log(ex, ViewData!);
            }
        }

        [BindProperty] public bool BusquedaRealizada { get; set; }
        [BindProperty] public Enumerables.Ventanas Accion { get; set; }
        [BindProperty] public HistoriaClinica? Actual { get; set; }
        [BindProperty] public HistoriaClinica? Filtro { get; set; }
        [BindProperty] public List<HistoriaClinica>? Lista { get; set; }
        [BindProperty] public bool ErrorConsulta { get; set; } = false;
        [BindProperty] public string? MensajeError { get; set; }


        public void OnPostBuscar()
        {
            BusquedaRealizada = true;

            if (!string.IsNullOrEmpty(Filtro?.Paciente?.Cedula))
            {
                try
                {
                    Accion = Enumerables.Ventanas.Listas;

                    var task = this.iPresentacion!.Buscar(Filtro!, "CEDULA_PACIENTE");
                    task.Wait();
                    Lista = task.Result;

                    Actual = Lista?.FirstOrDefault(h => h.Paciente?.Cedula == Filtro.Paciente.Cedula);

                    // Limpiar el campo después de buscar
                    Filtro.Paciente.Cedula = "";

                    // No hay error si la consulta fue exitosa
                    ErrorConsulta = false;
                    MensajeError = null;
                }
                catch (Exception ex)
                {
                    LogConversor.Log(ex, ViewData!);
                    ErrorConsulta = true;
                    MensajeError = "Ha ocurrido un error. Intentando recargar la información.";
                    Actual = null;    // Por seguridad, para que no muestre datos erróneos
                    Lista = null;
                }
            }
        }

        public void OnPostBtRefrescar()
        {
            if (Filtro!.Paciente != null)
            {

                try
                {

                    Filtro.Paciente.Cedula = Filtro.Paciente.Cedula ?? "";
                    Filtro.Paciente.Nombre = Filtro.Paciente.Nombre ?? "";
                    Filtro.Paciente.Email = Filtro.Paciente.Email ?? "";

                    Accion = Enumerables.Ventanas.Listas;
                    var task = this.iPresentacion!.Buscar(Filtro!, "COMPLEJA");
                    task.Wait();
                    Lista = task.Result;
                    Actual = null;
                }
                catch (Exception ex)
                {
                    LogConversor.Log(ex, ViewData!);
                }
            }
        }

        
        public void OnPostBtCancelar()
        {
            try
            {
                Accion = Enumerables.Ventanas.Listas;
                OnPostBtRefrescar();
            }
            catch (Exception ex)
            {
                LogConversor.Log(ex, ViewData!);
            }
        }

        public void OnPostBtCerrar()
        {
            try
            {
                if (Accion == Enumerables.Ventanas.Listas)
                    OnPostBtRefrescar();
            }
            catch (Exception ex)
            {
                LogConversor.Log(ex, ViewData!);
            }
        }

    }
}