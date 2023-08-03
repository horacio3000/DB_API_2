using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Formats.Asn1;
using System.Globalization;
using System;
using TESTDB1.Models;
using CsvHelper;
using System.Text.RegularExpressions;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Microsoft.AspNetCore.Hosting;
using System.Reflection.PortableExecutable;
using System.Collections.Generic;
using CsvHelper.Configuration;
using System.Text;
using System.Reflection;

namespace TESTDB1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly IWebHostEnvironment? _webHostEnvironment;
        public MainController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [Route("ProductsList")]
        public IActionResult ProductsList()
        {
            List<ProductModel> productsList = GetProductList();
            return Ok(productsList);
        }

        [HttpGet]
        [Route("ClientLIst")]
        public IActionResult ClientList()
        {
            List<ClientModel> clientslist = GetClientList();
            return Ok(clientslist);
        }


        [HttpGet]
        [Route("Refresh")]
        public IActionResult Refresh()
        {
            string mPathOri = Path.Combine(_webHostEnvironment.ContentRootPath, "cache", "Refresh", "products.csv");
            string mPathNew = Path.Combine(_webHostEnvironment.ContentRootPath, "cache", "products.csv");

            System.IO.File.Delete(mPathNew);
            System.IO.File.Copy(mPathOri, mPathNew);

            return Ok();
        }




        [HttpGet]
        [Route("ArticleDelete/{productid}/{type}")]
        public void ArticleDelete(Int64 productid, int type)
        {
            List<ProductModel> ProductListUpdate = GetProductListCsv();
            List<ProductModel> delete = ProductListUpdate.Where(x => x.productid == productid & x.productType == type).ToList();

            foreach (ProductModel client in delete)
            {
                ProductListUpdate.Remove(client);
            }

            UpdateCSVFile(ProductListUpdate);

        }

        private void UpdateCSVFile(List<ProductModel> ProductListUpdate)
        {
            string mPath = Path.Combine(_webHostEnvironment.ContentRootPath, "cache", "products.csv");
 
            StreamWriter mwr = new StreamWriter(mPath, !true);
            CreateHeader<ProductModel>(ProductListUpdate, mwr);
            mwr.Close();

            mwr = new StreamWriter(mPath, !false);
            CreateRows<ProductModel>(ProductListUpdate, mwr);
            mwr.Close();
        }

        private static void CreateHeader<T>(List<T> list, StreamWriter sw)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length - 1; i++)
            {
                sw.Write(properties[i].Name + ",");
            }
            var lastProp = properties[properties.Length - 1].Name;
            sw.Write(lastProp + sw.NewLine);
        }

        private static void CreateRows<T>(List<T> list, StreamWriter sw)
        {
            foreach (var item in list)
            {
                PropertyInfo[] properties = typeof(T).GetProperties();
                for (int i = 0; i < properties.Length - 1; i++)
                {
                    var prop = properties[i];
                    sw.Write(prop.GetValue(item) + ",");
                }
                var lastProp = properties[properties.Length - 1];
                sw.Write(lastProp.GetValue(item) + sw.NewLine);
            }
        }

        private List<ProductModel> GetProductList()
        {
            List<ProductModel> productsList = GetProductListCsv();
            return productsList;
        }

        private List<ProductModel> GetProductListCsv()
        {
            List<ProductModel> productsList = new List<ProductModel>();
            ProductModel row = new ProductModel();

            string mPath = Path.Combine(_webHostEnvironment.ContentRootPath, "cache", "products.csv");
            using (var reader = new StreamReader(mPath))  
                using (var csv = new CsvReader(reader, CultureInfo.GetCultureInfo("he-il")))
                {
                    var records = csv.GetRecords<ProductModel>();
                    foreach (var record in records)
                    {
                        row = new ProductModel();
                        row.price = record.price;
                        row.prname = record.prname;
                        row.thumb = record.thumb;
                        row.productid = record.productid;
                        row.productType = record.productType;
                        productsList.Add(row);
                    }
                
            }


            return productsList;
        }

        private List<ClientModel> GetClientList()
        {
            List<ClientModel> clientslist = new List<ClientModel>();
            ClientModel row = new ClientModel();

            string mPath = Path.Combine(_webHostEnvironment.ContentRootPath, "cache", "clients.csv");
            using (var reader = new StreamReader(mPath))
            using (var csv = new CsvReader(reader, CultureInfo.GetCultureInfo("he-il")))
            {
                var records = csv.GetRecords<ClientModel>();
                foreach (var record in records)
                {
                    row = new ClientModel();
                    row.isMale = record.isMale;
                    row.cliname = record.cliname;
                    row.dDate = record.dDate;
                    row.idzeut = record.idzeut;
                    row.productType = record.productType;
                    clientslist.Add(row);
                }

            }

            return clientslist;
        }
    }
}
