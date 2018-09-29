using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace nexus_web_uploader_api.Model
{
    public class Subida
{
        public string usuarioSUA { get; set; }
        public string passwordSUA { get; set; }
        public string carpetas { get; set; }
        public string repositorio { get; set; }
        public IFormFile archivo { get; set; }
        internal FileStream archivoStream;
        internal FileStream md5Stream;
        internal FileStream sha1Stream;


        internal void Decode(){
            usuarioSUA = Encoding.UTF8.GetString(Convert.FromBase64String(usuarioSUA));
            passwordSUA = Encoding.UTF8.GetString(Convert.FromBase64String(passwordSUA));
            repositorio = Encoding.UTF8.GetString(Convert.FromBase64String(repositorio));
            if(!string.IsNullOrEmpty(carpetas))
            {
                carpetas = Encoding.UTF8.GetString(Convert.FromBase64String(carpetas));
            }
        }

        internal void FixSlashes()
        {
            repositorio = repositorio + "/";
            if (!string.IsNullOrEmpty(carpetas))
            {
                if (carpetas[0] == '/') carpetas = carpetas.Substring(1);
                if (! (carpetas[carpetas.Length - 1] == '/')) carpetas = carpetas + "/";
            }
        }
        internal void SetFileStream()
        {
            archivoStream = new FileStream(archivo.FileName, FileMode.Create);
            archivo.CopyTo(archivoStream);
        }
        internal void SetHashStream()
        {
            md5Stream = new FileStream(archivo.FileName + ".md5", FileMode.Create);
            sha1Stream = new FileStream(archivo.FileName + ".sha1", FileMode.Create);

            archivoStream.Position = 0;
            byte[] md5arr = GetMD5HashFromFile();
            md5Stream.Write( md5arr, 0, md5arr.Count());

            archivoStream.Position = 0;
            byte[] sha1arr = GetSHA1HashFromFile();
            sha1Stream.Write(sha1arr, 0, sha1arr.Count());
        }

        private byte[] GetMD5HashFromFile()
        {
            return new UTF8Encoding(true)
                .GetBytes(
                BitConverter
                .ToString(
                    MD5
                    .Create()
                    .ComputeHash(archivoStream))
                    .Replace("-", string.Empty));
        }

        private byte[] GetSHA1HashFromFile()
        {
            return new UTF8Encoding(true)
                .GetBytes(
                BitConverter
                .ToString(
                    SHA1
                    .Create()
                    .ComputeHash(archivoStream))
                    .Replace("-", string.Empty));
        }


        ~Subida()
        {
            md5Stream.Dispose();
            sha1Stream.Dispose();
            archivoStream.Dispose();
        }
        internal async void DeleteFiles()
        {
            if (File.Exists(archivo.FileName))
            {
                File.Delete(archivo.FileName);
            }
            if (File.Exists(archivo.FileName+".md5"))
            {
                File.Delete(archivo.FileName+".md5");
            }
            if (File.Exists(archivo.FileName + ".sha1"))
            {
                File.Delete(archivo.FileName + ".sha1");
            }
        }

        internal async void Close()
        {
            try
            {
                md5Stream.Dispose();
            }
            catch (Exception)
            {
            }
            try
            {
                sha1Stream.Dispose();
            }
            catch (Exception)
            {
            }
            try
            {
                archivoStream.Dispose();
            }
            catch (Exception)
            {
            }

        }

    

    }
}
