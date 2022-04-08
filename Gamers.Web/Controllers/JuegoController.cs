﻿using Gamers.DataAccess;
using Gamers.DataAccess.Models;
using Gamers.Web.Helpers;
using Gamers.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;


#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

#else
using System.Web.Mvc;
using System.Web;
#endif

namespace Gamers.Web.Controllers
{
    public class JuegoController : Controller
    {
        public readonly EGamersContext _context;
        public JuegoController()
        {
            _context = new EGamersContext();
        }

        // GET: Juego
        public ActionResult Index()
        {
            //Include() eagerLoadin en EF por si queres googlear, lo que hace es cuando busca los juegos automaticamente te trae la relacion de categoria 
            //en categoria pasa al revez te trae toda la lista de juegos
            var lista = _context.Juego.Include(x => x.Categoria).ToList();
            var listaVm = lista.Select(x => new JuegoViewModel(x)).ToList();
            return View(listaVm);
        }


        public ActionResult Alta()
        {

            ViewBag.CategoriaId = new SelectList(CombosHelpers.GetCategorias(), "Id", "Descripcion");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Alta(JuegoViewModel juego)
        {
           
                try
                {
                    Juego juegoBD = juego.ToEntity();
                    List<ImagenesJuego> imagenes = GetImagesFromFilesRequest(juego.Image).Select(x => new ImagenesJuego()
                    {
                        Imagen = x.Imagen,
                        ImageName = x.ImageName,
                        IsActive = x.IsActive
                    }).ToList();

                    juegoBD.Imagenes = imagenes;

                    _context.Juego.Add(juegoBD);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException.InnerException != null && ex.InnerException.InnerException.Message.Contains("IX_"))
                    {
                        ModelState.AddModelError(string.Empty, "Juego existente");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                    return View(juego);
                }
                return RedirectToAction("Index");
            }
        

        public ActionResult Baja(int? id)
        {
            if (id == null)
            {
#if NETCOREAPP
                return BadRequest();
#else
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

#endif
            }
            var Juego = _context.Juego.Find(id);
            if (Juego == null)
            {
#if NETCOREAPP
                return NotFound();
#else
                return HttpNotFound();

#endif
            }
            var juegoVm = new JuegoViewModel(Juego);
            return View(juegoVm);
        }

        [HttpPost]
        public ActionResult Baja(int id)
        {
            var Juego = _context.Juego.Find(id);
            if (!ModelState.IsValid)
            {
#if NETCOREAPP
                return NotFound();
#else
                return HttpNotFound();

#endif
            }

            try
            {
                _context.Juego.Remove(Juego);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return View(Juego);
            }


            return RedirectToAction("Index");
        }

        [HttpGet]//TODO: ver porque no toma por default la categoriaId y la fecha
        public ActionResult Modificacion(int? id)
        {
#if NETCOREAPP
            ViewBag.CategoriaId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(CombosHelpers.GetCategorias(), "Id", "Descripcion");

#else
               ViewBag.CategoriaId = new SelectList(CombosHelpers.GetCategorias(), "Id", "Descripcion");


#endif

            if (id == null)
            {
#if NETCOREAPP
                return BadRequest();
#else
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

#endif
            }
            var Juego = _context.Juego.Find(id);
            var categoria = _context.Categoria.Find(Juego.CategoriaId);
            var imagenes = _context.ImagenesJuegos.Where(x => x.JuegoId == Juego.Id).ToList();

            if (Juego == null)
            {
#if NETCOREAPP
                return NotFound("No se encontró el juego a modificar");
#else
                return HttpNotFound("No se encontró el juego a modificar");

#endif
            }
            var juegoVm = new JuegoViewModel(Juego);
            juegoVm.Categoria = new CategoriaViewModel(categoria);
            juegoVm.Images = imagenes.Select(x => new ImagenJuegoViewModel(x)).ToList();
            return View(juegoVm);
        }


        public ActionResult Modificacion2(int? id)
        {
            ViewBag.CategoriaId = new SelectList(CombosHelpers.GetCategorias(), "Id", "Descripcion");

            if (id == null)
            {
#if NETCOREAPP
                return BadRequest();
#else
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

#endif
            }
            var Juego = _context.Juego.Find(id);
            var categoria = _context.Categoria.Find(Juego.CategoriaId);
            var imagenes = _context.ImagenesJuegos.Where(x => x.JuegoId == Juego.Id).ToList();

            if (Juego == null)
            {
#if NETCOREAPP
                return NotFound("No se encontró el juego a modificar");
#else
                return HttpNotFound("No se encontró el juego a modificar");

#endif
            }
            var juegoVm = new JuegoViewModel(Juego);
            juegoVm.Categoria = new CategoriaViewModel(categoria);
            juegoVm.Images = imagenes.Select(x => new ImagenJuegoViewModel(x)).ToList();
            return View(juegoVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Modificacion(JuegoViewModel juego)
        {
        
                try
                {
                    var JuegoBD = _context.Juego.Find(juego.Id);
                    if (JuegoBD == null)
                    {

                    }
                    _context.Entry(JuegoBD).CurrentValues.SetValues(juego.ToEntity());
                    List<ImagenesJuego> imagenes = GetImagesFromFilesRequest(juego.Image).Select(x => new ImagenesJuego()
                    {
                        JuegoId = juego.Id,
                        Imagen = x.Imagen,
                        ImageName = x.ImageName,
                        IsActive = x.IsActive
                    }).ToList();

                    _context.ImagenesJuegos.AddRange(imagenes);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    return View(juego);
                }
                return RedirectToAction("Index");
            }
        


            [HttpPost]
            public ActionResult EliminarImagen(int ImagenId)
            {
                if (ImagenId == 0)
                {
#if NETCOREAPP
                    return BadRequest();
#else
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

#endif
                }
                var image = _context.ImagenesJuegos.Find(ImagenId);
                if (image == null)
                {
#if NETCOREAPP
                    return BadRequest();
#else
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

#endif

                }
                try
                {
                    _context.ImagenesJuegos.Remove(image);
                    _context.SaveChanges();
                }
                catch (Exception)
                {
#if NETCOREAPP
                    return Problem("Se ha eliminado la imagen");
#else
                 return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError,"Ha ocurrido un error");
#endif
                }
#if NETCOREAPP
                return Ok("Se ha eliminado la imagen");
#else
                
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK, "Se ha eliminado la imagen");
#endif
        }

#if !NETCOREAPP

        private List<ImagenJuegoViewModel> GetImagesFromFilesRequest(List<HttpPostedFileBase> uploadImages)
        {
            List<ImagenJuegoViewModel> ImagenesJuegos = new List<ImagenJuegoViewModel>();
            foreach (var image in uploadImages)
            {
                if (image != null)
                {
                    if (image.ContentLength > 0)
                    {
                        byte[] imageData = null;
                        using (var binaryReader = new BinaryReader(image.InputStream))
                        {
                            imageData = binaryReader.ReadBytes(image.ContentLength);
                        }
                        var headerImage = new ImagenJuegoViewModel
                        {
                            Imagen = imageData,
                            ImageName = image.FileName,
                            IsActive = true
                        };
                        ImagenesJuegos.Add(headerImage);


                    }
                }

            }

            return ImagenesJuegos;

        }
#else
        private List<ImagenJuegoViewModel> GetImagesFromFilesRequest(List<IFormFile> uploadImages)
        {
            List<ImagenJuegoViewModel> ImagenesJuegos = new List<ImagenJuegoViewModel>();
            foreach (var image in uploadImages)
            {
                if (image != null)
                {
                    if (image.Length > 0)
                    {
                        byte[] imageData = null;
                        using (var ms = new MemoryStream())
                        {
                            image.CopyTo(ms);
                            imageData = ms.ToArray();

                        }

                        var headerImage = new ImagenJuegoViewModel
                        {
                            Imagen = imageData,
                            ImageName = image.FileName,
                            IsActive = true
                        };
                        ImagenesJuegos.Add(headerImage);


                    }
                }

            }

            return ImagenesJuegos;

        }
#endif

        public ActionResult GetImagen(int Id)
            {

                return View();
            }

            [HttpGet]
            public ActionResult Detalle(int Id)
            {
                if (Id == 0)
                {
#if NETCOREAPP
                return BadRequest();
#else
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

#endif
            }
            var juego = _context.Juego.Find(Id);
                var images = _context.ImagenesJuegos.Where(x => x.JuegoId == juego.Id).ToList();
                if (juego == null)
                {
#if NETCOREAPP
                return NotFound();
#else
                return HttpNotFound();

#endif
            }
            var juegoVm = new JuegoViewModel(juego);
                juegoVm.Images = images.Select(x => new ImagenJuegoViewModel(x)).ToList();

                return View(juegoVm);

            }
        }
    }




        



    