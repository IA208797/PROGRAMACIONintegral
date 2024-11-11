using F01_BazArt.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace F01_BazArt.ViewModels
{
    public class BuscaVM
    {
        public List<Plushie> LosPlushies { get; set; }
        public SelectList LosTipos { get; set; }
        public string bNombrePlushie { get; set; }
        public string bTipoPlushie { get; set; }
        public bool bStockPlushie { get; set; }
    }
}
