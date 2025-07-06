using ProductoraCelis.Models;

namespace ProductoraCelis.ViewsModes
{
    public class ReporteCarritoVM
    {
        public List<Reporte> Reportes { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
