using lib_entidades.Modelos;
using lib_presentaciones.Interfaces;
using lib_utilidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp_presentacion.Pages
{
    public class PacienteModel : PageModel
    {
        private readonly IPacientePresentacion _iPresentacion;

        [BindProperty]
        public Paciente Filtro { get; set; }

        [BindProperty]
        public string SearchType { get; set; } = "Cedula";

        [BindProperty]
        public string SearchValue { get; set; }

        public List<Paciente> Lista { get; set; }
        public Paciente Actual { get; set; }
        public Enumerables.Ventanas Accion { get; set; }

        public PacienteModel(IPacientePresentacion iPresentacion)
        {
            _iPresentacion = iPresentacion;
            Filtro = new Paciente();
            Accion = Enumerables.Ventanas.Listas;
        }

        public async Task<IActionResult> OnPostBuscarAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(SearchValue))
                {
                    ModelState.AddModelError(string.Empty, "Debe ingresar un valor para realizar la búsqueda.");
                    return Page();
                }

                // Preparar el objeto Filtro según el tipo de búsqueda
                Filtro = new Paciente();
                string tipoBusqueda = "COMPLEJA"; // Búsqueda por defecto

                // Asignar el valor de búsqueda y determinar el tipo de búsqueda
                switch (SearchType)
                {
                    case "Cedula":
                        Filtro.Cedula = SearchValue;
                        tipoBusqueda = "CEDULA";
                        break;
                    case "Nombre":
                        Filtro.Nombre = SearchValue;
                        tipoBusqueda = "NOMBRE";
                        break;
                    case "Email":
                        Filtro.Email = SearchValue;
                        tipoBusqueda = "EMAIL";
                        break;
                    case "Telefono":
                        Filtro.Telefono = SearchValue;
                        tipoBusqueda = "TELEFONO";
                        break;
                    case "HistoriaClinica":
                        Filtro.HistoriaClinica = new HistoriaClinica
                        {
                            Diagnosticos = new List<Diagnostico>
                            {
                                new Diagnostico { Descripcion = SearchValue }
                            }
                        };
                        tipoBusqueda = "HISTORIA_CLINICA";
                        break;
                    case "MedicoTratante":
                        Filtro.Citas = new List<Cita>
                        {
                            new Cita
                            {
                                Medico = new Medico { Nombre = SearchValue }
                            }
                        };
                        tipoBusqueda = "MEDICO_TRATANTE";
                        break;
                    default:
                        // Búsqueda compleja por defecto
                        Filtro.Nombre = SearchValue;
                        Filtro.Cedula = SearchValue;
                        Filtro.Email = SearchValue;
                        Filtro.Telefono = SearchValue;
                        break;
                }

                Accion = Enumerables.Ventanas.Listas;
                Lista = await _iPresentacion.Buscar(Filtro, tipoBusqueda);

                if (Lista == null || Lista.Count == 0)
                {
                    ModelState.AddModelError(string.Empty, "No se encontraron pacientes con los criterios especificados.");
                }
                else
                {
                    Actual = Lista.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                LogConversor.Log(ex, ViewData!);
                ModelState.AddModelError(string.Empty, $"Ocurrió un error al buscar el paciente: {ex.Message}");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostBtRefrescarAsync()
        {
            try
            {
                Filtro = new Paciente();
                Accion = Enumerables.Ventanas.Listas;
                Actual = null;
                Lista = await _iPresentacion.Buscar(Filtro, "COMPLEJA");

                if (Lista == null || Lista.Count == 0)
                {
                    ModelState.AddModelError(string.Empty, "No se encontraron pacientes en el sistema.");
                }
            }
            catch (Exception ex)
            {
                LogConversor.Log(ex, ViewData!);
                ModelState.AddModelError(string.Empty, $"Ocurrió un error al refrescar la lista: {ex.Message}");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostEliminarAsync(string pacienteId)
        {
            try
            {
                if (!string.IsNullOrEmpty(pacienteId))
                {
                    // Aquí deberías llamar al método de eliminación de tu servicio
                    // await _iPresentacion.Eliminar(pacienteId);

                    // Refrescar la lista después de eliminar
                    return await OnPostBtRefrescarAsync();
                }
            }
            catch (Exception ex)
            {
                LogConversor.Log(ex, ViewData!);
                ModelState.AddModelError(string.Empty, $"Error al eliminar paciente: {ex.Message}");
            }
            return Page();
        }

        public IActionResult OnPostEditar(string pacienteId)
        {
            try
            {
                if (Lista != null && !string.IsNullOrEmpty(pacienteId))
                {
                    Actual = Lista.FirstOrDefault(p => p.Cedula == pacienteId);
                    Accion = Enumerables.Ventanas.Editar;
                }
            }
            catch (Exception ex)
            {
                LogConversor.Log(ex, ViewData!);
                ModelState.AddModelError(string.Empty, $"Error al preparar edición: {ex.Message}");
            }
            return Page();
        }

        public IActionResult OnPostBtCancelar()
        {
            Accion = Enumerables.Ventanas.Listas;
            return Page();
        }

        public IActionResult OnPostBtCerrar()
        {
            return RedirectToPage("/Index");
        }
    }
}