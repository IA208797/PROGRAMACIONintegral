using F01_BazArt.Data;
using F01_BazArt.Models;
using F01_BazArt.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace F01_BazArt.Controllers
{
    public class PlushieController : Controller
    {
        private readonly PlushieContext _context;
        private readonly IWebHostEnvironment _Environment;

        public PlushieController(PlushieContext context, IWebHostEnvironment env)
        {
            _context = context;
            _Environment = env;
        }


        public async Task<IActionResult> Index(string ordenar, string oStock, string bNombrePlushie, string bTipoPlushie, bool bStockPlushie)
        {
            var elemento = from lista in _context.Plushies
                           select lista;
            IQueryable<string> tipos = from lista in _context.Plushies
                                       orderby lista.Type
                                       select lista.Type;

            ViewData["NameSort"] = string.IsNullOrEmpty(ordenar) ||
                ordenar == "name" ? "name_desc" : "name";

            ViewData["oStock"] = string.IsNullOrEmpty(oStock) ||
                oStock == "stock" ? "stock_desc" : "stock";

            switch (ordenar)
            {
                case "name":
                    elemento = elemento.OrderBy(s => s.Name);
                    break;
                case "name_desc":
                    elemento = elemento.OrderByDescending(s => s.Name);
                    break;
                default:
                    break;
            }

            switch (oStock)
            {
                case "stock":
                    elemento = elemento.OrderBy(s => s.Stock);
                    break;
                case "stock_desc":
                    elemento = elemento.OrderByDescending(s => s.Stock);
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(bNombrePlushie))
            {
                elemento = elemento.Where(s => s.Name.Contains(bNombrePlushie));
            }
            if (!string.IsNullOrEmpty(bTipoPlushie))
            {
                elemento = elemento.Where(x => x.Type == bTipoPlushie);
            }
            if (bStockPlushie)
            {
                elemento = elemento.Where(x => x.Stock > 0);
            }
            var buscaVM = new BuscaVM
            {
                LosTipos = new SelectList(await tipos.Distinct().ToListAsync()),
                LosPlushies = await elemento.ToListAsync()
            };
            return View(buscaVM);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlushieVM plushieVM)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(plushieVM);
                string uniqueFileNameT = UploadedFileT(plushieVM);
                Plushie plushie = new Plushie
                {
                    Name = plushieVM.Name,
                    Description = plushieVM.Description,
                    Type = plushieVM.Type,
                    Price = plushieVM.Price,
                    Stock = plushieVM.Stock,
                    ImageName = uniqueFileName,
                    ImageThumbnail = uniqueFileNameT
                };
                _context.Add(plushie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(plushieVM);
        }

        private string UploadedFileT(PlushieVM plushieVM)
        {
            string uniqueFileNameT = null;
            try
            {
                if (plushieVM.ImagePlushieThumbnail != null)
                {
                    string uploadsFolder = Path.Combine(_Environment.WebRootPath, "images");
                    uniqueFileNameT = Guid.NewGuid().ToString() + "_" + plushieVM.ImagePlushieThumbnail.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileNameT);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        plushieVM.ImagePlushieThumbnail.CopyTo(fileStream);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocurrió un error al cargar el archivo: " + ex.Message);
            }
            return uniqueFileNameT;
        }

        private string UploadedFile(PlushieVM plushieVM)
        {
            string uniqueFileName = null;
            try
            {
                if (plushieVM.ImagePlushie != null)
                {
                    string uploadsFolder = Path.Combine(_Environment.WebRootPath, "images");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + plushieVM.ImagePlushie.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        plushieVM.ImagePlushie.CopyTo(fileStream);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocurrió un error al cargar el archivo: " + ex.Message);
            }
            return uniqueFileName;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plushie = await _context.Plushies.FindAsync(id);
            if (plushie == null)
            {
                return NotFound();
            }

            var plushieVM = new PlushieVM
            {
                Name = plushie.Name,
                Description = plushie.Description,
                Type = plushie.Type,
                Price = plushie.Price,
                Stock = plushie.Stock,
                ImageNameActual = plushie.ImageName,
                ImageThumbnailActual = plushie.ImageThumbnail
            };

            ViewBag.PlushieId = plushie.Id;

            return View(plushieVM);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var plushie = await _context.Plushies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plushie == null)
            {
                return NotFound();
            }
            return View(plushie);
        }

        //[Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PlushieVM model)
        {
            if (id == 0)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var plushie = await _context.Plushies.FindAsync(id);
                    if (plushie == null)
                    {
                        return NotFound();
                    }
                    //si se sube una nueva imagen
                    if (model.ImagePlushie != null)
                    {
                        //si hay una imagen previa, se elimina
                        if (plushie.ImageName != null)
                        {
                            string filePath = Path.Combine(_Environment.WebRootPath, "images", plushie.ImageName);
                            System.IO.File.Delete(filePath);
                        }
                        string uniqueFileName = UploadedFile(model);
                        plushie.ImageName = uniqueFileName;
                    }
                    if (model.ImagePlushieThumbnail != null)
                    {
                        if (plushie.ImageThumbnail != null)
                        {
                            string filePath = Path.Combine(_Environment.WebRootPath, "images", plushie.ImageThumbnail);
                            System.IO.File.Delete(filePath);
                        }
                        string uniqueFileNameT = UploadedFileT(model);
                        plushie.ImageThumbnail = uniqueFileNameT;
                    }

                    plushie.Name = model.Name;
                    plushie.Description = model.Description;
                    plushie.Type = model.Type;
                    plushie.Price = model.Price;
                    plushie.Stock = model.Stock;


                    _context.Update(plushie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlushieExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        private bool PlushieExists(object id)
        {
            return _context.Plushies.Any(e => e.Id == (int)id);
        }

        //[Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var plushie = await _context.Plushies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plushie == null)
            {
                return NotFound();
            }
            return View(plushie);
        }

        //[Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plushie = await _context.Plushies.FindAsync(id);
            _context.Plushies.Remove(plushie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
