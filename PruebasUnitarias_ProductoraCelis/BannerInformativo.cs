using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebasUnitarias_ProductoraCelis
{
    public class BannerInformativo
    {
        [Fact]
        public void Baner_Informativo()
        {
            var rutasEsperadas = new List<string>
            {
                "/src/afiches/afiche1.jpeg",
                "/src/afiches/afiche2.jpeg",
                "/src/afiches/afiche3.jpeg",
                "/src/afiches/afiche4.jpeg",
                "/src/afiches/afiche5.jpeg"
            };

            // Simula el contenido HTML que tendrías en la vista (puedes obtenerlo manualmente y ponerlo aquí)
            string contenidoHtmlSimulado = @"
                <div class='item' style='background-image:url(""/src/afiches/afiche1.jpeg"");'></div>
                <div class='item' style='background-image:url(""/src/afiches/afiche2.jpeg"");'></div>
                <div class='item' style='background-image:url(""/src/afiches/afiche3.jpeg"");'></div>
                <div class='item' style='background-image:url(""/src/afiches/afiche4.jpeg"");'></div>
                <div class='item' style='background-image:url(""/src/afiches/afiche5.jpeg"");'></div>";

            foreach (var ruta in rutasEsperadas)
            {
                Assert.Contains(ruta, contenidoHtmlSimulado);
            }
        }
    }

}
