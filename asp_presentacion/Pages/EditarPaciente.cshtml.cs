using lib_entidades.Modelos;
using lib_presentaciones.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace asp_presentacion.Pages
{
    public class EditarPacienteModel : PageModel
    {
        private readonly ILogger<EditarPacienteModel> _logger;
        private readonly IPacientePresentacion _pacientePresentacion;

        [BindProperty]
        public Paciente PacienteEditable { get; set; } = new Paciente();

        [TempData]
        public string Mensaje { get; set; }

        public EditarPacienteModel(
            ILogger<EditarPacienteModel> logger,
            IPacientePresentacion pacientePresentacion)
        {
            _logger = logger;
            _pacientePresentacion = pacientePresentacion;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            try
            {
                if (id.HasValue)
                {
                    // Si viene con ID, buscar ese paciente espec�fico
                    var paciente = (await _pacientePresentacion.Buscar(
                        new Paciente { Id = id.Value }, "ID")).FirstOrDefault();

                    if (paciente != null)
                    {
                        PacienteEditable = paciente;
                    }
                }
                else
                {
                    // Si no viene con ID, obtener el �ltimo
                    var pacientes = await _pacientePresentacion.Listar();
                    if (pacientes != null && pacientes.Any())
                    {
                        PacienteEditable = pacientes.OrderByDescending(p => p.Id).First();
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                Mensaje = $"Error al cargar paciente: {ex.Message}";
                _logger.LogError(ex, "Error al cargar paciente para edici�n");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                _logger.LogInformation("Intentando modificar paciente ID: {Id}", PacienteEditable.Id);

                // Validaci�n adicional de datos
                if (string.IsNullOrEmpty(PacienteEditable.Nombre) ||
                    string.IsNullOrEmpty(PacienteEditable.Telefono))
                {
                    ModelState.AddModelError("", "Nombre y Tel�fono son obligatorios");
                    return Page();
                }

                var pacientes = await _pacientePresentacion.Buscar(new Paciente { Email = PacienteEditable.Email }, "EMAIL");

                if (pacientes.Any(p => p.Id != PacienteEditable.Id))
                {
                    ModelState.AddModelError("PacienteEditable.Email", "El correo ya est� registrado en otro paciente.");
                    return Page();
                }


                // Si pasa todas las validaciones, modificar
                var pacienteActualizado = await _pacientePresentacion.Modificar(PacienteEditable);

                _logger.LogInformation("Paciente actualizado: {@Paciente}", pacienteActualizado);

                Mensaje = "�Datos actualizados correctamente!";
                return RedirectToPage(new { id = PacienteEditable.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar paciente ID: {Id}", PacienteEditable.Id);
                Mensaje = $"Error al guardar cambios: {ex.Message}";
                return Page();
            }
        }
    }
}