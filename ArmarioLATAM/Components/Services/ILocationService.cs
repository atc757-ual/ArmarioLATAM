using System.Collections.Generic;

namespace ArmarioLATAM.Services
{
    public interface ILocationService
    {
        List<string> GetEmergencyTypes();
        List<string> GetProvinces();
        List<string> GetLimaDistricts();
    }

    public class LocationService : ILocationService
    {
        public List<string> GetEmergencyTypes()
        {
            return new()
            {
                "Pérdida o robo",
                "Rotura",
                "Cambio de talla"
            };
        }

        public List<string> GetProvinces()
        {
            return new()
            {
                "Lima Metropolitana"
            };
        }

        public List<string> GetLimaDistricts()
        {
            return new()
            {
                "Ancón", "Ate", "Barranco", "Breña", "Carabayllo", "Chaclacayo",
                "Chorrillos", "Cieneguilla", "Comas", "El Agustino", "Independencia",
                "Jesús María", "La Molina", "La Victoria", "Lima", "Lince", "Los Olivos",
                "Lurigancho", "Lurín", "Magdalena del Mar", "Miraflores", "Pachacámac",
                "Pucusana", "Pueblo Libre", "Puente Piedra", "Punta Hermosa", "Punta Negra",
                "Rímac", "San Bartolo", "San Borja", "San Isidro", "San Juan de Lurigancho",
                "San Juan de Miraflores", "San Luis", "San Martín de Porres", "San Miguel",
                "Santa Anita", "Santa María del Mar", "Santa Rosa", "Santiago de Surco",
                "Surquillo", "Villa El Salvador", "Villa María del Triunfo"
            };
        }
    }
}
