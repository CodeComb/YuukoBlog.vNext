﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Extensions;
using CodeComb.vNext.Sample.YuukoBlog.Models;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CodeComb.vNext.Sample.YuukoBlog.Controllers
{
    public class FileController : BaseController
    {
        // GET: /<controller>/
        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            if (Context.Session.GetString("Admin") != "true")
                return Json(new
                {
                    code = 403,
                    msg = "Forbidden"
                });

            if(file == null)
                return Json(new
                {
                    code = 400,
                    msg = "File not found"
                });

            var _file = new Blob();
            _file.FileName = file.GetFileName();
            _file.Time = DateTime.Now;
            _file.Id = Guid.NewGuid();
            _file.ContentLength = file.Length;
            _file.ContentType = file.ContentType;
            _file.File = file.ReadAllBytes();
            DB.Blobs.Add(_file);
            DB.SaveChanges();
            return Json(new
            {
                code = 200,
                fileId = _file.Id.ToString()
            });
        }

        [HttpGet]
        public IActionResult Download(Guid id)
        {
            var file = DB.Blobs
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (file == null)
                return Error(404);
            return File(file.File, file.ContentType, file.FileName);
        }
    }
}
