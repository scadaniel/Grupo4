using SQLite;

namespace App_v3.Models
{
    [Table("Comisiones")]
    public class Comision
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Nombre { get; set; } // Nombre o identificación de la comisión

        // Umbrales de configuración específicos para esta comisión
        public int InasistenciasBajo { get; set; }
        public int InasistenciasMedio { get; set; }
        public int TpsBajo { get; set; }
        public int TpsMedio { get; set; }
    }
}