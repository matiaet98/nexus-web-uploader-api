using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using nexus_web_uploader_api.Model;
using System.Net.Http.Headers;
using System.Text;

namespace nexus_web_uploader_api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ArtefactosController : ControllerBase
    {
        private IConfiguration config;

        public ArtefactosController(IConfiguration iConfig)
        {
            config = iConfig;
        }

        [HttpPost]
        public JsonResult Post([FromForm] Subida subida)
        {
            JsonResult res = new JsonResult(new { });
            try {
                subida.Decode();
                subida.FixSlashes();
                subida.SetFileStream();
                subida.SetHashStream();


                string nexusUrl = config.GetSection("nexusUrl").Value;

                foreach (var fs in new Dictionary<string, FileStream>{
                    { subida.archivo.FileName,subida.archivoStream },
                    { subida.archivo.FileName+".md5",subida.md5Stream },
                    { subida.archivo.FileName+".sha1",subida.sha1Stream }
                })
                {
                    string creds = Convert.ToBase64String(Encoding.ASCII.GetBytes(subida.usuarioSUA + ":" + subida.passwordSUA));
                    using (HttpClient httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", creds);
                        fs.Value.Position = 0;
                        using(StreamContent sc = new StreamContent(fs.Value))
                        {
                            HttpResponseMessage response = httpClient.PutAsync(nexusUrl + subida.repositorio + subida.carpetas + fs.Key, sc).Result;
                            if(response.StatusCode != System.Net.HttpStatusCode.Created)
                            {
                                res.Value = new { message = response.ReasonPhrase};
                                return res;
                            }
                        }
                    }
                }
                res.Value = new { message = "Archivo subido: " + nexusUrl + subida.repositorio + subida.carpetas + subida.archivo.FileName };
                return res;
                
            }
            //catch (Exception e)
            //{
            //    Debug.WriteLine(e.Message);
            //    res.Value = new { message = e.Message };
            //    return res;
            //}
            finally
            {
                subida.Close();
                subida.DeleteFiles();
            }
        }
    }
}
